using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.Forms
{
    public sealed class ProjectDatesFormModel
    {
        [Required] 
        public DateTime? StartLocal { get; set; }

        [Required] 
        public DateTime? EndLocal { get; set; }
    }
}
