namespace WonderDevTracker.Client.Models.ViewModels
{
    // This class encapsulates the permissions for various dashboard actions based on the user's roles and project memberships.
    public class DashboardActionPermissions
    {
        public bool CanManageProjects { get; set; }
        public bool CanManageTickets { get; set; }
        public bool CanManageUsers { get; set; }
        public bool CanSubmitTickets { get; set; }
    }
}
