using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Models
{
    public class Company
    {
        public int Id { get; set; }
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        [Required]
        public string? Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public Guid? ImageId { get; set; }

        public virtual FileUpload? Image { get; set; }

        public virtual ICollection<ApplicationUser>? Members { get; set; } = [];
        public virtual ICollection<Project>? Projects { get; set; } = [];
        public virtual ICollection<Invite>? Invites { get; set; } = [];

        
    }

}
