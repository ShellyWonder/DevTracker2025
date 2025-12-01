using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Interfaces
{
    public interface IInviteRepository
    {
        ///<summary>
        ///Create Invite
        ///</summary>
        ///<remarks>Creates an invite and sends an email to the invitee.
        ///User has 7 days to response before cookie expires. 
        ///When app receives the return creditials,user is directed to the registration page where user creates account. 
        ///The new user's id is connected to the company extending the offer.</remarks>
        ///<param name="invite">The details sent from Company Admin to the invitee to join the sender's company.</param>
        ///<param name="user">The current user's claims</param>
        public Task<Invite> CreateInviteAsync(Invite invite, UserInfo user);

        /// <summary>
        /// Get Invite (Collection)
        /// </summary>
        /// <param name="user">The current user's claims.</param>
        /// <remarks>Asynchronously retrieves the collection of invites associated 
        /// with the specified identifier and user. 
        /// The task result contains a collection of <see cref="InviteDTO"/>
        /// objects associated with the specified identifier and user. 
        /// The collection is empty if no invites are found.</remarks>
        public Task<IEnumerable<Invite>> GetInviteAsync(UserInfo user);

        /// <summary>
        /// Cancel Invite
        /// </summary>
        /// <param name="inviteId">Id of the invite to cancel. Must correspond to an existing, pending invite.</param>
        /// <param name="user">The user's claims.</param>
        /// <remarks>Cancels a pending invite identified by the specified invite ID on behalf of the given user. 
        /// Only the invite's company admin may cancel the invite
        /// </remarks>
        public Task CancelInviteAsync(int inviteId, UserInfo user);
        /// <summary>
        /// Send Invite to New User
        /// </summary>
        /// <param name="baseUri">The base URI of the service endpoint to which the invitation request will be sent. Cannot be null.</param>
        /// <param name="inviteId">Id of the invitation to be sent.</param>
        /// <param name="user">Current user's claims.</param>
        /// <remarks>Sends an invitation to the specified user asynchronously
        /// using the provided base URI and invite identifier. 
        /// The task result is <see langword="true"/> if the
        /// invitation was sent successfully; otherwise, <see langword="false"/>.</remarks>
        public Task<bool> SendInviteAsync(Uri baseUri, int inviteId, UserInfo user);
        /// <summary>
        /// Get Valid Invite
        /// </summary>
        /// <param name="protectedToken">A protected string representing the invite token to validate. Cannot be null or empty.</param>
        /// <param name="protectedEmail">A protected string representing the email address associated with the invite. Cannot be null or empty.</param>
        /// <param name="protectedCompanyId">A protected string representing the company identifier associated with the invite. Cannot be null or empty.</param>
        /// <remarks>Asynchronously retrieves a valid invite that matches the specified protected token, email, and company
        /// identifier.A task that represents the asynchronous operation. The task result contains the matching valid invite if
        /// found; otherwise, null.</remarks>
        public Task<Invite?> GetValidInviteAsync(string protectedToken, string protectedEmail, string protectedCompanyId);

        /// <summary>
        /// Accept Invite
        /// </summary>
        /// <param name="inviteId">The unique identifier of the invitation to accept. Must correspond to a valid, pending invitation.</param>
        /// <param name="invitee">The user who is accepting the invitation. Cannot be null.</param>
        /// <remarks>Accepts an invitation identified by the specified invite ID on behalf of the given user
        /// and invalidates invite so it may not be used again.
        /// </remarks>
        public Task AcceptInviteAsync(int inviteId, ApplicationUser invitee);
    }
}
