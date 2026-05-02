//SubmitterDashboardStatsDTO
using System.ComponentModel;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class SubmitterDashboardStatsDTO
    {
        [Description("Total number of projects the submitter has submitted tickets to.")]
        public int SubmittedTicketCount { get; set; }

        [Description("Number of open tickets submitted by the submitter.")]
        public int OpenSubmittedTicketCount { get; set; }

        [Description("Number of resolved tickets submitted by the submitter.")]
        public int ResolvedSubmittedTicketCount { get; set; }
    }
}
