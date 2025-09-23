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
        /// <param name="UserInfo">"The current user's claims"</param>
        Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo user);

        /// <summary>
        /// Get Archived Tickets
        /// </summary>
        /// <remarks>
        /// Get a list of Archived tickets for the specified user's company.
        /// </remarks>
        /// <param name="userInfo">"The current user's claims"</param>
        Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo);
        #endregion
    }
}
