using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class InviteDTOService(IInviteRepository repository) : IInviteDTOService
    {
        public Task<InviteDTO> CreateInviteAsync(InviteDTO invite, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
