using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class CompanyDTOService(ICompanyRepository repository) : ICompanyDTOService
    {
        public async Task<IEnumerable<AppUserDTO>> GetUsersAsync(UserInfo userInfo)
        {
            IEnumerable<ApplicationUser> users = await repository.GetUsersAsync(userInfo);
            var dtos = users.Select(u => u.ToDTO());
            return dtos;
        }
        public Task<IEnumerable<AppUserDTO>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}
