//DeveloperDashboardDTO
using System.ComponentModel;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    /// <summary>
    /// DTO for the developer dashboard, containing all necessary data for rendering the developer dashboard view.
    /// </summary>
    public class DeveloperDashboardDTO
    {
        [Description("Stats for developer dashboard.")]
        public DevDashboardStatsDTO DevStats { get; set; } = new();

        [Description("List of developer-assigned projects.")]
        public List<DashboardProjectSummaryDTO> DevProjects { get; set; } = [];

        [Description("List of developer-assigned tickets.")]
        public List<DashboardTicketSummaryDTO> AssignedTickets { get; set; } = [];

        [Description("Chart data for developer dashboard.")]
        public DashboardChartDataDTO DevChartData { get; set; } = new DashboardChartDataDTO();

        [Description("List of tickets submitted by the current user.")]
        public List<DashboardTicketSummaryDTO> MySubmittedTickets { get; set; } = [];
    }
}