//DashboardRepository.ProjectManager.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Repositories
{
    /// <summary>
    /// Contains methods related to aggregating and retrieving dashboard data specific to Project Managers.
    /// </summary>
    public partial class DashboardRepository
    {
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


    }
}
