using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Models
{
    public class Notification
    {
        //backing field for Created & ArchivedAt to ensure they are stored in UTC
        private DateTimeOffset _created;
        private DateTimeOffset? _archivedAt;

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

        public bool IsArchived { get; set; } = false;
       
        public DateTimeOffset? ArchivedAt
        {
            get => _archivedAt;
            set => _archivedAt = value?.ToUniversalTime();
        }

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
    public static class NotificationExtensions
    {
        public static NotificationDTO ToDTO(this Notification notification)
        {
            NotificationDTO dto = new()
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Created = notification.Created,
                ArchivedAt = notification.ArchivedAt,
                Type = notification.Type,
                HasBeenViewed = notification.HasBeenViewed,
                IsArchived = notification.IsArchived,
                TicketId = notification.TicketId,
                ProjectId = notification.ProjectId,
                SenderId = notification.SenderId,
                RecipientId = notification.RecipientId
            };
            return dto;
        }
    }
}
