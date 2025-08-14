using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.Forms
{
    public sealed class ProjectNameFormModel
    {
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        [Required]
        public string? Name { get; set; }

        public string? Slug { get; set; }
    }
}
