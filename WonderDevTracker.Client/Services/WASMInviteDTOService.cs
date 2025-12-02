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


        public async Task<bool> SendInviteAsync(Uri baseUri, int inviteId, UserInfo user)
        {
            try
            {
                var response = await http.PostAsync($"api/invites/{inviteId}/send", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return false;
            }
            
        }

        public Task<IEnumerable<InviteDTO>> GetInviteAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task CancelInviteAsync(int inviteId, UserInfo user)
        {
            var response = await http.DeleteAsync($"api/invites/{inviteId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
