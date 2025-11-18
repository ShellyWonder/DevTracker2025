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

        /// <summary>
        /// Get Company
        /// </summary>
        /// <param name="userInfo">The Current user's claims.</param>
        /// <remarks>Asynchronously retrieves the company information associated with the specified user.</remarks>
        public Task <CompanyDTO> GetCompanyAsync(UserInfo userInfo);
        /// <summary>
        /// Update Company (Name, Logo Image and Description)
        /// </summary>
        /// <param name="company">An object containing the updated company information. Cannot be null.</param>
        /// <param name="userInfo">Current user's claims.</param>
        /// <remarks>Updates the details of an existing company using the provided data transfer object. User must be company Admin.</remarks>
        public Task UpdateCompanyAsync(CompanyDTO company, UserInfo userInfo);
    }
}
