using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController(ITicketDTOService ticketService) : ControllerBase
    {
        // Check if the user is authenticated and authorized to access ticket data; User accesses UserClaimsPrincipal
        private UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

        #region GET TICKETS
        /// <summary>
        /// Get Tickets
        /// </summary>
        /// <remarks>Get A list of tickets(per filter) associated with the current user's company; 
        /// User is authenticated/authorized to access ticket data before calling this method.</remarks>
        ///  <param name="filter">Optional filter to specify which tickets to retrieve: **Open (default)**, Archived, Resolved or Assigned.</param>
        [HttpGet]
        //Example: api/Tickets?fiter=assigned
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTickets([FromQuery] TicketsFilter filter = TicketsFilter.Open)
        {
            var tickets = filter switch
            {
                TicketsFilter.Archived => await ticketService.GetArchivedTicketsAsync(UserInfo),
                TicketsFilter.Assigned => await ticketService.GetTicketsAssignedToUserAsync(UserInfo),
                TicketsFilter.Resolved => await ticketService.GetResolvedTicketsAsync(UserInfo),
                //default expression ~ no filter provided
                _ => await ticketService.GetOpenTicketsAsync(UserInfo)

            };
            return Ok(tickets);
        }
        #endregion
        #region GET TICKET BY ID
        /// <summary>
        /// Get Ticket by Id
        /// </summary>
        /// <param name="ticketId">The ID of the ticket to retrieve.</param>
        /// <remarks>Returns detailed information about a specific ticket. User is
        /// authenticated/authorized to access ticket data before calling this method. Returns a 404
        /// status code if no ticket is found.</remarks>

        //Route parameter must match blazor parameter 
        [HttpGet("{ticketId:int}")]
        public async Task<ActionResult<TicketDTO>> GetTicketById([FromRoute] int ticketId)
        {
            var ticket = await ticketService.GetTicketByIdAsync(ticketId, UserInfo);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }
        #endregion

        #region UPDATE TICKET
        /// <summary>
        /// Update Ticket
        /// </summary>
        /// <param name="ticketId">The unique identifier of the ticket to update. Must match the ID of the ticket provided in the request body.</param>
        /// <param name="ticket">The updated ticket information. The ticket's Id property must match the value of ticketId.</param>
        /// <remarks>Updates the details of an existing ticket with the specified identifier. Returns: 
        /// 
        /// NoContent if the update is successful;
        /// 
        /// BadRequest if the ticket ID does not match;
        /// 
        ///NotFound if the ticket is null.</remarks>
        [HttpPut("{ticketId:int}")]
        public async Task<IActionResult> UpdateTicket([FromRoute] int ticketId, [FromBody] TicketDTO ticket)
        {
            if (ticketId != ticket.Id) return BadRequest("Ticket ID mismatch.");
            if (ticket == null) return NotFound();
            
            await ticketService.UpdateTicketAsync(ticket, UserInfo);
            return NoContent();
        }
        #endregion
    }


}
