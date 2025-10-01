using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMTicketDTOService : ITicketDTOService
    {
        public Task<TicketDTO?> AddTicketAsync(TicketDTO ticket, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<TicketDTO?> GetTicketByIdAsync(int ticketId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketDTO>> GetTicketsAssignedToUserAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}
