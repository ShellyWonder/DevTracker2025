using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class AppAuthorizationService(IProjectDTOService projectService) : IAppAuthorizationService
    {
        /// <summary>
        /// Is the user an Admin or the Project Manager of the specified project?
        /// </summary>
        ///  <remarks> This method checks if the user has an Admin role or is the Project Manager for the given project ID.
        ///  It is the most common authorization/permission pattern in the app</remarks>
        /// <param name="projectId">Project Id</param>
        /// <param name="user">Current user's claims</param>
        
        public async Task<bool> IsUserAdminPMAsync(int projectId, UserInfo user)
        {
            
                if (user.IsInRole(Role.Admin)) return true;

                var pm = await projectService.GetProjectManagerAsync(projectId, user);
            if (pm is null) return false;
            return pm?.Id == user.UserId;
            
        }


    }
}
