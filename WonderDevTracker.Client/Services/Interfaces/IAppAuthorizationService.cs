using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface IAppAuthorizationService
    {
        /// <summary>
        /// Determines whether the specified user has administrative privileges.
        /// </summary>
        /// <param name="user">The user information to evaluate. Cannot be null.</param>
        /// <returns>true if the user has administrative privileges; otherwise, false.</returns>
        public bool UserIsAdmin(UserInfo user);

        /// <summary>
        /// Is the user an Admin or the Project Manager of the specified project?
        /// </summary>
        ///  <remarks> This method checks if the user has an Admin role or is the Project Manager for the given project ID.
        ///  It is the most common authorization/permission pattern in the app</remarks>
        /// <param name="projectId">Project Id</param>
        /// <param name="user">Current user's claims</param>
        public Task<bool> IsUserAdminPMAsync(int projectId, UserInfo user);

        /// <summary>
        /// Can the user edit the specified ticket?
        /// </summary>
        /// <remarks>Determines whether a user belongs to one of these roles:
        /// 1. Admin
        /// 2.Project Manager assigned to the project
        /// 3. Submitter of the specific ticket
        /// 4. Developer assigned to the specific ticket</remarks>
        /// <param name="ticket">ticket to be edited(updated)</param>
        /// <param name="user">Current user's claims</param>
        public Task<bool> CanEditTicketAsync(TicketDTO ticket, UserInfo user);


    }
}
