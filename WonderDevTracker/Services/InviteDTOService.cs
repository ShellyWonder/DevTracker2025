using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class InviteDTOService(IInviteRepository repository) : IInviteDTOService
    {
        public async Task<InviteDTO> CreateInviteAsync(InviteDTO dto, UserInfo user)
        {
            Invite invite = new()
            {
                InviteDate = DateTimeOffset.UtcNow,
                InviteeEmail = dto.InviteeEmail,
                InviteeFirstName = dto.InviteeFirstName,
                InviteeLastName = dto.InviteeLastName,
                Message = dto.Message,
                CompanyId = user.CompanyId,
                ProjectId = dto.ProjectId!.Value,
                InvitorId = user.UserId

            };
            Invite createdInvite = await repository.CreateInviteAsync(invite, user);
            return createdInvite.ToDTO();
        }
    }
}
