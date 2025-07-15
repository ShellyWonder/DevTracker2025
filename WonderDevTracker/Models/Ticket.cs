using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Models
{
    public class Ticket
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

        public TicketPriority? Priority { get; set; }
        public TicketStatus? Status { get; set; }

        public TicketType? Type { get; set; }

        //navigation properties
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        [Required]
        public string? SubmitterUserId { get; set; }
        public virtual ApplicationUser? SubmitterUser { get; set; }
        public string? DeveloperUserId { get; set; }
        public virtual ApplicationUser? DeveloperUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; } = [];
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = [];
        public virtual ICollection<TicketHistory> History { get; set; } = [];





    }
}