using System.ComponentModel.DataAnnotations;
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

            public TicketPriorityDTO? Priority { get; set; }
            public TicketStatusDTO? Status { get; set; }

            public TicketTypeDTO? Type { get; set; }

            //navigation properties
            public int ProjectId { get; set; }
            public virtual ProjectDTO? Project { get; set; }

            [Required]
            public string? SubmitterUserId { get; set; }
            public AppUserDTO? SubmitterUser { get; set; }
            public string? DeveloperUserId { get; set; }
            public AppUserDTO? DeveloperUser { get; set; }

            public ICollection<TicketCommentDTO> Comments { get; set; } = [];
            public ICollection<TicketAttachmentDTO> Attachments { get; set; } = [];
            public ICollection<TicketHistoryDTO> History { get; set; } = [];


        }
    }

