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
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTickets([FromQuery] TicketsFilter filter =TicketsFilter.Open)
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
        /// <remarks>Returns detailed information about a specific ticket. Ensure the user is
        /// authenticated and authorized to access ticket data before calling this method. Returns a 404
        /// status code if no tickets are found.</remarks>

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
    }


}
