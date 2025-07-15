using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.Enums
{
    public enum TicketType
    {
        [Display(Name = "New Development")]
        NewDevelopment,
        Enhancement,
        Defect,
        [Display(Name = "Work Task")]
        WorkTask,
        [Display(Name = "Change Request")]
        ChangeRequest,
        [Display(Name = "General Task")]
        GeneralTask,
    }
}
