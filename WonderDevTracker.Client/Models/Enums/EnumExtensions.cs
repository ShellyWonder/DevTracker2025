using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WonderDevTracker.Client.Models.Enums
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            string? displayName = value.GetType()
                                       .GetMember(value.ToString())
                                       .FirstOrDefault()?
                                       .GetCustomAttribute<DisplayAttribute>()?.GetName();

            return string.IsNullOrEmpty(displayName) ? value.ToString() : displayName;
 
        }

        private static readonly Dictionary<Type, Dictionary<Enum, Color>> _enumColorMap = new()
        {
            {
                typeof(ProjectPriority), new()
                {
                    { ProjectPriority.Low, Color.Tertiary },
                    { ProjectPriority.Medium, Color.Info },
                    { ProjectPriority.High, Color.Warning },
                    { ProjectPriority.Urgent, Color.Error }
                }
            },
            {
                typeof(TicketPriority), new()
                {
                    { TicketPriority.Low, Color.Tertiary },
                    { TicketPriority.Medium, Color.Info },
                    { TicketPriority.High, Color.Warning },
                    { TicketPriority.Urgent, Color.Error }
                }
            },
            {
                typeof(TicketStatus), new()
                {
                    { TicketStatus.New, Color.Tertiary },
                    { TicketStatus.InDevelopment, Color.Info },
                    { TicketStatus.InTesting, Color.Warning },
                    { TicketStatus.Resolved, Color.Success }
                }
            }
        };
        public static Color GetColor<TEnum>(this TEnum? value) where TEnum : struct, Enum
        {
            if (value is null) return Color.Default;

            if (_enumColorMap.TryGetValue(typeof(TEnum), out var map)
                && map.TryGetValue(value.Value, out var color))
            {
                return color;
            }

            return Color.Default;
        }
    }
}
