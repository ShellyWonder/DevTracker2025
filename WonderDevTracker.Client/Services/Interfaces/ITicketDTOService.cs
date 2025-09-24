using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface ITicketDTOService
    {
        #region GET METHODS
        /// <summary>
        /// Get Open Tickets
        /// </summary>
        /// <remarks>
        /// Get a list of open tickets for the specified user belonging to that user's company.
        /// </remarks>
        /// <param name="UserInfo">"The current user's claims"</param>
        public Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user);

        /// <summary>
        /// Get Resolved Tickets
        /// </summary>
        /// <remarks>
        /// Get a list of resolved tickets for the specified user's company.
        /// </remarks>
        /// <param name="UserInfo">The current user's claims</param>
        Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo user);

        /// <summary>
        /// Get Archived Tickets
        /// </summary>
        /// <remarks>
        /// Get a list of Archived tickets for the specified user's company.
        /// </remarks>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Get Tickets Assigned to User
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        /// <remarks>Get all the tickets currently assigned to current user in a specific company.
        /// If the user is a Project Manager this query returns all tickets in assigned project(s).
        /// Admins will see tickets they submitted.
        /// </remarks>
        Task<IEnumerable<TicketDTO>> GetTicketsAssignedToUserAsync(UserInfo userInfo);
        #endregion

        #region CREATE METHODS
        /// <summary>
        /// Add Ticket
        /// </summary>
        /// <remarks>
        /// Creates a new project ticket in the database asynchronously.
        /// </remarks>
        /// <param name="ticket">New ticket to be saved in Db</param>
        /// <param name="userInfo">Current user's claims</param>
        /// <returns>New ticket after being saved in Db</returns>
        Task<TicketDTO?> AddTicketAsync(TicketDTO ticket, UserInfo userInfo);
        #endregion
    }
}
