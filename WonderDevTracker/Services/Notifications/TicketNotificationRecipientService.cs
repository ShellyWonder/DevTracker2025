using WonderDevTracker.Client;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Notifications
{
    public class TicketNotificationRecipientService(IProjectRepository projectRepository) : ITicketNotificationRecipientService
    {
        public string? GetAssignedDeveloperRecipient(string? assignedUserId, UserInfo actor)
        {
            if (assignedUserId ==actor.UserId) return null;

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
    }
}
