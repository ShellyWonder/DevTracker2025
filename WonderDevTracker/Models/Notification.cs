using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Models
{
    public class Notification
    {
        //backing field for Created to ensure it is stored in UTC
        private DateTimeOffset _created;

        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Message { get; set; }

        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }
        public NotificationType Type { get; set; }

        public bool HasBeenViewed { get; set; } = false;


        // Navigation properties
        public int? TicketId { get; set; }
        public virtual Ticket? Ticket { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Required]
        public string? SenderId { get; set; }
        public virtual ApplicationUser? Sender { get; set; }
        [Required]
        public string? RecipientId { get; set; }
        public virtual ApplicationUser? Recipient { get; set; }



    }
}
