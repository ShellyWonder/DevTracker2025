using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.DTOs;

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
    public static class CompanyExtensions
    {
        public static CompanyDTO ToDTO(this Company company)
        {
            CompanyDTO dto = new()
            {
                Name = company.Name,
                Description = company.Description,
                ImageUrl = company.ImageId.HasValue
                    ? $"/api/uploads/{company.ImageId}"
                    : $"https://api.dicebear.com/9.x/glass/svg?seed={company.Name}",
                //TODO:Members = company.Members?.Select(m => m.ToDTO()).ToList() ?? [],
                //TODO:Projects = company.Projects?.Select(p => p.ToDTO()).ToList() ?? [],
                //TODO:Invites = company.Invites?.Select(i => i.ToDTO()).ToList() ?? []
            };
            return dto;
        }

    }
}
