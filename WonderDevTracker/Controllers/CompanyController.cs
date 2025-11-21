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

        #region GET COMPANY                     
        /// <summary>
        /// Get Company Information
        /// </summary>
        /// <remarks>Retrieves information about the current company associated with the authenticated user 
        /// including name, logo, description, members and invitations. 
        /// An <see cref="ActionResult{T}"/> containing a <see cref="CompanyDTO"/> with the company's details.
        /// Returns a 200 OK response with the company information if found.</remarks>
        [HttpGet]
        public async Task<ActionResult<CompanyDTO>> GetCompanyInfo()
        {
            var company = await CompanyService.GetCompanyAsync(UserInfo);
            return Ok(company);
        }
        #endregion

        /// <summary>
        /// Update Company
        /// </summary>
        /// <remarks>Updates existing company's details including name, description and logo image.
        /// This action requires the caller to have the Admin role. The company to update is
        /// identified by the information provided in the <paramref name="company"/> parameter.</remarks>
        /// <param name="company">An object containing the updated company information. Must not be null.</param>
         [HttpPut,Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> UpdateCompany([FromBody] CompanyDTO company)
        {
            await CompanyService.UpdateCompanyAsync(company, UserInfo);
            return NoContent();
        }

        /// <summary>
        /// Assign User Role
        /// </summary>
        /// <remarks>Removes a specified user from their previous role and assigns a new role. 
        /// This action is restricted to company's Admin role. The user ID in the route must
        /// match the ID in the request body, and the role must be specified.
        /// Returns 204 No Content if the role assignment is successful; 
        /// otherwise, returns 400 Bad Request if the input is invalid.
        /// Note: Admins cannot change their own role.</remarks>
        /// <param name="id">Current user id. This value must match the Id property of
        /// <paramref name="user"/>User id and role(current user claims)</param>
        /// <param name="user">An <see cref="AppUserDTO"/> object containing the user's updated role information. The <c>Role</c> property
        /// must not be null.</param>
        [HttpPost("users/{id}"), Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> AssignUserRole([FromRoute]string id, [FromBody] AppUserDTO user)
        {
            if (user.Id != id || user.Role is null) return BadRequest();
            await CompanyService.AssignUserRoleAsync(id, user.Role.Value, UserInfo);
            return NoContent();
        }

    }
}
