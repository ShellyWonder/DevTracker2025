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
    }
}
