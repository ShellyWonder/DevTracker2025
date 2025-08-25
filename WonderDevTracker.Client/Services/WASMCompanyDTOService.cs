using System.Net.Http.Json;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMCompanyDTOService(HttpClient http) : ICompanyDTOService
    {
        public async Task<IEnumerable<AppUserDTO>> GetUsersAsync(UserInfo userInfo)
        {
            try
            {
                List<AppUserDTO> users = await http.GetFromJsonAsync<List<AppUserDTO>>("api/companies") ?? [];
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public Task<IEnumerable<AppUserDTO>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}
