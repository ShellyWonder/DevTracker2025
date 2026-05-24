using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class DashboardTicketsByProjectDTO
    {
        public int ProjectId { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; } = string.Empty;
        
        [Display(Name = "Total Tickets")]
        public int TotalTicketCount { get; set; }

        [Display(Name = "Open Tickets")]
        public int OpenTicketCount { get; set; }

        [Display(Name = "Resolved Tickets")]
        public int ResolvedTicketCount { get; set; }
    }
}