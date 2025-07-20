using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.Enums;
using static MudBlazor.CategoryTypes;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class ProjectDTO
    {
        private DateTimeOffset _startDate;
        private DateTimeOffset _created;
        private DateTimeOffset _endDate;

        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }

        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public DateTimeOffset StartDate
        {
            get => _startDate;
            set => _startDate = value.ToUniversalTime();
        }

        public DateTimeOffset EndDate
        {
            get => _endDate;
            set => _endDate = value.ToUniversalTime();
        }
        public ProjectPriority? Priority { get; set; }

        public bool Archived { get; set; } = false;

                
        public ICollection<AppUserDTO>? Members { get; set; } = [];

        public ICollection<TicketDTO> Tickets { get; set; } = [];

    }
    
}
