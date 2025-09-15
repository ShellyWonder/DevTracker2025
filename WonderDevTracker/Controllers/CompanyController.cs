using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyController(ICompanyDTOService CompanyService) : ControllerBase
    {
        //Check if user is authenticated
        private UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

        #region GET USERS IN COMPANY

        /// <summary>
        /// Get Users in Company
        /// </summary>
        /// <param name="role">Optional role filter to retrieve users with a specific role 
        /// (e.g., Admin, ProjectManager, Developer, Submitter).</param>
        /// <remarks>This method returns all users associated with the current user's company. 
        /// If a role is specified, only users in that role are returned.
        ///  Ensure the user is authenticated and authorized to access
        ///  user data before calling this method.</remarks>

        [HttpGet("users")]//api/company/users?role=Admin
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetUsersInCompany([FromQuery] Role? role)
        {
            if (role.HasValue)
            {
                var usersInRole = await CompanyService.GetUsersInRoleAsync(role!.Value, UserInfo);
                return Ok(usersInRole);

            }
            else
            {
                var users = await CompanyService.GetUsersAsync(UserInfo);

                return Ok(users);
            }

        }
        #endregion

       
    }
}
