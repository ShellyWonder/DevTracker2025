using WonderDevTracker.Client;

namespace WonderDevTracker.Services.Interfaces
{
    public interface ITicketNotificationRecipient
    {
        public Task<string?> GetProjectManagerRecipientAsync(int projectId, UserInfo actor);
        public Task<string?> GetAssignedDeveloperRecipientAsync(int projectId, string assignedUserId, UserInfo actor);
    }
}
