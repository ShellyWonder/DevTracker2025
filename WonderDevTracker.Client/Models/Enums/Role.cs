using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.Enums
{
    public enum Role
    {
        [Display(Name="Administrator")]
        Admin,
        [Display(Name="Project Manager")]ProjectManager,
        Developer,
        Submitter,
        [Display(Name = "Demo User")]
        DemoUser,
    }
}
