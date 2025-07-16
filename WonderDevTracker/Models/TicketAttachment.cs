using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Models
{
    public class TicketAttachment
    {
        private DateTimeOffset _created;
        public int Id { get; set; }

        [Required]
        public string? FileName { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        //navigation properties --all strings are nullable
        public Guid UploadId { get; set; }
        public virtual FileUpload? Upload { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket? Ticket { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}