using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Interfaces
{
    public interface ITicketRepository
    {
        #region GET METHODS
        /// <summary>
        /// Get Ticket By Id
        /// </summary>
        /// <remarks>Asynchronously retrieves a ticket by its unique identifier for the specified user.</remarks>
        /// <param name="ticketId">The unique identifier of the ticket to retrieve. </param>
        /// <param name="userInfo">Current user's claims</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a TicketDTO representing the
        /// ticket if found and accessible to the user; otherwise, null.</returns>
        public Task<Ticket?> GetTicketByIdAsync(int ticketId, UserInfo userInfo);

        /// <summary>
        /// Get Open Tickets
        /// </summary>
        /// <remarks>
        /// Get a list of open tickets for the specified user belonging to that user's company.
        /// </remarks>
        /// <param name="userInfo">"The current user's claims"</param>
        public Task<IEnumerable<Ticket>> GetOpenTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Get Resolved Tickets
        /// </summary>
        /// <remarks>
        /// Get a list of resolved tickets for the specified user's company.
        /// </remarks>
        /// <param name="userInfo">"The current user's claims"</param>
        Task<IEnumerable<Ticket>> GetResolvedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Get Archived Tickets
        /// </summary>
        /// <remarks>
        /// Get a list of Archived tickets for the specified user's company.
        /// </remarks>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<Ticket>> GetArchivedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Get Tickets Assigned to User
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        /// <remarks>Get all the tickets currently assigned to current user in a specific company.
        /// If the user is a Project Manager this query returns all tickets in assigned project(s).
        /// Admins will see tickets they submitted.
        /// </remarks>
        Task<IEnumerable<Ticket>> GetTicketsAssignedToUserAsync(UserInfo userInfo);
        #endregion

        #region CREATE METHODS
        /// <summary>
        /// Add Ticket
        /// </summary>
        /// <remarks>
        /// Creates a new project ticket in the database asynchronously.
        /// </remarks>
        /// <param name="ticket">New project ticket to be saved in Db</param>
        /// <param name="userInfo">Current user's claims</param>
        /// <returns>New ticket after being saved in Db</returns>
        Task<Ticket?> AddTicketAsync(Ticket ticket, UserInfo userInfo);
        #endregion
    }
}
