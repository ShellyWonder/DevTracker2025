using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.Forms
{
    /// <summary>
    /// Used for editing the project description in EditProjectDescriptionDialog.razor.
    /// </summary>
    public sealed class ProjectDescriptionFormModel
    {
        [StringLength(4000, ErrorMessage = "Description cannot exceed 4000 characters.")]
        public string? Description { get; set; }
    }
}
