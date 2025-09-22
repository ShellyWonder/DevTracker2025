using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMTicketDTOService : ITicketDTOService
    {
        public Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
