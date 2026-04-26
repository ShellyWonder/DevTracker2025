using Microsoft.AspNetCore.Identity;
using WonderDevTracker.Client;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Notifications
{
    public class TicketNotificationRecipientService(IProjectRepository projectRepository, UserManager<ApplicationUser> userManager) : ITicketNotificationRecipientService
    {
        public string? GetAssignedDeveloperRecipient(string? assignedUserId, UserInfo actor)
        {
            if (string.IsNullOrWhiteSpace(assignedUserId) || assignedUserId == actor.UserId) return null;

            return assignedUserId;

        }

        public async Task<string?> GetProjectManagerRecipientAsync(int projectId, UserInfo actor)
        {
            var pmId = await projectRepository.GetProjectManagerIdAsync(projectId, actor);

            if (string.IsNullOrWhiteSpace(pmId) || pmId == actor.UserId) return null;

            return pmId;
        }

        public string? GetSubmitterRecipient(string? submitterUserId, UserInfo actor)
        {
            if (string.IsNullOrWhiteSpace(submitterUserId) || submitterUserId == actor.UserId) return null;

            return submitterUserId;
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
