//AdminDashboardDTO
using System.ComponentModel;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class AdminDashboardDTO

    {
        #region Stats for dashboard cards
        [Description("General stats for company dashboard, primarily for admin consumption.")]
        public CompanyDashboardStatsDTO CompanyStats { get; set; } = new();
        #endregion

        #region Dashboard Ticket Lists for tables
        [Description("List of recent active tickets on the dashboard.")]
        public List<DashboardTicketSummaryDTO> RecentActiveTickets { get; set; } = [];

        [Description("List of recent resolved tickets on the dashboard.")]
        public List<DashboardTicketSummaryDTO> RecentResolvedTickets { get; set; } = [];

        [Description("List of recent unassigned tickets on the dashboard.")]
        public List<DashboardTicketSummaryDTO> RecentUnassignedTickets { get; set; } = [];

        [Description("List of tickets submitted by the current user.")]
        public List<DashboardTicketSummaryDTO> MySubmittedTickets { get; set; } = [];
        #endregion

        #region Chart data for dashboard charts

        [Description("Data for charts on the dashboard.")]
        public DashboardChartDataDTO ChartData { get; set; } = new();

        #endregion

        #region Dashboard Projects
        [Description("List of projects to display on the dashboard.")]
        public List<DashboardProjectSummaryDTO> RecentProjects { get; set; } = [];
        #endregion

    }
}