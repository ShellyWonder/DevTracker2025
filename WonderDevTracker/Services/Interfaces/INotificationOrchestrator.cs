using WonderDevTracker.Client;

namespace WonderDevTracker.Services.Interfaces
{
    public interface INotificationOrchestrator
    {
        public Task TicketCreatedAsync(int ticketId, UserInfo user);

        public Task TicketAssignedAsync(int ticketId, string assignedUserId, UserInfo user);

        public Task TicketResolvedAsync(int ticketId, UserInfo user);

        public Task TicketArchivedAsync(int ticketId, UserInfo user);
    }
}
