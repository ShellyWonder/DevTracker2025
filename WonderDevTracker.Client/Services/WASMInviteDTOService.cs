using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMInviteDTOService : IInviteDTOService
    {
        public Task CancelInviteAsync(int inviteId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<InviteDTO> CreateInviteAsync(InviteDTO invite, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InviteDTO>> GetInviteAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendInviteAsync(Uri baseUri, int inviteId, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
