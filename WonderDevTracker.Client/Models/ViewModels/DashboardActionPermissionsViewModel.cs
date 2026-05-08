namespace WonderDevTracker.Client.Models.ViewModels
{
    // This class encapsulates the permissions for various dashboard actions based on the user's roles and project memberships.
    public class DashboardActionPermissionsViewModel
    {
        public bool CanManageProjects { get; set; }
        public bool CanManageTickets { get; set; }
        public bool CanManageMembers { get; set; }
        public bool CanSubmitTickets { get; set; }
        public bool CanViewInvites { get; set; }
        public bool CanViewUnassignedTickets { get; set; }
        public bool CanViewArchivedTickets { get; set; }


    }
}
