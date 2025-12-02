using System.Net.Http.Json;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMInviteDTOService(HttpClient http) : IInviteDTOService
    {

        public async Task<InviteDTO> CreateInviteAsync(InviteDTO invite, UserInfo user)
        {
            var response = await http.PostAsJsonAsync("api/invites", invite);
            InviteDTO createdInvite = await response.Content.ReadFromJsonAsync<InviteDTO>()
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse);
            return createdInvite;
        }


        public Task<bool> SendInviteAsync(Uri baseUri, int inviteId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InviteDTO>> GetInviteAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task CancelInviteAsync(int inviteId, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
