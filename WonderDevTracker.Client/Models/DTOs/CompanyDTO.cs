using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static MudBlazor.CategoryTypes;
using static System.Net.WebRequestMethods;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class CompanyDTO
    {

        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        [Required]
        public string? Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = $"https://api.dicebear.com/9.x/glass/svg?seed={Random.Shared.Next()}";

        public ICollection<AppUserDTO>? Members { get; set; } = [];
        public ICollection<ProjectDTO>? Projects { get; set; } = [];
        public ICollection<InviteDTO>? Invites { get; set; } = [];

    }
}
