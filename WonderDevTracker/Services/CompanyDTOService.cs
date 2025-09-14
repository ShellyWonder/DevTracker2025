using Microsoft.AspNetCore.Identity;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class CompanyDTOService(ICompanyRepository repository, 
                                   IServiceScopeFactory scopeFactory) : ICompanyDTOService
    {
        public async Task<IEnumerable<AppUserDTO>> GetUsersAsync(UserInfo userInfo)
        {
            IEnumerable<ApplicationUser> users = await repository.GetUsersAsync(userInfo);

            await using var scope = scopeFactory.CreateAsyncScope();
            var scopedUserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            List<AppUserDTO> dtos = [];

            foreach (ApplicationUser user in users)
            {
                AppUserDTO dto = await user.ToDTOWithRole(scopedUserManager);
                dtos.Add(dto);
            }

            return dtos;
        }
        public async Task<IReadOnlyList<AppUserDTO>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {
            var usersInRole =  await repository.GetUsersInRoleAsync(role, userInfo);
            List<AppUserDTO> dtos = [];

            foreach (ApplicationUser user in usersInRole)
            {
                AppUserDTO dto = user.ToDTO();
                dto.Role = role;
                dtos.Add(dto);
            }
            return dtos;
        }
    }
}
