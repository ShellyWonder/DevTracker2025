using System.Text.Json.Serialization;

namespace WonderDevTracker.Client.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProjectsFilter
    {
        Active,
        Archived,
        Assigned,
    }
}
