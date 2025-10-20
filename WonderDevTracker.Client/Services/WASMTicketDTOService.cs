using System.Net.Http.Json;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMTicketDTOService(HttpClient http) : ITicketDTOService
    {
        public Task<TicketDTO?> AddTicketAsync(TicketDTO ticket, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task ArchiveTicketAsync(int ticketId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public  async Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo)
        {
            try
            {
                List<TicketDTO>? tickets = await http.GetFromJsonAsync<List<TicketDTO>>($"api/Tickets?filter={TicketsFilter.Archived}")
                    ?? [];
                return tickets;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user)
        {
            try
            {
                List<TicketDTO>? tickets = await http.GetFromJsonAsync<List<TicketDTO>>("api/Tickets")
                    ?? [];
                return tickets;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public  async Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo user)
        {
            try
            {
                List<TicketDTO>? tickets = await http.GetFromJsonAsync<List<TicketDTO>>($"api/Tickets?filter={TicketsFilter.Resolved}")
                    ?? [];
                return tickets;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public Task<TicketDTO?> GetTicketByIdAsync(int ticketId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TicketDTO>> GetTicketsAssignedToUserAsync(UserInfo userInfo)
        {
            try
            {
                List<TicketDTO>? tickets = await http.GetFromJsonAsync<List<TicketDTO>>($"api/Tickets?filter={TicketsFilter.Assigned}")
                    ?? [];
                return tickets;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public Task RestoreTicketByIdAsync(int ticketId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTicketAsync(TicketDTO ticket, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
