using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.Enums;
using static MudBlazor.CategoryTypes;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class ProjectDTO
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _startDate;
        private DateTimeOffset? _endDate;

        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        [Required]
        public string? Name { get; set; }

        [Required (ErrorMessage = "A description is required.")]
        public string? Description { get; set; }

       
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public DateTimeOffset? StartDate
        {
            get => _startDate;
            set => _startDate = value?.ToUniversalTime();
        }

        [Required]
        public DateTimeOffset? EndDate
        {
            get => _endDate;
            set => _endDate = value?.ToUniversalTime();
        }

        [Required]
        public ProjectPriority Priority { get; set; } = ProjectPriority.Low;

        public bool Archived { get; set; } = false;

                
        public ICollection<AppUserDTO>? Members { get; set; } = [];

        public ICollection<TicketDTO> Tickets { get; set; } = [];

        #region Helper Properties 
        /// <summary>
        /// Helpers to convert DateTimeOffset to DateTime in UTC
        /// Necessary for MudBlazor DatePicker to work correctly
        /// </summary>
        /// 
        [Required(ErrorMessage = "Start Date is required and must precede the end date")]
        public DateTime? StartDateTime
        {
            // Present local date (no time) to the UI
            get => StartDate?.ToLocalTime().DateTime.Date;

            // Convert a local date picked in the UI to UTC midnight safely
            set => StartDate = value.HasValue
                ? new DateTimeOffset(DateTime.SpecifyKind(value.Value.Date, DateTimeKind.Local))
                    .ToUniversalTime()
                : null;
        }

        [Required(ErrorMessage = "End Date is required and must follow the start date")]
        public DateTime? EndDateTime
        {
            get => EndDate?.ToLocalTime().DateTime.Date;

            set => EndDate = value.HasValue
                ? new DateTimeOffset(DateTime.SpecifyKind(value.Value.Date, DateTimeKind.Local))
                    .ToUniversalTime()
                : null;
        }
        #endregion
    }

}
