using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Models
{
    public class Project

    {
        private DateTimeOffset _created;
        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;
        
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }

        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public DateTimeOffset StartDate 
        {
            get => _startDate; 
            set => _startDate = value.ToUniversalTime();
        }

       public DateTimeOffset EndDate 
        {
            get => _endDate; 
            set => _endDate = value.ToUniversalTime();
        }
        public ProjectPriority? Priority { get; set; }

        public bool Archived { get; set; } = false;

        public Guid? ImageId { get; set; }
        public virtual FileUpload? Image { get; set; }

        //foreign key to Company
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public virtual ICollection<ApplicationUser>? Members { get; set; } = [];

        public virtual ICollection<Ticket> Tickets { get; set; } = [];
    }
}