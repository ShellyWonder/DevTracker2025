//DevDashboardStatsDTO
using System.ComponentModel;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class DevDashboardStatsDTO
    {
        [Description("Total number of projects the developer is assigned to.")]
        public int AssignedProjectsCount { get; set; }
        
        [Description("Total number of tickets assigned to the developer.")]
        public int AssignedTicketCount { get; set; }

        [Description("Number of open tickets assigned to the developer.")]
        public int OpenAssignedTicketCount { get; set; }

        [Description("Number of resolved tickets assigned to the developer.")]
        public int ResolvedAssignedTicketCount { get; set; }
    }
}