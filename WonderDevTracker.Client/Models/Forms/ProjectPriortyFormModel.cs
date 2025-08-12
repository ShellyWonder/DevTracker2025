using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Models.Forms
{
    public sealed class ProjectPriortyFormModel
    {
        /// <summary>
        /// Used for editing the project priority in EditProjectPriorityDialog.razor.
        /// </summary>
        [Required(ErrorMessage = "Priority is required.")]
        public ProjectPriority Priority { get; set; } = ProjectPriority.Low;
        public string? PriorityString => Priority.ToString();
    }
}
