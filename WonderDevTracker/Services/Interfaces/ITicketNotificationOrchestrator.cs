using WonderDevTracker.Client;

namespace WonderDevTracker.Services.Interfaces
{
    public interface ITicketNotificationOrchestrator
    {
        /// <summary>
        /// Ticket Created Notification
        /// </summary>
        /// <param name="ticketId">Ticket id.</param>
        /// <param name="user">Current user's claims Cannot be null.</param>
        /// <remarks>Handles notification when a new ticket is created </remarks>
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
        /// Ticket Unassignment Notification
        /// </summary>
        /// <param name="ticketId">Ticket id.</param>
        /// <param name="previousDevId">The user ID of the developer from whom the ticket was unassigned. Cannot be null or empty.</param>
        /// <param name="actor">Current user who performed the unassignment action. Cannot be null.</param>
        /// <remarks>Handles notification when a ticket is unassigned from a developer.</remarks>
        public Task TicketUnassignedAsync(int ticketId, string previousDevId, UserInfo actor);

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

        /// <summary>
        /// Ticket Restored Notification
        /// </summary>
        /// <param name="ticketId">Ticket id. Must correspond to an existing archived ticket.</param>
        /// <param name="user">Information about the user performing the restore operation. Cannot be null.</param>
        /// <remarks>Restores a previously archived ticket with the specified identifier on behalf of the given user.</remarks>
        public Task TicketRestoredAsync(int ticketId, UserInfo user);
    }
}
