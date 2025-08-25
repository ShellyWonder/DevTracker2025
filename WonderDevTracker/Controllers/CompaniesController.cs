using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Controllers
{

    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompaniesController(ICompanyDTOService CompanyService) : ControllerBase
    {
        //Check if user is authenticated
        UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

        #region GET USERS IN COMPANY

        /// <summary>
        /// Get Users in Company
        /// </summary>
        /// <remarks>This method returns all users associated with the current user's company. Ensure the user is
        /// authenticated and authorized to access user data before calling this method.</remarks>
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetUsersInCompany()
        {
            var users = await CompanyService.GetUsersAsync(UserInfo);
            if (users == null || !users.Any()) return NotFound();
            return Ok(users);
        }
        #endregion

        #region GET USERS IN ROLE
        /// <summary>
        /// Get Users in Role
        /// </summary>
        /// <param name="role">The role of the users to retrieve (e.g., Admin, ProjectManager, Developer, Submitter).</param>
        /// <remarks>This method returns all users associated with the current user's company who have the specified role.
        /// Ensure the user is authenticated and authorized to access user data before calling this method. Returns a 404
        /// status code if no users are found.</remarks>
        [HttpGet("users/{role}")]
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetUsersInRole([FromRoute] Role role)
        {
            var users = await CompanyService.GetUsersInRoleAsync(role, UserInfo);
            if (users == null || !users.Any()) return NotFound();
            return Ok(users);
        }
        #endregion
    }
}
