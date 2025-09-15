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
                List<AppUserDTO> users = await http.GetFromJsonAsync<List<AppUserDTO>>("api/company/users") ?? [];
                return users ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<IReadOnlyList<AppUserDTO>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {

            try
            {
                List<AppUserDTO> users = await http.GetFromJsonAsync<List<AppUserDTO>>($"api/company/users?role={role}") ?? [];
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }
    }
}
