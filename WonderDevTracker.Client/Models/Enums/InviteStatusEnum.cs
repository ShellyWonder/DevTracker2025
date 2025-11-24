using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.Enums
{
    public enum InviteStatusEnum
    {
        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "Accepted")]
        Accepted,

        [Display(Name = "Cancelled / Expired")]
        Cancelled
    }
}
