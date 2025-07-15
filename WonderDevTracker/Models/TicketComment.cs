using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Models
{
    public class TicketComment
    {
        private DateTimeOffset _created;

        public int Id { get; set; }
        [Required]
        public string? Content { get; set; } = string.Empty;

        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }
        public bool Edited { get; set; } = false;

        //navigation properties
        public int TicketId { get; set; }
        public virtual Ticket? Ticket { get; set; }

        [Required]
        public virtual string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}