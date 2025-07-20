using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.DTOs;

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
    public static class TicketCommentExtensions
    {
        public static TicketCommentDTO ToDTO(this TicketComment comment)
        {
            TicketCommentDTO dto = new()
            {
                Id = comment.Id,
                Content = comment.Content,
                Created = comment.Created,
                Edited = comment.Edited,
                UserId = comment.UserId,
                User = comment.User?.ToDTO(),
                TicketId = comment.TicketId
            };
            return dto;
        }
    }
}