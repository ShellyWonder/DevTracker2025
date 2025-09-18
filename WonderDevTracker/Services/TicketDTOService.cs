using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class TicketDTOService : ITicketDTOService
    {
        public Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
