//DashboardDTO
using System.ComponentModel;
using WonderDevTracker.Client.Models.DTOs;



namespace WonderDevTracker.Client.Models.DTOs
{
    //carries data for the dashboard page:
    //total projects
    //total tickets
    //open tickets
    //closed tickets
    public class DashboardDTO
    {
        [Description("Total number of companyprojects.")]
        public int TotalProjectCount { get; set; }

        [Description("Total number of tickets across all projects.")]
        public int TotalTicketCount { get; set; }

        [Description("Total number of open tickets not in a resolved status.")]
        public int OpenTicketCount { get; set; }

        [Description("Total number of resolved status tickets.")]
        public int ResolvedTicketCount { get; set; }

    }
}
