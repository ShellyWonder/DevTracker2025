using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.DTOs;

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
    public static class TicketAttachmentExtensions
    {
        public static TicketAttachmentDTO ToDTO(this TicketAttachment attachment)
        {
            TicketAttachmentDTO dto = new()
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                Description = attachment.Description,
                Created = attachment.Created,
                //TODO: placeholder endpoint, replace with actual file serving endpoint
                AttachmentUrl = $"/api/attachments/{attachment.UploadId}",
                UserId = attachment.UserId,
                User= attachment.User?.ToDTO(),
                TicketId = attachment.TicketId,
            };
            return dto;
        }
    }
}