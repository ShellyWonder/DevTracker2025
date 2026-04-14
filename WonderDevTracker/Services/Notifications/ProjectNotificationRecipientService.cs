using Microsoft.AspNetCore.Identity;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Notifications
{
    // This service is responsible for determining the appropriate recipients for project-related notifications
    // based on the context of the action performed and the roles of the users involved.
    // It provides methods to retrieve recipient identifiers for project managers, company administrators, affected members,
    // and project members while ensuring that notifications are sent to relevant parties without including the actor performing the action.

    public class ProjectNotificationRecipientService(IProjectRepository projectRepository,
                                                     UserManager<ApplicationUser> userManager) : IProjectNotificationRecipientService
    {

        public string? GetAffectedMemberRecipient(string? userId, UserInfo actor)
        {
            if (userId == actor.UserId || string.IsNullOrWhiteSpace(userId)) return null;

            return userId;
        }

        public async Task<string?> GetCompanyAdminRecipientAsync(UserInfo actor)
        {
            var admins = await userManager.GetUsersInRoleAsync(nameof(Role.Admin));
            return admins
                .Where(u => !string.IsNullOrWhiteSpace(u.Id))
                .Where(u => u.CompanyId == actor.CompanyId)
                .Where(u => u.Id != actor.UserId)
                .Select(u => u.Id!)
                .FirstOrDefault();
        }

        public async Task<string?> GetProjectManagerRecipientAsync(int projectId, UserInfo actor)
        {
            var pmId = await projectRepository.GetProjectManagerIdAsync(projectId, actor);

            if (string.IsNullOrWhiteSpace(pmId) || pmId == actor.UserId) return null;

            return pmId;

        }
        public async Task<List<string>> GetProjectMemberRecipientsAsync(int projectId, UserInfo actor)
        {
            var projectMembers = await projectRepository.GetProjectMembersAsync(projectId, actor);
            if (projectMembers is null || !projectMembers.Any()) return [];
            return [.. projectMembers
               .Where(m => !string.IsNullOrWhiteSpace(m.Id))
               .Where(m => m.Id != actor.UserId)
               .Select(m => m.Id!)
               .Distinct()];
        }

        public async Task<List<string>> GetProjectMemberRecipientsExcludingAsync(int projectId, UserInfo actor, params string?[] excludedUserIds)
        {
            var projectMembers = await projectRepository.GetProjectMembersAsync(projectId, actor);
            if (projectMembers is null || !projectMembers.Any()) return [];

            var excluded = excludedUserIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToHashSet();

            excluded.Add(actor.UserId);

            return [.. projectMembers
                .Where(m => !string.IsNullOrWhiteSpace(m.Id))
                .Where(m => !excluded.Contains(m.Id))
                .Select(m => m.Id!)
                .Distinct()];
        }

        public async Task<string?> GetUserDisplayNameAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return null;
            return $"{user.FirstName} {user.LastName}";
        }
    }
   
}
