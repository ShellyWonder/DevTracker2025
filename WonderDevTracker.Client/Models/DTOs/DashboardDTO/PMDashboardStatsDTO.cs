//PMDashboardStatsDTO

using System.ComponentModel;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class PMDashboardStatsDTO
    {
        [Description("Total number of projects the PM is managing.")]
        public int ManagedProjectCount { get; set; }

        [Description("Total number of tickets across all managed projects.")]
        public int ManagedProjectTicketCount { get; set; }

        [Description("Number of open tickets across all managed projects.")]
        public int OpenManagedTicketCount { get; set; }

        [Description("Number of resolved tickets across all managed projects.")]
        public int ResolvedManagedTicketCount { get; set; }
    }
}