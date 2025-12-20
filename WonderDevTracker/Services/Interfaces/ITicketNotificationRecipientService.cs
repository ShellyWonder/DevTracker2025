using WonderDevTracker.Client;

namespace WonderDevTracker.Services.Interfaces
{
    public interface ITicketNotificationRecipientService
    {
        public Task<string?> GetProjectManagerRecipientAsync(int projectId, UserInfo actor);

        public Task<string?> GetAssignedDeveloperRecipient(int projectId, string assignedUserId, UserInfo actor);

        public string? GetSubmitterRecipient(string? submitterUserId, UserInfo actor);
    }
}
