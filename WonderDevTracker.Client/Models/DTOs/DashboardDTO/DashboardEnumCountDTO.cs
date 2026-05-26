using System.ComponentModel;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class DashboardEnumCountDTO<TEnum>
    where TEnum : struct, Enum
    {
        [Description("Value of the enum, used to determine the color of the chart bar based on the enum value.")]
        public TEnum? Value { get; set; }

        [Description("Label of the enum, used to display the name of the enum value on the chart x-axis.")]
        public string Label { get; set; } = string.Empty;

        [Description("Count of the enum value, used to display the number of occurrences on the chart y-axis.")]
        public int Count { get; set; }
    }
}