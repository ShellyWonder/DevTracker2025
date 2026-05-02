//CompanyDashboardStatsDTO.cs

using System.ComponentModel;
/// <summary>
/// Represents the statistics for a company's dashboard.
/// </summary>
/// <remarks>DTO used for displaying company-level dashboard statistics.
/// Primary use case is for ADMIN-level company dashboards.</remarks>

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class CompanyDashboardStatsDTO
    {
        [Description("Total number of company projects.")]
        public int TotalProjectCount { get; set; }

        [Description("Total number of tickets across all projects.")]
        public int TotalTicketCount { get; set; }

        [Description("Total number of open tickets not in a resolved status.")]
        public int OpenTicketCount { get; set; }

        [Description("Total number of resolved status tickets.")]
        public int ResolvedTicketCount { get; set; }
    }
}
