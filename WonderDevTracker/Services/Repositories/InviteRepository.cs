using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class InviteRepository : IInviteRepository
    {
        public Task<Invite> CreateInviteAsync(Invite invite, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
