using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Services.Interfaces
{
    public interface ITicketRepository
    {
        #region GET METHODS
        public Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user);

        #endregion
    }
}
