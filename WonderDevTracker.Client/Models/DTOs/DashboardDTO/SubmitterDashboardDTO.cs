//SubmitterDashboardDTO
using System.ComponentModel;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class SubmitterDashboardDTO
    {
        [Description("Stats for submitters on the dashboard.")]
        public SubmitterDashboardStatsDTO SubmitterStats { get; set; } = new();

        [Description("List of tickets submitted by the current user.")]
        public List<DashboardTicketSummaryDTO> MySubmittedTickets { get; set; } = [];
    }
}