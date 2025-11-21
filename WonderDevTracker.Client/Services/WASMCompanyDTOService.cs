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

        public async Task UpdateCompanyAsync(CompanyDTO company, UserInfo userInfo)
        {
            var response = await http.PutAsJsonAsync("api/company", company);
            response.EnsureSuccessStatusCode();
        }

        public async Task<CompanyDTO> GetCompanyAsync(UserInfo userInfo)
        {
            CompanyDTO? company = await http.GetFromJsonAsync<CompanyDTO>("api/company")
                                  ?? throw new HttpIOException(HttpRequestError.InvalidResponse);
            return company;
        }

        public async Task AssignUserRoleAsync(string userId, Role newRole, UserInfo userInfo)
        {
            AppUserDTO userRoleUpdate = new()
            {
                Id = userId,
                FirstName = "firstName",
                LastName = "lastName",
                Role = newRole
            };
            var response = await http.PutAsJsonAsync("api/company/users/{userId}", userRoleUpdate);
            response.EnsureSuccessStatusCode();
        }
    }
}
