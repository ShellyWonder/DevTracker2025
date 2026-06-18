//DashboardRepository.Developer.cs 

using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Repositories
{
    //Developer-specific methods 
    public partial class DashboardRepository
    {
        private static async Task<List<DashboardTicketSummaryDTO>> GetDeveloperAssignedTicketSummariesAsync(
                                                                   IQueryable<Ticket> devTickets,
                                                                   int take = 10) => await devTickets
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
}
