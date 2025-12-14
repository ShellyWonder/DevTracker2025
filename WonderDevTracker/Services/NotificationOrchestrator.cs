using WonderDevTracker.Client;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{

    ///Coordinates notification delivery and management within the application.
    ///Responsibilities:
    //Resolve recipients(PM, assigned dev, submitter, project members, admins, etc.)
    //Build notification objects(title/message/type/entity ids)
    //Persist via INotificationRepository.AddRangeAsync(...)
    

    public class NotificationOrchestrator : INotificationOrchestrator
    {
        public Task TicketArchivedAsync(int ticketId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task TicketAssignedAsync(int ticketId, string assignedUserId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task TicketCreatedAsync(int ticketId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task TicketResolvedAsync(int ticketId, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
