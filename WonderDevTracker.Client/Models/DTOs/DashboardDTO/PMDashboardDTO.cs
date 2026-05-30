using System.ComponentModel;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class PMDashboardDTO
    {
        [Description("Stats for project managers on the dashboard.")]
        public PMDashboardStatsDTO PMStats { get; set; } = new();

        [Description("List of projects managed by the project manager.")]
        public List<DashboardProjectSummaryDTO> ManagedProjects { get; set; } = [];

        [Description("List of unassigned tickets that the project manager can assign to developers.")]
        public List<DashboardTicketSummaryDTO> UnassignedTickets { get; set; } = [];

        [Description("Chart data for project managers on the dashboard.")]
        public DashboardChartDataDTO PMChartData { get; set; } = new();

        [Description("List of team members assigned to the project manager.")]
        public List<AppUserDTO> TeamMembers { get; set; } = [];
    }
}
