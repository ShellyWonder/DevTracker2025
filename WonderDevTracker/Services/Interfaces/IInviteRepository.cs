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
    }
}
