//DashboardDTO
using System.ComponentModel;



namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    //carries data for the dashboard page
    
    public class DashboardDTO
    {
        //CompanyInfo:Basic info about the company, used for display and to determine what data to load.
        public CompanyDashboardInfoDTO CompanyInfo { get; set; } = new();

        //CompanyStats:General stats for company dashboard, primarily for admin consumption.
        public CompanyDashboardStatsDTO CompanyStats { get; set; } = new();
        public PMDashboardStatsDTO PMStats { get; set; } = new();
        public DevDashboardStatsDTO DevStats { get; set; } = new();
        public SubmitterDashboardStatsDTO SubmitterStats { get; set; } = new();
        public List<DashboardTicketSummaryDTO> RecentActiveTickets { get; set; } = [];
        public List<DashboardTicketSummaryDTO> RecentResolvedTickets { get; set; } = [];
        public List<DashboardTicketSummaryDTO> RecentUnassignedTickets { get; set; } = [];
        public IEnumerable<DashboardMonthlyTicketsDTO> TicketsOverTime { get; set; } = [];
        public IEnumerable<DashboardMonthlyTicketsDTO> ResolvedTicketsOverTime { get; set; } = [];




    }
}
