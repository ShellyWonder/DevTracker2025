//DashboardDTO
using System.ComponentModel;



namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    //carries data for the dashboard page
    
    public class DashboardDTO
    {
        [Description("Basic info about the company, used for display and to determine what data to load.")]
        public CompanyDashboardInfoDTO CompanyInfo { get; set; } = new();

        [Description("General stats for company dashboard, primarily for admin consumption.")]
        public CompanyDashboardStatsDTO CompanyStats { get; set; } = new();

        [Description("Stats for project managers on the dashboard.")]
        public PMDashboardStatsDTO PMStats { get; set; } = new();

        [Description("Stats for developers on the dashboard.")]
        public DevDashboardStatsDTO DevStats { get; set; } = new();

        [Description("Stats for submitters on the dashboard.")]
        public SubmitterDashboardStatsDTO SubmitterStats { get; set; } = new();

        [Description("List of recent active tickets on the dashboard.")]
        public List<DashboardTicketSummaryDTO> RecentActiveTickets { get; set; } = [];

        [Description("List of recent resolved tickets on the dashboard.")]
        public List<DashboardTicketSummaryDTO> RecentResolvedTickets { get; set; } = [];

        [Description("List of recent unassigned tickets on the dashboard.")]
        public List<DashboardTicketSummaryDTO> RecentUnassignedTickets { get; set; } = [];

        [Description("Data for charts on the dashboard.")]
        public DashboardChartDataDTO ChartData { get; set; } = new();



    }
}
