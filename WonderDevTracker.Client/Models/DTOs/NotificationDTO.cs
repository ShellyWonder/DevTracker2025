using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class NotificationDTO
    {
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
        public  TicketDTO? Ticket { get; set; }
        public int? ProjectId { get; set; }
        public  ProjectDTO? Project { get; set; }

        [Required]
        public string? SenderId { get; set; }
        public  AppUserDTO? Sender { get; set; }

        [Required]
        public string? RecipientId { get; set; }
        public virtual AppUserDTO? Recipient { get; set; }
    }
}
