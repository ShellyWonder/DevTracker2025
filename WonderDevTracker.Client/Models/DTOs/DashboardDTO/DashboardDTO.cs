//DashboardDTO
using System.ComponentModel;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{

    /// <summary>
    /// DTO for the dashboard, containing all necessary data for rendering the dashboard view.
    /// </summary>
    public class DashboardDTO
    {
        [Description("Basic info about the company, used for display and to determine what data to load.")]
        public CompanyDashboardInfoDTO CompanyInfo { get; set; } = new();

        #region Stats for dashboard cards
        [Description("General stats for company dashboard, primarily for admin consumption.")]
        public CompanyDashboardStatsDTO CompanyStats { get; set; } = new();

        #region Dashboards by role
        [Description("Dashboard data for project managers.")]
        public PMDashboardDTO PMDashboard { get; set; } = new();

        [Description("Dashboard data for developers.")]
        public DeveloperDashboardDTO DevDashboard { get; set; } = new();
        #endregion

        [Description("Stats for submitters on the dashboard.")]
        public SubmitterDashboardStatsDTO SubmitterStats { get; set; } = new();
        #endregion

        #region Dashboard Projects
        [Description("List of projects to display on the dashboard.")]
        public List<DashboardProjectSummaryDTO> RecentProjects { get; set; } = [];
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

    }
}
