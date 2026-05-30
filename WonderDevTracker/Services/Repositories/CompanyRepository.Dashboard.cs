//CompanyRepository.Dashboard.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Repositories
{
    public partial class CompanyRepository
    {

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
                PMDashboard = await GetPMDashboardDataAsync(context, userInfo.CompanyId, userInfo.UserId, userManager),
                DevStats = await GetDeveloperDashboardStatsAsync(devProjects, devTickets),
                SubmitterStats = await GetSubmitterDashboardStatsAsync(submitterTickets),
                RecentActiveTickets = await GetRecentTicketSummariesAsync(
                                       GetRecentActiveTicketsQuery(adminCompanyTickets)),
                RecentResolvedTickets = await GetRecentTicketSummariesAsync(
                                        GetRecentResolvedTicketsQuery(adminCompanyTickets)),
                RecentUnassignedTickets = await GetRecentTicketSummariesAsync(
                                        GetRecentUnassignedTicketsQuery(adminCompanyTickets)),
                ChartData = await GetDashboardChartDataAsync(context, userInfo.CompanyId),
                MySubmittedTickets = await GetMySubmittedTicketsAsync(context, userInfo.CompanyId, userInfo.UserId)

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


        /// <summary>
        /// For Admin dashboard ticket summaries, pull all active company tickets,
        /// regardless of assignment, but still filter out archived items.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
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

        #region Role-Specific Dashboard Aggregation Methods
        #region PM Dashboard
        private static async Task<PMDashboardDTO> GetPMDashboardDataAsync(ApplicationDbContext context, 
                                                                          int companyId, string userId, 
                                                                          UserManager<ApplicationUser> userManager)
        {
            IQueryable<Project> pmProjects = GetPMProjectsQuery(context, companyId, userId);
            IQueryable<Ticket> pmTickets = GetPMProjectTicketsQuery(context, companyId, userId);

            return new PMDashboardDTO
            {
                PMStats = await GetPMDashboardStatsAsync(pmProjects, pmTickets),
                ManagedProjects = await GetPMManagedProjectsAsync(pmProjects),
                UnassignedTickets = await GetPMUnassignedTicketsAsync(pmTickets),
                PMChartData = await GetPMDashboardChartDataAsync(pmProjects, pmTickets),
                TeamMembers = await GetPMTeamMembersAsync(pmProjects, userManager)
            };
        }

        private static async Task<List<DashboardTicketSummaryDTO>> GetPMUnassignedTicketsAsync(IQueryable<Ticket> pmTickets)
        {
            return await pmTickets
                        .Where(t => string.IsNullOrWhiteSpace(t.DeveloperUserId))
                        .OrderByDescending(t => t.Updated ?? t.Created)
                        .Select(t => new DashboardTicketSummaryDTO
                        {
                            Id = t.Id,
                            Title = t.Title ?? string.Empty,

                            ProjectId = t.ProjectId,
                            ProjectName = t.Project!.Name ?? string.Empty,

                            Type = t.Type,
                            Priority = t.Priority,
                            Status = t.Status,

                            Created = t.Created,
                            Updated = t.Updated,

                            SubmitterUser = t.SubmitterUser == null
                                            ? null
                                            : new AppUserDTO
                                            {
                                                Id = t.SubmitterUser.Id,
                                                FirstName = t.SubmitterUser.FirstName,
                                                LastName = t.SubmitterUser.LastName,
                   
                                            }

                        })
        .ToListAsync();
        }

        private static async Task<List<DashboardProjectSummaryDTO>> GetPMManagedProjectsAsync(IQueryable<Project> pmProjects)
                                                                                               
        {
            return await pmProjects
            .OrderByDescending(p => p.Created)
            .Select(p => new DashboardProjectSummaryDTO
            {
                Id = p.Id,
                Name = p.Name ?? string.Empty,
                Description = p.Description,
                Priority = p.Priority,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                MemberCount = p.Members == null ? 0 : p.Members.Count,
                OpenTicketCount = p.Tickets.Count(t =>
                    !t.Archived && t.Status != TicketStatus.Resolved),
                UnassignedTicketCount = p.Tickets.Count(t =>
                    !t.Archived && string.IsNullOrWhiteSpace(t.DeveloperUserId))
            })
            .ToListAsync();
        }
        #endregion
        #region Developer Dashboard

        #endregion

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

        private static async Task<List<DashboardTicketSummaryDTO>> GetMySubmittedTicketsAsync(
            ApplicationDbContext context,
            int companyId,
            string userId,
            int take = 10)
        {
            return await GetSubmitterTicketsQuery(context, companyId, userId)
                .OrderByDescending(t => t.Updated ?? t.Created)
                .Take(take)
                .Select(t => new DashboardTicketSummaryDTO
                {
                    Id = t.Id,
                    Title = t.Title ?? "Untitled",
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project!.Name ?? "Unknown",
                    Status = t.Status,
                    Priority = t.Priority,
                    Type = t.Type,
                    Created = t.Created,
                    Updated = t.Updated,
                    DeveloperUser = t.DeveloperUser == null
                                    ? null
                                    : t.DeveloperUser.ToDTO(),

                    SubmitterUser = t.SubmitterUser == null
                                    ? null
                                    : t.SubmitterUser.ToDTO()
                })
                .ToListAsync();
        }

        #endregion

        private static async Task<List<AppUserDTO>> GetPMTeamMembersAsync(
                                IQueryable<Project> pmProjects,
                                UserManager<ApplicationUser> userManager)
        {
            List<ApplicationUser> members = await pmProjects
                .SelectMany(p => p.Members!)
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .OrderBy(m => m.LastName)
                .ThenBy(m => m.FirstName)
                .ToListAsync();

            var dtoTasks = members.Select(member => member.ToDTOWithRole(userManager));
            AppUserDTO[] dtos = await Task.WhenAll(dtoTasks);

            return [.. dtos];
        }
        #endregion
    }
}
