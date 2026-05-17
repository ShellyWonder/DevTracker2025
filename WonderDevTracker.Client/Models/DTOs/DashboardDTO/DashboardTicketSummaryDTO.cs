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

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }

        public AppUserDTO? DeveloperUser { get; set; }

        public AppUserDTO? SubmitterUser { get; set; }

        public DateTimeOffset LastActivity => Updated ?? Created;

        [Display(Name = "Submitter")]
        public string SubmitterName => SubmitterUser?.FullName ?? "Unassigned";

        [Display(Name = "Developer")]
        public string? DeveloperName => DeveloperUser?.FullName ?? "Unknown";



    }
}
