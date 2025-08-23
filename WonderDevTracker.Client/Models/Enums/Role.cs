using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WonderDevTracker.Client.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
