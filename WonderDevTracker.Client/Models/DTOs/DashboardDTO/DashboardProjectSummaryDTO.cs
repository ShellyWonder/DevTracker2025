using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class DashboardProjectSummaryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ProjectPriority? Priority { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public int OpenTicketCount { get; set; }

        public int UnassignedTicketCount { get; set; }

        public int MemberCount { get; set; }

        public List<AppUserDTO> Members { get; set; } = [];
        public bool Archived { get; set; } = false;
    }
}