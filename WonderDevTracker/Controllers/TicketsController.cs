using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Helpers;

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
        /// <remarks>Updates the details of an existing ticket with the specified identifier. 
        /// 
        /// Returns: 
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


        /// <summary>
        /// Update Ticket Comment
        /// </summary>
        /// <param name="ticketId">Id of the ticket</param>
        /// <param name="commentId">Id of the comment</param>
        /// <param name="comment">updated content passed from the body</param>
        /// <remarks>Updates a ticket comment by owner of the comment.</remarks>
        [HttpPut("{ticketId:int}/comments/{commentId:int}"), Tags("Comments")]
        public async Task<IActionResult> UpdateComment([FromRoute] int ticketId,
                                                       [FromRoute] int commentId,
                                                       [FromBody] TicketCommentDTO comment)
        {
            if (commentId != comment.Id || ticketId != comment.TicketId)
                return BadRequest("Ticket ID or Comment ID mismatch.");
            if (comment == null) return NotFound();
            await ticketService.UpdateCommentAsync(comment, UserInfo);
            return NoContent();
        }
        #endregion

        #region ARCHIVE/RESTORE TICKET
        /// <summary>
        /// Archive Ticket
        /// </summary>
        /// <param name="ticketId">Id of a ticket.</param>
        /// <remarks>
        ///Archives an individual active ticket if user is authorized.
        ///Functions as a soft delete.</remarks>

        [HttpPatch("{ticketId:int}/archive")]
        public async Task<IActionResult> ArchiveTicket([FromRoute] int ticketId)
        {
            await ticketService.ArchiveTicketAsync(ticketId, UserInfo);
            return NoContent();
        }
        /// <summary>
        /// Restore Ticket
        /// </summary>
        /// <param name="ticketId">Id of a ticket.</param>
        /// <remarks>Restores an individual archived ticket if user is authorized. </remarks>

        [HttpPatch("{ticketId:int}/restore")]
        public async Task<IActionResult> RestoreTicket([FromRoute] int ticketId)
        {
            await ticketService.RestoreTicketByIdAsync(ticketId, UserInfo);
            return NoContent();
        }
        #endregion

        #region ADD TICKET/ADD NEW COMMENT/ ADD TICKET ATTACHMENT
        /// <summary>
        /// Create Ticket
        /// </summary>
        /// <remarks>Creates a new project ticket in the database. 
        /// Authorized user roles in their respective companies can create(submit) a new ticket:
        /// 'Admin' 
        /// 'ProjectManager' 
        /// 'Developer'
        /// 'Submitter'
        /// </remarks>
        /// <param name="ticket">Ticket to be created.</param>

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Admin)}, " +
                           $"{nameof(Role.ProjectManager)}, {nameof(Role.Developer)}," +
                           $"{nameof(Role.Submitter)}")]
        public async Task<ActionResult<TicketDTO>> AddTicket([FromBody] TicketDTO ticket)
        {
            try
            {
                var createdTicket = await ticketService.AddTicketAsync(ticket, UserInfo);
                return CreatedAtAction(nameof(GetTicketById), new { ticketId = createdTicket!.Id }, createdTicket);

            }
            catch (ApplicationException invalidProjectException)
            {

                Console.WriteLine(invalidProjectException);
                return BadRequest(invalidProjectException.Message);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return Problem();
            }
        }

        /// <summary>
        /// Add Ticket Comment
        /// </summary>
        /// <remarks>Returns a 400 Bad Request response if the ticketId does not match the TicketId in the
        /// comment. Returns a 403 Forbidden response if the ticket is invalid. Returns a generic error response for
        /// unexpected failures.</remarks>
        /// <param name="ticketId">The unique id of the ticket to which the comment will be added. Must match the TicketId property of
        /// the comment.</param>
        /// <param name="comment">The comment details to add to the ticket. Must contain a valid TicketId that matches the ticketId parameter.</param>
        /// <returns>An ActionResult containing the created TicketCommentDTO if the operation succeeds; otherwise, a result
        /// indicating the error condition.</returns>

        [HttpPost("{ticketId:int}/comments"), Tags("Comments")]
        public async Task<ActionResult<TicketCommentDTO>> AddTicketComment([FromRoute] int ticketId,
                                                                            [FromBody] TicketCommentDTO comment)
        {
            if (ticketId != comment.TicketId) return BadRequest("Ticket ID mismatch.");

            try
            {
                var createdComment = await ticketService.CreateCommentAsync(comment, UserInfo);
                return Ok(createdComment);

            }
            catch (ApplicationException invalidTicketExceptiom)
            {

                Console.WriteLine(invalidTicketExceptiom);
                return Forbid(); //403
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }

        }
        /// <summary>
        /// Add Ticket Attachment
        /// </summary>
        /// <remarks>  Uploads a new file attachment to the specified ticket. The file and attachment metadata must be submitted as multipart/form-data. The method
        /// returns a bad request if the ticket ID does not match or if the file size exceeds the allowed
        /// limit. Uploader must be one of the following:
        ///
        /// 1. Ticket-assigned developer or Submitter OR company Admin
        /// 2. Ticket-assigned submitter 
        /// 3. company Admin
        /// </remarks>
        /// <param name="ticketId">Ticket id. Must match the TicketId in the
        /// attachment data.</param>
        /// <param name="file">The file to attach to the ticket. The file must not exceed the maximum allowed file size.</param>
        /// <param name="attachment">The metadata for the ticket attachment, including details such as TicketId and additional attachment
        /// information. Must be submitted as form data.</param>

        [HttpPost("{ticketId:int}/attachments"), Tags("Ticket Attachments")]
        public async Task<IActionResult> AddTicketAttachment([FromRoute] int ticketId,
                                                     IFormFile file,
                                                     [FromForm] TicketAttachmentDTO attachment)//form encoded:from form-data not JSON body
        {
            if (ticketId != attachment.TicketId || file.Length > BrowserFileHelper.MaxFileSize) return BadRequest("Ticket ID mismatch or file size exceeds limit.");
            try
            {
                attachment.UserId = UserInfo.UserId;
                attachment.Created = DateTimeOffset.Now;
                attachment.FileName = file.FileName;

                var upload = await UploadHelper.GetFileUploadAsync(file);

                TicketAttachmentDTO createdAttachment = await ticketService.AddTicketAttachmentAsync(attachment, upload.Data!, upload.Type!, UserInfo);
                return Ok(createdAttachment);
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException);
                return BadRequest(ioException.Message);
            }
            catch (ApplicationException invalidTicketException)
            {
                Console.WriteLine(invalidTicketException);
                return BadRequest(invalidTicketException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }
        #endregion

        #region DELETE TICKET COMMENT/ DELETE TICKET ATTACHMENT
        /// <summary>
        /// Delete Ticket Comment
        /// </summary>
        /// <param name="commentId">Id of a ticket comment</param>
        /// <remarks>Deletes an existing comment on an active ticket. Only ticket owner(author) or company admin may delete a comment.</remarks>
        [HttpDelete("comments/{commentId:int}"), Tags("Comments")]
        public async Task<IActionResult> DeleteTicketComment([FromRoute] int commentId)
        {
            try
            {
                await ticketService.DeleteCommentAsync(commentId, UserInfo);
                return NoContent();
            }
            catch (ApplicationException invalidTicketException)
            {
                Console.WriteLine(invalidTicketException);
                return Forbid(); //403
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        /// <summary>
        /// Delete Ticket Attachment
        /// </summary>
        /// <remarks>Deletes an existing attachment from a ticket. Only the uploader of the attachment or a company admin may delete the attachment.</remarks>
        /// <param name="attachmentId">The unique identifier of the attachment to delete. Must correspond to an existing ticket attachment.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="NoContentResult"/>
        /// if the deletion is successful; <see cref="ForbidResult"/> if the operation is not permitted; or a generic
        /// error result for other failures.</returns>
        [HttpDelete("attachments/{attachmentId:int}"), Tags("Attachments")]
        public async Task<IActionResult> DeleteTicketAttachment([FromRoute] int attachmentId)
        {
            try
            {
                await ticketService.DeleteTicketAttachmentAsync(attachmentId, UserInfo);
                return NoContent();
            }
            catch (ApplicationException invalidTicketException)
            {
                Console.WriteLine(invalidTicketException);
                return Forbid(); //403
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }
        #endregion

    }

}




