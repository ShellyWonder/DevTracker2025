using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvitesController(IInviteDTOService InviteService) : ControllerBase
    {
        private UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

        /// <summary>
        /// Get Invites
        /// </summary>
        /// <remarks>An <see cref="ActionResult{T}"/> containing a collection of <see cref="InviteDTO"/> objects for the current
        /// user. Returns an empty collection if no invites are found.
        /// Retrieves all invites associated with the current user.</remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InviteDTO>>> GetInvites()
        {
            IEnumerable<InviteDTO> invites = await InviteService.GetInviteAsync(UserInfo);
            return Ok(invites);
        }
        /// <summary>
        ///Create Invitation 
        /// </summary>
        /// <remarks> Creates a new invite using the specified invite details.
        /// This action requires the caller to have the Admin role. The invite details provided
        /// in the request body are validated before creation.
        /// 
        /// **Note: This endpoint only creates the endpoint. It does not send the invitation.**
        /// 
        /// **To send the email, use the "Send Invite" endpoint.**
        /// </remarks>
        /// <param name="invite">An <see cref="InviteDTO"/> object containing the details of the invite to create. Must not be null.</param>
        /// <returns>An <see cref="ActionResult{InviteDTO}"/> containing the created invite if successful; otherwise, a result
        /// indicating the error.</returns>
        [HttpPost]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<ActionResult<InviteDTO>> CreateInvite([FromBody] InviteDTO invite)
        {
            try
            {
                InviteDTO createdInvite = await InviteService.CreateInviteAsync(invite, UserInfo);
                return Ok(createdInvite);
            }
            catch (ApplicationException validationEx)
            {
                //user error
                Console.WriteLine(validationEx.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                //App error
                Console.WriteLine(ex);
                return Problem();
            }
        }

        /// <summary>
        /// Send Invite
        /// </summary>
        /// <remarks>Sends a previously created invitation associated with the specified invite identifier.
        /// Invitors are restricted to users with the Admin role. No data is persisted to the
        /// database by this operation.</remarks>
        /// <param name="inviteId">The unique identifier of the invitation to send. Must correspond to an existing invitation.</param>
        /// <returns>A 204 No Content response if the invitation was sent successfully; otherwise, a 400 Bad Request response if
        /// the invitation could not be sent.</returns>
        [HttpPost("{inviteId:int}/send")]//POST: api/invites/24/send
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> SendInvite([FromRoute] int inviteId)
        {
            Uri requestUri = new Uri(Request.GetEncodedUrl());
            Uri baseUri = new Uri(requestUri.GetLeftPart(UriPartial.Authority));
            bool sent = await InviteService.SendInviteAsync(baseUri, inviteId, UserInfo);
            return sent ? NoContent() : BadRequest();
        }

        /// <summary>
        /// Cancel Invite
        /// </summary>
        /// <remarks>Cancels a pending invite identified by the specified invite ID.Used as a "soft"
        /// delete where the record remains in the db.
        /// Only users with the Admin role are authorized to perform this operation. If the
        /// invite does not exist or has already been processed, no action is taken.</remarks>
        /// <param name="inviteId">The unique identifier of the invite to cancel.
        /// Must correspond to an existing pending invite.</param>
        
        [HttpDelete("{inviteId:int}")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> CancelInvite([FromRoute] int inviteId)
        {
            await InviteService.CancelInviteAsync(inviteId, UserInfo);
            return NoContent();
        }

    }
}
