using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMInviteDTOService : IInviteDTOService
    {
        public Task<InviteDTO> CreateInviteAsync(InviteDTO invite, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
