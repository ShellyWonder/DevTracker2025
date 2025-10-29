using Microsoft.AspNetCore.Identity;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;
using WonderDevTracker.Services.Repositories;

namespace WonderDevTracker.Services
{
    public class TicketDTOService(ITicketRepository ticketRepository,
                                  IProjectRepository projectRepository,
                                   UserManager<ApplicationUser> userManager) : ITicketDTOService
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
        public  async Task<TicketCommentDTO> CreateCommentAsync(TicketCommentDTO comment, UserInfo userInfo)
        {
            // convert DTO to entity
            TicketComment dbComment = new()
            {
                Content = comment.Content,
                Created = DateTimeOffset.UtcNow,
                TicketId = comment.TicketId,
                UserId = userInfo.UserId
            };
            dbComment = await ticketRepository.CreateCommentAsync(dbComment, userInfo)
                ?? throw new InvalidOperationException("Comment creation failed.");
            return dbComment.ToDTO();
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

        #region ARCHIVE/RESTORE METHODS

        public async Task RestoreTicketByIdAsync(int ticketId, UserInfo user)
        {
            await ticketRepository.RestoreTicketByIdAsync(ticketId, user);
        }

        public async Task ArchiveTicketAsync(int ticketId, UserInfo user)
        {
            await ticketRepository.ArchiveTicketAsync(ticketId, user);
        }
        #endregion

        #region UPDATE METHODS

        public async Task UpdateTicketAsync(TicketDTO ticket, UserInfo user)
        {
            Ticket dbTicket = await ticketRepository.GetTicketByIdAsync(ticket.Id, user)
                  ?? throw new KeyNotFoundException("Ticket not found or access denied.");
            if (dbTicket is null) return;

            dbTicket.Title = ticket.Title;
            dbTicket.Description = ticket.Description;
            dbTicket.Type = ticket.Type;
            dbTicket.Priority = ticket.Priority;
            dbTicket.Status = ticket.Status;
            dbTicket.Updated = DateTimeOffset.UtcNow;

            // Only handle developer changes if the value actually changed
            if (dbTicket.DeveloperUserId != ticket.DeveloperUserId)
            {
                // Permission check (PM on this project OR Admin)
                var pm = await projectRepository.GetProjectManagerAsync(dbTicket.ProjectId, user);
                var userIsPmOrAdmin = (pm?.Id == user.UserId) || user.IsInRole(Role.Admin);
                if (!userIsPmOrAdmin)
                    throw new UnauthorizedAccessException("You do not have permission to (re)assign the developer.");

                // Unassign request
                var newDevId = ticket.DeveloperUserId;
                if (string.IsNullOrWhiteSpace(newDevId))
                {
                    dbTicket.DeveloperUserId = null;
                    dbTicket.DeveloperUser = null;

                    await ticketRepository.UpdateTicketAsync(dbTicket, user);
                    return;
                }

                // Assign request: must be a project member AND in Developer role
                var members = await projectRepository.GetProjectMembersAsync(dbTicket.ProjectId, user);
                var developer = members.FirstOrDefault(m => m.Id == newDevId)
                    ?? throw new InvalidOperationException("Developer must be a member of the project.");

                if (!await userManager.IsInRoleAsync(developer, nameof(Role.Developer)))
                    throw new InvalidOperationException("Selected user is not a Developer.");

                dbTicket.DeveloperUserId = newDevId;
                dbTicket.DeveloperUser = developer;

                await ticketRepository.UpdateTicketAsync(dbTicket, user);
                return;
            }

            await ticketRepository.UpdateTicketAsync(dbTicket, user);
            return;
        }

        #endregion
    }
}
