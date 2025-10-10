using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class AppAuthorizationService(IProjectDTOService projectService) : IAppAuthorizationService
    {
        public async Task<bool> CanEditTicketAsync(TicketDTO ticket, UserInfo user)
        {
            if (ticket is null) return false;

            // Admin or PM on the project?
            if (await IsUserAdminPMAsync(ticket.ProjectId, user)) return true;

            // Otherwise, only the ticket's submitter or assigned developer can edit.
            return string.Equals(ticket.SubmitterUserId, user.UserId, StringComparison.Ordinal)
                || string.Equals(ticket.DeveloperUserId, user.UserId, StringComparison.Ordinal);
        }

        
        public async Task<bool> IsUserAdminPMAsync(int projectId, UserInfo user)
        {
            
                if (UserIsAdmin(user)) return true;

                var pm = await projectService.GetProjectManagerAsync(projectId, user);
            if (pm is null) return false;
            return pm?.Id == user.UserId;
            
        }

        public bool UserIsAdmin(UserInfo user) => user.IsInRole(Role.Admin);

    }
}
