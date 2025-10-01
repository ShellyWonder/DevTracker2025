using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class TicketDTOService(ITicketRepository ticketRepository) : ITicketDTOService
    {
        #region CREATE METHODS
        public async Task<TicketDTO?> AddTicketAsync(TicketDTO ticket, UserInfo userInfo)
        {
            Ticket dbTicket = new()
            {
                Title = ticket.Title,
                Description = ticket.Description,
                ProjectId = ticket.ProjectId,
                Created = DateTimeOffset.UtcNow,
                Status = TicketStatus.New,
                Priority = ticket.Priority,
                Type = ticket.Type,
                SubmitterUserId = userInfo.UserId,
                DeveloperUserId = ticket.DeveloperUserId
            };
            dbTicket = await ticketRepository.AddTicketAsync(dbTicket, userInfo)
                ?? throw new InvalidOperationException("Ticket creation failed."); 
            return dbTicket.ToDTO();
        }
        #endregion

        #region GET METHODS

        public async Task<TicketDTO?> GetTicketByIdAsync(int ticketId, UserInfo userInfo)
        {
           Ticket ticket = await ticketRepository.GetTicketByIdAsync(ticketId, userInfo)
                ?? throw new KeyNotFoundException("Ticket not found or access denied.");
              return ticket.ToDTO();
        }
        public async Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo)
        {
            IEnumerable<Ticket> tickets = await ticketRepository.GetArchivedTicketsAsync(userInfo);
            IEnumerable<TicketDTO> dtos = tickets.Select(t => t.ToDTO());
            return dtos;
        }

        public async Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user)
        {
            IEnumerable<Ticket> tickets = await ticketRepository.GetOpenTicketsAsync(user);
            IEnumerable<TicketDTO> dtos = tickets.Select(t => t.ToDTO());
            return dtos;
        }

        public async Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo user)
        {
            IEnumerable<Ticket> tickets = await ticketRepository.GetResolvedTicketsAsync(user);
            IEnumerable<TicketDTO> dtos = tickets.Select(t => t.ToDTO());
            return dtos;
        }

        

        public async Task<IEnumerable<TicketDTO>> GetTicketsAssignedToUserAsync(UserInfo userInfo)
        {
            IEnumerable<Ticket> tickets = await ticketRepository.GetTicketsAssignedToUserAsync(userInfo);
            IEnumerable<TicketDTO> dtos = tickets.Select(t => t.ToDTO());
            return dtos;
        }
        #endregion


    }
}
