using WonderDevTracker.Client;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Notifications
{
    public class TicketNotificationRecipientService(IProjectRepository projectRepository) : ITicketNotificationRecipientService
    {
        public Task<string?> GetAssignedDeveloperRecipient(int projectId, string assignedUserId, UserInfo actor)
        {
            if (string.Equals(assignedUserId, actor.UserId, StringComparison.Ordinal))
                return Task.FromResult<string?>(null);

            return Task.FromResult<string?>(assignedUserId);
        }

        public async Task<string?> GetProjectManagerRecipientAsync(int projectId, UserInfo actor)
        {
            var pmId = await projectRepository.GetProjectManagerIdAsync(projectId, actor);

            if (string.IsNullOrWhiteSpace(pmId)) return null;

            if (string.Equals(pmId, actor.UserId, StringComparison.Ordinal)) return null;

            return pmId;
        }

        public  string? GetSubmitterRecipient(string? submitterUserId, UserInfo actor)
        {
            if (string.IsNullOrWhiteSpace(submitterUserId)) return null;

            if (string.Equals(submitterUserId, actor.UserId, StringComparison.Ordinal)) return null;

            return submitterUserId;
        }
    }
}
