using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class CompanyRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
                                   IServiceScopeFactory scopeFactory,
                                    UserManager<ApplicationUser> userManager) : ICompanyRepository
    {
        public async Task<Company> GetCompanyAsync(UserInfo userInfo)
        {
            await using var context = contextFactory.CreateDbContext();
            Company company = await context.Companies
                .Include(c => c.Members)
                  .FirstAsync(c => c.Id == userInfo.CompanyId); //Cannot be null
            return company;
        }


        public async Task UpdateCompanyAsync(Company company, UserInfo userInfo)
        {
            if (!userInfo.IsInRole(Role.Admin) || company.Id != userInfo.CompanyId) return;

            await using var context = contextFactory.CreateDbContext();
            FileUpload? existingImage = null;
            //check if new image is being added
            if (company.Image is not null && company.Image.Id != company.ImageId)
            {
                //fetch existing image to delete after save changes
                existingImage = await context.Companies
                    .Where(c => c.Id == userInfo.CompanyId)
                    .Select(c => c.Image)
                    .FirstOrDefaultAsync();

                context.Add(company.Image);//save new image
                company.ImageId = company.Image.Id;//update foreign key
            }
            context.Update(company);
            await context.SaveChangesAsync();//save new image and company changes first

            if (existingImage is not null)
            {
                context.Remove(existingImage);//remove old image
                await context.SaveChangesAsync();//save changes again
            }


        }

        #region GET ALL USERS BY COMPANY ID
        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(UserInfo userInfo)
        {
            await using var context = contextFactory.CreateDbContext();

            List<ApplicationUser> users = await context.Users
                .Where(u => u.CompanyId == userInfo.CompanyId)
                .ToListAsync();

            return users;
        }
        #endregion

        #region GET USERS IN ROLE 
        public async Task<IReadOnlyList<ApplicationUser>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var usersInRole = await userManager.GetUsersInRoleAsync(role.ToString());
            return [.. usersInRole.Where(u => u.CompanyId == userInfo.CompanyId)];
        }

        #endregion

        #region UPDATE USER ROLES/ASSIGN ROLES
        //Note:database access is provided via UserManager; no need to open context here
        public async Task AssignUserRoleAsync(string userId, Role newRole, UserInfo userInfo)
        {
            // Only Admins can assign roles, 
            if (!userInfo.IsInRole(Role.Admin)
                // cannot re-assign DemoUser role,
                || newRole == Role.DemoUser
                // user cannot change own role -safeguard against Admin locking self out
                || userId == userInfo.UserId) return;
            //Lookup user to assign
            ApplicationUser? userToAssign = await userManager.FindByIdAsync(userId);

            if (userToAssign?.CompanyId != userInfo.CompanyId) return;
            //remove user's current roles
            var originalRoles = await userManager.GetRolesAsync(userToAssign);

            //verify user is not DemoUser or already in target role
            if (originalRoles.Any(roleName => roleName == nameof(Role.DemoUser)
                             || roleName == Enum.GetName(newRole))) return;
            try
            {
                var removedResult = await userManager.RemoveFromRolesAsync(userToAssign, originalRoles);

                if (!removedResult.Succeeded)
                {
                    throw new ApplicationException(string.Join(string.Join(",", removedResult.Errors.Select(e => e.Description))));
                }
                var addedResult = await userManager.AddToRoleAsync(userToAssign, Enum.GetName(newRole)!);

                if (!addedResult.Succeeded)
                {
                    throw new ApplicationException(string.Join(string.Join(",", addedResult.Errors.Select(e => e.Description))));
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error removing roles: {ex.Message}");
                //Reassign previous roles in case of failure
                await userManager.AddToRolesAsync(userToAssign, originalRoles);
                //throw to UI to show error msg
                throw;
            }
        }

        #endregion

        #region DASHBOARD DATA

        public async Task<DashboardDTO> GetDashboardDataAsync(UserInfo userInfo)
        {
            await using var context = contextFactory.CreateDbContext();

            //Primarily for admin dashboard consumption, so we will pull all company data.
            //For user - specific dashboards, we will filter by user assignments in the query to pull only relevant data.
            //For PM, Dev and Submitter queries, we are filtering out archived projects and tickets to focus on active work.

            IQueryable<Project> allCompanyProjects = GetCompanyProjectsQuery(context, userInfo.CompanyId);
            IQueryable<Ticket> allCompanyTickets = GetCompanyTicketsQuery(context, userInfo.CompanyId);
            IQueryable<Project> pmProjects = GetPMProjectsQuery(context, userInfo.CompanyId, userInfo.UserId);
            IQueryable<Ticket> pmTickets = GetPMProjectTicketsQuery(context, userInfo.CompanyId, userInfo.UserId);
            IQueryable<Project> devProjects = GetDeveloperProjectsQuery(context, userInfo.CompanyId, userInfo.UserId);
            IQueryable<Ticket> devTickets = GetDeveloperTicketsQuery(context, userInfo.CompanyId, userInfo.UserId);
            IQueryable<Ticket> submitterTickets = GetSubmitterTicketsQuery(context, userInfo.CompanyId, userInfo.UserId);
            IQueryable<Ticket> adminCompanyTickets = GetAdminCompanyTicketsQuery(context, userInfo.CompanyId);


            DashboardDTO dashboard = new()
            {
                CompanyInfo = await GetCompanyInfoAsync(context, userInfo.CompanyId),
                CompanyStats = await GetCompanyDashboardStatsAsync(allCompanyProjects, allCompanyTickets),
                PMStats = await GetPMDashboardStatsAsync(pmProjects, pmTickets),
                DevStats = await GetDeveloperDashboardStatsAsync(devProjects, devTickets),
                SubmitterStats = await GetSubmitterDashboardStatsAsync(submitterTickets),
                RecentActiveTickets = await GetRecentTicketSummariesAsync(
                                       GetRecentActiveTicketsQuery(adminCompanyTickets)),
                RecentResolvedTickets = await GetRecentTicketSummariesAsync(
                                        GetRecentResolvedTicketsQuery(adminCompanyTickets)),
                RecentUnassignedTickets = await GetRecentTicketSummariesAsync(
                                        GetRecentUnassignedTicketsQuery(adminCompanyTickets)),
                ChartData = await GetTicketsOverTimeDataAsync(context, userInfo.CompanyId)
            };

            return dashboard;
        }
        #region Query Builders
        //For company-wide(ADMIN) stats
        private static IQueryable<Project> GetCompanyProjectsQuery(ApplicationDbContext context, int companyId)
        {
            return context.Projects
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId);
        }

        private static IQueryable<Ticket> GetCompanyTicketsQuery(ApplicationDbContext context, int companyId)
        {
            return context.Tickets
                .AsNoTracking()
                .Where(t => t.Project!.CompanyId == companyId);
        }

        //For PM-specific stats
        private static IQueryable<Project> GetPMProjectsQuery(ApplicationDbContext context, int companyId, string userId)
        {
            return context.Projects
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId && p.Members!.Any(m => m.Id == userId) && !p.Archived);
        }
        private static IQueryable<Ticket> GetPMProjectTicketsQuery(ApplicationDbContext context, int companyId, string userId)
        {
            return context.Tickets
                .AsNoTracking()
                .Where(t => t.Project!.CompanyId == companyId &&
                !t.Archived &&
                !t.Project!.Archived &&
                t.Project!.Members!.Any(m => m.Id == userId));
        }

        //For Developer-specific stats
        private static IQueryable<Project> GetDeveloperProjectsQuery(ApplicationDbContext context, int companyId, string userId)
        {
            return context.Projects
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId && p.Members!.Any(m => m.Id == userId) && !p.Archived);
        }

        private static IQueryable<Ticket> GetDeveloperTicketsQuery(ApplicationDbContext context, int companyId, string userId)
        {
            return context.Tickets
                .AsNoTracking()
                .Where(t => t.Project!.CompanyId == companyId &&
                !t.Archived &&
                !t.Project!.Archived &&
                t.DeveloperUserId == userId);
        }

        //For Submitter-specific stats
        private static IQueryable<Ticket> GetSubmitterTicketsQuery(ApplicationDbContext context, int companyId, string userId)
        {
            return context.Tickets
                .AsNoTracking()
                .Where(t => t.Project!.CompanyId == companyId &&
                !t.Archived &&
                !t.Project!.Archived &&
                t.SubmitterUserId == userId);
        }

        //For Admin dashboard ticket summaries, we want to pull all active tickets across the company, regardless of assignment, but still filter out archived items.
        private static IQueryable<Ticket> GetAdminCompanyTicketsQuery(ApplicationDbContext context, int companyId)
        {
            return context.Tickets
                .AsNoTracking()
                .Where(t => t.Project!.CompanyId == companyId &&
                !t.Archived &&
                !t.Project!.Archived);
        }
        private static IQueryable<Ticket> GetRecentActiveTicketsQuery(IQueryable<Ticket> tickets)
        {
            return tickets.Where(t => t.Status != TicketStatus.Resolved);
        }

        private static IQueryable<Ticket> GetRecentResolvedTicketsQuery(IQueryable<Ticket> tickets)
        {
            return tickets.Where(t => t.Status == TicketStatus.Resolved);
        }

        private static IQueryable<Ticket> GetRecentUnassignedTicketsQuery(IQueryable<Ticket> tickets)
        {
            return tickets.Where(t => t.Status != TicketStatus.Resolved
                                      && string.IsNullOrWhiteSpace(t.DeveloperUserId));
        }
        #endregion

        #region Role-Specific Stats Calculators
        private static async Task<CompanyDashboardStatsDTO> GetCompanyDashboardStatsAsync(
                                                                IQueryable<Project> companyProjects,
                                                                IQueryable<Ticket> companyTickets)
        {
            return new CompanyDashboardStatsDTO
            {
                TotalProjectCount = await companyProjects.CountAsync(),
                TotalTicketCount = await companyTickets.CountAsync(),
                OpenTicketCount = await companyTickets
                                        .CountAsync(t => t.Status != TicketStatus.Resolved),
                ResolvedTicketCount = await companyTickets
                                        .CountAsync(t => t.Status == TicketStatus.Resolved)
            };
        }

        private static async Task<PMDashboardStatsDTO> GetPMDashboardStatsAsync(
                                                                IQueryable<Project> pmProjects,
                                                                IQueryable<Ticket> pmTickets)
        {
            return new PMDashboardStatsDTO
            {
                ManagedProjectCount = await pmProjects.CountAsync(),
                ActiveManagedTicketCount = await pmTickets.CountAsync(),
                OpenManagedTicketCount = await pmTickets
                                        .CountAsync(t => t.Status != TicketStatus.Resolved),
                ResolvedManagedTicketCount = await pmTickets
                                        .CountAsync(t => t.Status == TicketStatus.Resolved)
            };
        }
        private static async Task<DevDashboardStatsDTO> GetDeveloperDashboardStatsAsync(
                                                                IQueryable<Project> devProjects,
                                                                IQueryable<Ticket> devTickets)
        {
            return new DevDashboardStatsDTO
            {
                AssignedProjectsCount = await devProjects.CountAsync(),
                AssignedTicketCount = await devTickets.CountAsync(),
                OpenAssignedTicketCount = await devTickets
                    .CountAsync(t => t.Status != TicketStatus.Resolved),
                ResolvedAssignedTicketCount = await devTickets
                    .CountAsync(t => t.Status == TicketStatus.Resolved)
            };
        }
        private static async Task<SubmitterDashboardStatsDTO> GetSubmitterDashboardStatsAsync(
                                                                IQueryable<Ticket> submitterTickets)
        {
            return new SubmitterDashboardStatsDTO
            {
                SubmittedTicketCount = await submitterTickets.CountAsync(),
                OpenSubmittedTicketCount = await submitterTickets
                                                .CountAsync(t => t.Status != TicketStatus.Resolved),
                ResolvedSubmittedTicketCount = await submitterTickets
                                                .CountAsync(t => t.Status == TicketStatus.Resolved)
            };
        }
        #endregion

        #region Company Info DTO
        private static async Task<CompanyDashboardInfoDTO> GetCompanyInfoAsync(ApplicationDbContext context, int companyId)
        {
            return await context.Companies
                .Where(c => c.Id == companyId)
                .Select(c => new CompanyDashboardInfoDTO
                {
                    CompanyId = c.Id,
                    CompanyName = c.Name ?? "Company",
                    //ImageUrl = c.Image != null ? $"/fileuploads/{c.Image.Id}" : null
                })
                .FirstOrDefaultAsync() ?? throw new ApplicationException("Company not found");
        }
        #endregion

        #region Ticket Info DTO
        private static Expression<Func<Ticket, DashboardTicketSummaryDTO>> TicketSummaryProjection =>
                t => new DashboardTicketSummaryDTO
                {
                    Id = t.Id,
                    Title = t.Title!,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project!.Name!,
                    Status = t.Status,
                    Priority = t.Priority,
                    Type = t.Type,

                    SubmitterUser = t.SubmitterUser == null
                    ? null
                    : new AppUserDTO
                    {
                        Id = t.SubmitterUser.Id,
                        FirstName = t.SubmitterUser.FirstName,
                        LastName = t.SubmitterUser.LastName,
                        // Use your actual AppUserDTO image property name here.
                        ImageUrl = t.SubmitterUser.ProfilePictureId == null
                                                                    ? null
                                                                    : $"/api/uploads/{t.SubmitterUser.ProfilePictureId}",

                        // Only include this if Initials is settable, not computed.
                        Initials = t.SubmitterUser.FirstName.Substring(0, 1)
                     + t.SubmitterUser.LastName.Substring(0, 1)
                    },


                    DeveloperUser = t.DeveloperUser == null
            ? null
            : new AppUserDTO
            {
                Id = t.DeveloperUser.Id,
                FirstName = t.DeveloperUser.FirstName,
                LastName = t.DeveloperUser.LastName,

                ImageUrl = t.DeveloperUser.ProfilePictureId == null
                    ? null
                    : $"/api/uploads/{t.DeveloperUser.ProfilePictureId}",

                Initials = t.DeveloperUser.FirstName.Substring(0, 1)
                         + t.DeveloperUser.LastName.Substring(0, 1)
            },
                    Created = t.Created,
                    Updated = t.Updated
                };

        private static Task<List<DashboardTicketSummaryDTO>> GetRecentTicketSummariesAsync(
                        IQueryable<Ticket> query,
                        int take = 10)
        {
            return query
                .OrderByDescending(t => t.Updated ?? t.Created)
                .Take(take)
                .Select(TicketSummaryProjection)
                .ToListAsync();
        }

        #endregion

        #region CHART DATA
        private static async Task<DashboardChartDataDTO> GetTicketsOverTimeDataAsync(ApplicationDbContext context, int companyId)
        {

            List<DashboardMonthlyTicketsDTO> ticketsOverTime = [];
            List<DashboardMonthlyTicketsDTO> resolvedTicketsOverTime = [];
           
            IQueryable<Ticket> allCompanyTickets = GetCompanyTicketsQuery(context, companyId);

            //1.Calculate the date range for the past 12 months
            DateTimeOffset now = DateTimeOffset.UtcNow;

            //loop over the previous 12 months
            for (int monthsAgo = 12; monthsAgo >= 0; monthsAgo--)
            {
                int year = now.Year;
                int month = now.Month - monthsAgo;

                if (month <= 0)
                {
                    year -= 1;
                    month += 12;
                }
                //Set to first day of month for consistent grouping
                var thisMonth = new DateTimeOffset(
                    year,
                    month,
                    day: 1,
                    hour: 0,
                    minute: 0,
                    second: 0,
                    TimeSpan.Zero);

                //Set the end of the month by adding 1 month 
                var nextMonth = new DateTimeOffset(
                    year: month == 12 ? year + 1 : year,
                    month: month == 12 ? 1 : month + 1,
                    day: 1,
                    hour: 0,
                    minute: 0,
                    second: 0,
                    TimeSpan.Zero
                    );

                //query db for tickets created in that month
                var createdCount = await allCompanyTickets.CountAsync(t => t.Created >= thisMonth && t.Created < nextMonth);

                //query db for tickets resolved and updatedin that month
                var resolvedCount = await allCompanyTickets.Where(t => t.Status == TicketStatus.Resolved)
                                                               .CountAsync(t => t.Updated.HasValue
                                                               ? (t.Updated.Value >= thisMonth && t.Updated.Value < nextMonth)
                                                               : (t.Created >= thisMonth && t.Created < nextMonth)
                                                                );
                //ignore first month if no data to show on chart
                if (ticketsOverTime.Count > 0 || resolvedTicketsOverTime.Count > 0 || createdCount > 0 || resolvedCount > 0)
                {
                    ticketsOverTime.Add(new DashboardMonthlyTicketsDTO
                    {
                        Month = thisMonth,
                        Count = createdCount
                    });
                    resolvedTicketsOverTime.Add(new DashboardMonthlyTicketsDTO
                    {
                        Month = thisMonth,
                        Count = resolvedCount
                    });
                    
                }


            }

            return new DashboardChartDataDTO
            {
                TicketsOverTime = ticketsOverTime,
                ResolvedTicketsOverTime = resolvedTicketsOverTime
            };
        }
        #endregion

        #endregion
    }
}
