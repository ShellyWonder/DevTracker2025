using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.Enums
{
    public enum TicketStatus
    {
        New,
        [Display(Name = "In Development")]
        InDevelopment,
        [Display(Name = "Testing")]
        InTesting,
        Resolved,
    }
}
