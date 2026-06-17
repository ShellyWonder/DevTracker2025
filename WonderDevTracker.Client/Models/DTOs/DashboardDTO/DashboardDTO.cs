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

        #region Dashboards by role
        [Description("Dashboard data for administrators.")]
        public AdminDashboardDTO AdminDashboard { get; set; } = new();

        [Description("Dashboard data for project managers.")]
        public PMDashboardDTO PMDashboard { get; set; } = new();

        [Description("Dashboard data for developers.")]
        public DeveloperDashboardDTO DevDashboard { get; set; } = new();
        
        [Description("Dashboard data for submitters.")]
        public SubmitterDashboardDTO SubmitterDashboard { get; set; } = new();
        #endregion

    }
}
