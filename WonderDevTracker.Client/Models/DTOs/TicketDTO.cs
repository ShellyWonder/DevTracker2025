using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class TicketDTO
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;

        [Description("Primary key")]
        public int Id { get; set; }

        [Description("Ticket Title")]
        [Display(Name = "Ticket Name")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        [Required]
        public string? Title { get; set; }
        [Description("Ticket Description")]
        [Required(ErrorMessage = "A description is required.")]
        public string? Description { get; set; }

        [DisplayName("Creation Date")]
        [Description("Date the ticket was created(System-generated) in Utc")]
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }
        [DisplayName("Last Updated")]
        [Description("Date the ticket was updated(System-generated) in Utc")]
        public DateTimeOffset? Updated
        {

            get => _updated;
            set => _updated = value?.ToUniversalTime();
        }
        [Description("A flag indicating ticket status as active or archived(archived as a soft delete - ticket may be restored to active status by authorized user)")]
        public bool Archived { get; set; } = false;
        [Description("A flag indicating ticket archived when its parent project was archived" +
                    "(ticket restored to active status by authorized user when project returned to active status.)")]
        public bool ArchivedByProject { get; set; } = false;

        [DisplayName("Ticket Priority")]
        [Description("Ticket Priority Level")]
        [Required]
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        [DisplayName("Ticket Status")]
        [Description("Ticket Status")]
        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.New;

        [DisplayName("Ticket Type")]
        [Description("Ticket Type")]
        [Required]
        public TicketType Type { get; set; } = TicketType.Defect;

        //navigation properties
        [Required]
        [Description("The Id of the project this ticket belongs to")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid project.")]
        public int ProjectId { get; set; }

        [Description("The project this ticket belongs to")]
        public virtual ProjectDTO? Project { get; set; }

        [Description("The Id of company user who submitted this ticket")]
        public string? SubmitterUserId { get; set; }

        [Description("Company user who submitted this ticket")]
        public AppUserDTO? SubmitterUser { get; set; }

        [Description("The Id of project developer assigned to this ticket")]
        public string? DeveloperUserId { get; set; }

        [Description("Project developer assigned to this ticket")]
        public AppUserDTO? DeveloperUser { get; set; }

       [Description("Comments associated with this ticket")]
        public int CommentCount { get; set; }
        public CommentPreviewDTO? LatestComment { get; set; }

        [Description("Attachments associated with this ticket")]
        public ICollection<TicketAttachmentDTO> Attachments { get; set; } = [];

        [Description("History records associated with this ticket")]
        public ICollection<TicketHistoryDTO> History { get; set; } = [];

        /// <summary>
        /// Computed property to get the most recent modification date 
        /// (either Updated or Created if never updated)
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset Modified => Updated ?? Created;


    }

    public sealed class CommentPreviewDTO
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = default!;
        public string Snippet { get; set; } = default!;   
        public DateTimeOffset Created { get; set; }
    }
}

