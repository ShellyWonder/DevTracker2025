using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class DashboardTicketSummaryDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        [Display(Name = "Project")]
        public string ProjectName { get; set; } = string.Empty;

        public TicketStatus? Status { get; set; }
        public TicketPriority? Priority { get; set; }
        public TicketType? Type { get; set; }

        [Display(Name = "Submitter")]
        public string SubmitterName { get; set; } = string.Empty;

        [Display(Name = "Developer")]
        public string? DeveloperName { get; set; } = string.Empty;

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        public DateTimeOffset LastActivity => Updated ?? Created;
    }
}
