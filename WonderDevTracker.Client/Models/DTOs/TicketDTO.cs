using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class TicketDTO
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;

        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public DateTimeOffset? Updated
        {

            get => _updated;
            set => _updated = value?.ToUniversalTime();
        }

        public bool Archived { get; set; } = false;

        public bool ArchivedByProject { get; set; } = false;

        [Required]
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.New;
        [Required]
        public TicketType Type { get; set; } = TicketType.Defect;

        //navigation properties
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid project.")]
        public int ProjectId { get; set; }
        public virtual ProjectDTO? Project { get; set; }

      
        public string? SubmitterUserId { get; set; }
        public AppUserDTO? SubmitterUser { get; set; }
        public string? DeveloperUserId { get; set; }
        public AppUserDTO? DeveloperUser { get; set; }

        public ICollection<TicketCommentDTO> Comments { get; set; } = [];
        public ICollection<TicketAttachmentDTO> Attachments { get; set; } = [];
        public ICollection<TicketHistoryDTO> History { get; set; } = [];

        /// <summary>
        /// Computed property to get the most recent modification date 
        /// (either Updated or Created if never updated)
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset Modified => Updated ?? Created;


    }
}

