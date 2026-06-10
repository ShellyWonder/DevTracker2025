//CompanyRepository.Dashboard.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
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


            //For user - specific dashboards, filter by user assignments in the query to pull only relevant data.
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
                DevDashboard = await GetDeveloperDashboardDataAsync(context, userInfo.CompanyId, userInfo.UserId),
                SubmitterStats = await GetSubmitterDashboardStatsAsync(submitterTickets),
                RecentActiveTickets = await GetRecentTicketSummariesAsync(
                                       GetRecentActiveTicketsQuery(adminCompanyTickets)),
                RecentResolvedTickets = await GetRecentTicketSummariesAsync(
                                        GetRecentResolvedTicketsQuery(adminCompanyTickets)),
                RecentUnassignedTickets = await GetRecentTicketSummariesAsync(
                                        GetRecentUnassignedTicketsQuery(adminCompanyTickets)),
                ChartData = await GetDashboardChartDataAsync(context, userInfo.CompanyId),
                MySubmittedTickets = await GetMySubmittedTicketsAsync(context, userInfo.CompanyId, userInfo.UserId),
                RecentProjects = await GetProjectSummariesAsync(GetActiveCompanyProjectsQuery(context, userInfo.CompanyId))

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
        private static IQueryable<Project> GetActiveCompanyProjectsQuery(ApplicationDbContext context, int companyId)
        {
            return GetCompanyProjectsQuery(context, companyId)
                .Where(p => !p.Archived);
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
                     .Where(t =>
                             t.Project != null &&
                             t.Project.CompanyId == companyId &&
                             !t.Archived &&
                             !t.ArchivedByProject &&
                             !t.Project.Archived &&
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
            return tickets.Where(t => t.DeveloperUserId == null)
                .Where(t => t.Status != TicketStatus.Resolved)
                .Where(t => !t.Archived)
                .Where(t => !t.Project!.Archived);
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
                ManagedProjects = await GetProjectSummariesAsync(pmProjects),
                UnassignedTickets = await GetPMUnassignedTicketsAsync(pmTickets),
                PMChartData = await GetPMDashboardChartDataAsync(pmProjects, pmTickets),
                TeamMembers = await GetPMTeamMembersAsync(context, companyId, userId, userManager)
            };
        }

        private static async Task<List<DashboardTicketSummaryDTO>> GetPMUnassignedTicketsAsync(IQueryable<Ticket> pmTickets)
        {
            return await pmTickets
                        .Where(t => string.IsNullOrWhiteSpace(t.DeveloperUserId))
                        .Where(t => t.Status != TicketStatus.Resolved)
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
                                                ImageUrl = t.SubmitterUser.ProfilePictureId == null
                                                                            ? null
                                                                            : $"/api/uploads/{t.SubmitterUser.ProfilePictureId}",

                                                Initials = UserDisplayHelper.GetInitials(
                                                    t.SubmitterUser.FirstName,
                                                    t.SubmitterUser.LastName,
                                                    t.SubmitterUser.UserName)
                                            }

                        })
        .ToListAsync();
        }

        private static async Task<List<DashboardProjectSummaryDTO>> GetProjectSummariesAsync(
                                                                        IQueryable<Project> projects)
        {
            const int maxVisibleMembers = 5;

            // Step 1: Keep the project summary projection flat.
            List<DashboardProjectSummaryDTO> summaries = await projects
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
                                                    !t.Archived &&
                                                    t.Status != TicketStatus.Resolved &&
                                                    string.IsNullOrWhiteSpace(t.DeveloperUserId)),

                    Members = new List<AppUserDTO>()
                })
                .ToListAsync();

            if (summaries.Count == 0)
                return summaries;

            // Step 2: Pull only the project ids we need.
            List<int> projectIds = [.. summaries.Select(p => p.Id)];

            // Step 3: Query project/member pairs separately.
            // This keeps EF from having to build a nested DTO graph inside one projection.
            List<ProjectMemberPreviewRow> memberRows = await projects
    .Where(p => projectIds.Contains(p.Id))
    .SelectMany(p => p.Members!
        .Select(m => new ProjectMemberPreviewRow
        {
            ProjectId = p.Id,
            UserId = m.Id,
            FirstName = m.FirstName,
            LastName = m.LastName,
            UserName = m.UserName,
            ProfilePictureId = m.ProfilePictureId
        }))
    .ToListAsync();

            // Step 4: Group in memory and attach previews to the summary DTOs.
            Dictionary<int, List<AppUserDTO>> membersByProject = memberRows
                .GroupBy(m => m.ProjectId)
                .ToDictionary(
                    g => g.Key,
                    g => g
                    .OrderBy(m => m.LastName)
                    .ThenBy(m => m.FirstName)
                    .Take(maxVisibleMembers)
                    .Select(m => new AppUserDTO
                    {
                        Id = m.UserId,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        ImageUrl = m.ProfilePictureId == null
                                        ? null
                                        : $"/api/uploads/{m.ProfilePictureId}",

                        Initials = UserDisplayHelper.GetInitials(
                                                    m.FirstName,
                                                    m.LastName,
                                                    m.UserName)
                    }).ToList());

            foreach (DashboardProjectSummaryDTO summary in summaries)
            {
                if (membersByProject.TryGetValue(summary.Id, out List<AppUserDTO>? members))
                {
                    summary.Members = members;
                }
            }

            return summaries;
        }

        private sealed class ProjectMemberPreviewRow
        {
            public int ProjectId { get; set; }
            public string UserId { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? UserName { get; set; }
            public Guid? ProfilePictureId { get; set; }
        }

        #endregion
        #region Developer Dashboard
        private static async Task<DeveloperDashboardDTO> GetDeveloperDashboardDataAsync(
                                                                    ApplicationDbContext context,
                                                                    int companyId,
                                                                    string userId)
        {
            var devTickets = GetDeveloperTicketsQuery(context, companyId, userId);
            

            return new DeveloperDashboardDTO
            {
                DevStats = await GetDeveloperDashboardStatsAsync(devTickets),
                DevProjects = await GetProjectSummariesAsync(GetDeveloperProjectsQuery(context, companyId, userId)),
                AssignedTickets = await GetDeveloperAssignedTicketSummariesAsync(devTickets),
                DevChartData = await GetDeveloperDashboardChartDataAsync(devTickets)
            };
        }

        private static async Task<List<DashboardTicketSummaryDTO>> GetDeveloperAssignedTicketSummariesAsync(
    IQueryable<Ticket> devTickets,
    int take = 10)
        {
            return await devTickets
                .OrderBy(t => t.Status == TicketStatus.Resolved)
                .ThenByDescending(t => t.Priority)
                .ThenByDescending(t => t.Updated ?? t.Created)
                .Take(take)
                .Select(t => new DashboardTicketSummaryDTO
                {
                    Id = t.Id,
                    Title = t.Title ?? "Untitled",
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project != null ? t.Project.Name ?? "Unknown Project" : "Unknown Project",
                    Type = t.Type,
                    Priority = t.Priority,
                    Status = t.Status,
                    Created = t.Created,
                    Updated = t.Updated,
                   
                })
                .ToListAsync();
        }
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
        private static async Task<DevDashboardStatsDTO> GetDeveloperDashboardStatsAsync(IQueryable<Ticket> devTickets)
        {
            return new DevDashboardStatsDTO
            {
                AssignedTicketCount = await devTickets.CountAsync(),
                OpenAssignedTicketCount = await devTickets
                    .CountAsync(t => t.Status != TicketStatus.Resolved),
                InProgressCount = await devTickets
                    .CountAsync(t =>
                        t.Status == TicketStatus.InDevelopment ||
                        t.Status == TicketStatus.InTesting),
                ResolvedCount = await devTickets
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

                        Initials = UserDisplayHelper.GetInitials(
                                            t.SubmitterUser.FirstName,
                                            t.SubmitterUser.LastName,
                                            t.SubmitterUser.UserName)
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

                                        Initials = UserDisplayHelper.GetInitials(
                                            t.DeveloperUser.FirstName,
                                            t.DeveloperUser.LastName,
                                            t.DeveloperUser.UserName)
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
    ApplicationDbContext context,
    int companyId,
    string userId,
    UserManager<ApplicationUser> userManager)
        {
            List<string> memberIds = await context.Projects
                .AsNoTracking()
                .Where(p =>
                    p.CompanyId == companyId &&
                    !p.Archived &&
                    p.Members!.Any(m => m.Id == userId))
                .SelectMany(p => p.Members!)
                .Select(m => m.Id)
                .Distinct()
                .ToListAsync();

            List<ApplicationUser> members = await context.Users
                .AsNoTracking()
                .Where(u => memberIds.Contains(u.Id))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            List<AppUserDTO> dtos = [];

            foreach (ApplicationUser member in members)
            {
                try
                {
                    AppUserDTO dto = await member.ToDTOWithRole(userManager);
                    dtos.Add(dto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"PM TeamMembers mapping failed for user: {member.Id} | {member.Email} | {member.FirstName} {member.LastName}");
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }

            return dtos;
        }
        #endregion
    }
}
