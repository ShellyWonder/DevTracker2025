using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
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

            return new DashboardDTO
            {
                CompanyStats = await GetCompanyDashboardStatsAsync(allCompanyProjects, allCompanyTickets),
                PMStats = await GetPMDashboardStatsAsync(pmProjects, pmTickets),
                DevStats = await GetDeveloperDashboardStatsAsync(devProjects, devTickets),
                SubmitterStats = await GetSubmitterDashboardStatsAsync(submitterTickets)
            };
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
        #endregion
    }
}
