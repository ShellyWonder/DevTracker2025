//DashboardDTO
using System.ComponentModel;



namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    //carries data for the dashboard page
    
    public class DashboardDTO
    {
        //General stats for company dashboard, primarily for admin consumption.
        public CompanyDashboardStatsDTO CompanyStats { get; set; } = new();
        public PMDashboardStatsDTO PMStats { get; set; } = new();
        public DevDashboardStatsDTO DevStats { get; set; } = new();
        public SubmitterDashboardStatsDTO SubmitterStats { get; set; } = new();



    }
}
