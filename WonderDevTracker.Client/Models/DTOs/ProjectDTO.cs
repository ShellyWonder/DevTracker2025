using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WonderDevTracker.Client.Models.Enums;
using static MudBlazor.CategoryTypes;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class ProjectDTO
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _startDate;
        private DateTimeOffset? _endDate;

        [Description("Primary key")]
        public int Id { get; set; }

        [Description("Project Title")]
        [Display(Name = "Project Name")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        [Required]
        public string? Name { get; set; }

        [Description("Project Description")]
        [Required (ErrorMessage = "A description is required.")]
        public string? Description { get; set; }

        [Description("Date the project was created(System-generated) in Utc")]
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }
        [Required]
        [Description("Project start date, in Utc")]
        public DateTimeOffset? StartDate
        {
            get => _startDate;
            set => _startDate = value?.ToUniversalTime();
        }

        [Description("Project's target completion date, in Utc")]
        [Required]
        public DateTimeOffset? EndDate
        {
            get => _endDate;
            set => _endDate = value?.ToUniversalTime();
        }
        [Description("Project Priority Level")]
        [Required]
        public ProjectPriority Priority { get; set; } = ProjectPriority.Low;

        [Description("A flag indicating project status as active or archived")]
        public bool Archived { get; set; } = false;

        [Description("Company members (users) assigned to this project")]
        public ICollection<AppUserDTO>? Members { get; set; } = [];

        [Description("Tickets (tasks) assigned to this project")]
        public ICollection<TicketDTO> Tickets { get; set; } = [];

        #region Helper Properties 
        /// <summary>
        /// Helpers to convert DateTimeOffset to DateTime in UTC
        /// Necessary for MudBlazor DatePicker to work correctly.
        /// Ignored by the API and not mapped to the database.
        /// </summary>
        /// 
        [JsonIgnore]
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


        [JsonIgnore]
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
