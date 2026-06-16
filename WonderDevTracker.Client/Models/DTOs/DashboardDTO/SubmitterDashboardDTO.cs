//SubmitterDashboardDTO
using System.ComponentModel;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    /// <summary>
    /// DTO for submitter role-specific dashboard data, 
    /// including stats and lists of tickets submitted by the current user. 
    /// Since any user can submit tickets, 
    /// the other role DTOs have their own submitted ticket list properties.
    /// </summary>
    public class SubmitterDashboardDTO
    {
        [Description("Stats for submitters on the dashboard.")]
        public SubmitterDashboardStatsDTO SubmitterStats { get; set; } = new();

        [Description("List of tickets submitted by the current user.")]
        public List<DashboardTicketSummaryDTO> MySubmittedTickets { get; set; } = [];
    }
}