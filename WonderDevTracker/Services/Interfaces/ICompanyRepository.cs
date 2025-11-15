using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Interfaces
{
    public interface ICompanyRepository
    {
        /// <summary>
        /// Gets all users in a company based on the provided UserInfo.
        /// </summary>
        /// <param name="userInfo">Curent user's claims</param>
        public Task<IEnumerable<ApplicationUser>> GetUsersAsync(UserInfo userInfo);

        /// <summary>
        /// Gets all users in a company based on the provided UserInfo.
        /// </summary>
        /// <param name="role">Role assigned to the user</param>
        /// <param name="userInfo">Current user's claims</param>
        public Task<IReadOnlyList<ApplicationUser>> GetUsersInRoleAsync(Role role, UserInfo userInfo);

        /// <summary>
        /// Get Company
        /// </summary>
        /// <param name="userInfo">The Current user's claims.</param>
        /// <remarks>Asynchronously retrieves the company information associated with the specified user.</remarks>
        public Task<Company> GetCompanyAsync(UserInfo userInfo);


    }

}

