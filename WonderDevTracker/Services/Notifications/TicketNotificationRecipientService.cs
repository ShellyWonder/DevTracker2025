using WonderDevTracker.Client;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Notifications
{
    public class TicketNotificationRecipientService : ITicketNotificationRecipient
    {
        public Task<string?> GetAssignedDeveloperRecipientAsync(int projectId, string assignedUserId, UserInfo actor)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetProjectManagerRecipientAsync(int projectId, UserInfo actor)
        {
            throw new NotImplementedException();
        }
    }
}
