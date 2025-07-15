using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Models
{
    public class Project

    {
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; } 

        public DateTimeOffset Created { get; set; } 

        public DateTimeOffset? StartDate { get; set; }
     
        public DateTime? EndDate { get; set; }
        public ProjectPriority? Priority { get; set; } 

        public bool Archived { get; set; } = false;

        public Guid? ImageId { get; set; }
        public virtual ImageUpload? Image { get; set; }

        //foreign key to Company
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public virtual ICollection<ApplicationUser>? Members { get; set; } = [];

        public virtual ICollection<Ticket> Tickets { get; set; } = [];
    }
}