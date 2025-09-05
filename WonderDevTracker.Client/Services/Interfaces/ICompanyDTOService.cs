using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface ICompanyDTOService
    {
        /// <summary>
        /// Gets all users in a company based on the provided UserInfo.
        /// </summary>
        /// <param name="userInfo">Curent user's claims</param>
        public Task<IEnumerable<AppUserDTO>> GetUsersAsync(UserInfo userInfo);

        /// <summary>
        /// Gets all users in a company based on the provided UserInfo.
        /// </summary>
        /// <param name="role">Role assigned to the user</param>
        /// <param name="userInfo">Current user's claims</param>
        public Task<IReadOnlyList<AppUserDTO>> GetUsersInRoleAsync(Role role, UserInfo userInfo);
    }
}
