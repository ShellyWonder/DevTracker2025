using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WonderDevTracker.Client.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
