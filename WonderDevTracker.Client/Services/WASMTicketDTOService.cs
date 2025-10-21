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

        public async Task ArchiveTicketAsync(int ticketId, UserInfo user)
        {
           var response = await http.PatchAsync($"api/Tickets/{ticketId}/archive", null);//null body
              response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo)
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

        public async Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo user)
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

        public async Task<TicketDTO?> GetTicketByIdAsync(int ticketId, UserInfo userInfo)
        {
            try
            {
                TicketDTO? ticket = await http.GetFromJsonAsync<TicketDTO>($"api/Tickets/{ticketId}");
                return ticket;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return null;
            }
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

        public async Task RestoreTicketByIdAsync(int ticketId, UserInfo user)
        {
            var response = await http.PatchAsync($"api/Tickets/{ticketId}/restore", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateTicketAsync(TicketDTO ticket, UserInfo user)
        {
            var response = await http.PutAsJsonAsync($"api/Tickets/{ticket.Id}", ticket);
            response.EnsureSuccessStatusCode();
        }
    }
}
