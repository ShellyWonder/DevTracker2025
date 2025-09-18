using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface ITicketDTOService
    {
        #region GET METHODS
        public Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user);

        #endregion
    }
}
