using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Services.Interfaces
{
    /// <summary>
    /// Get Open Tickets
    /// </summary>
    /// <remarks>
    /// Get a list of open tickets for the specified user belonging to that user's company.
    /// </remarks>
    /// <param name="UserInfo">"The current user's claims"</param>
    public interface ITicketDTOService
    {
        #region GET METHODS
        public Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user);

        #endregion
    }
}
