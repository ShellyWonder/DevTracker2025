using WonderDevTracker.Client;

namespace WonderDevTracker.Services.Interfaces
{
    public interface INotificationOrchestrator
    {
        /// <summary>
        /// Ticket Created Notification
        /// </summary>
        /// <param name="ticketId">Ticket id.</param>
        /// <param name="user">Current user's claims Cannot be null.</param>
        /// <returns>Handles the event when a new ticket is created by performing any necessary processing or notifications</returns>
        public Task TicketCreatedAsync(int ticketId, UserInfo user);

        /// <summary>
        /// Ticket Assignment Notification
        /// </summary>
        /// <param name="ticketId">The ticket id.</param>
        /// <param name="assignedUserId">The identifier of the user to whom the ticket is being assigned. Cannot be null or empty.</param>
        /// <param name="user">Current user's claims. Cannot be null.</param>
        /// <returns>Handles the assignment of a ticket to a specified user asynchronously..</returns>
        public Task TicketAssignedAsync(int ticketId, string assignedUserId, UserInfo user);

        /// <summary>
        /// Ticket Resolved Notification
        /// </summary>
        /// <param name="ticketId">Ticket's id.</param>
        /// <param name="user">Current user's claims. Cannot be null.</param>
        /// <remarks>Handles resolution notification.</remarks>
        public Task TicketResolvedAsync(int ticketId, UserInfo user);

        /// <summary>
        /// Ticket Archived Notification
        /// </summary>
        /// <param name="ticketId">Ticket id.</param>
        /// <param name="user">Current user's claims. Cannot be null.</param>
        /// <remarks>Handles ticket archive notification to assignee.</remarks>
        public Task TicketArchivedAsync(int ticketId, UserInfo user);
    }
}
