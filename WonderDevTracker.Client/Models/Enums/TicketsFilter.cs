using System.Text.Json.Serialization;

namespace WonderDevTracker.Client.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TicketsFilter
    {
        Open,
        Assigned,
        Resolved,
        Archived
    }
}
