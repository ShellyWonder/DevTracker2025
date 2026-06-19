//DashboardRepository.Summaries.cs

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Repositories
{
   
    // Contains methods related to aggregating and retrieving summary data for the dashboard, shared across different user roles.
   
    public partial class DashboardRepository
    {

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

        private sealed class ProjectMemberPreviewRow
        {
            public int ProjectId { get; set; }
            public string UserId { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? UserName { get; set; }
            public Guid? ProfilePictureId { get; set; }
        }
    }
}
