using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WonderDevTracker.Client.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
