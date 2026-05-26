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

        //A symmetric overload called on TEnum directly(no boxing/casts in the component):
        public static string GetDisplayName<TEnum>(this TEnum value) where TEnum : struct, Enum
                            => ((Enum)(object)value).GetDisplayName();

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
            },
            {
                typeof(TicketType), new()
                {
                    { TicketType.NewDevelopment, Color.Primary },
                    { TicketType.Enhancement, Color.Info },
                    { TicketType.Defect, Color.Error },
                    { TicketType.WorkTask, Color.Secondary },
                    { TicketType.ChangeRequest, Color.Warning },
                    { TicketType.GeneralTask, Color.Tertiary }
                }
            },
                {
                    typeof(NotificationType), new()
                    {
                        { NotificationType.Company, Color.Secondary },
                        { NotificationType.Project, Color.Warning },
                        { NotificationType.Ticket, Color.Success }
                    }
                }
        };
        public static Color GetColor<TEnum>(this TEnum? value) where TEnum : struct, Enum
        {
            if (value is null) return Color.Default;

            if (_enumColorMap.TryGetValue(typeof(TEnum), out var map)
              && map.TryGetValue((Enum)(object)value.Value, out var color)) // <-- boxed cast
            {
                return color;
            }

            return Color.Default;
        }

        // Overload for non-nullable enums
        public static Color GetColor<TEnum>(this TEnum value) where TEnum : struct, Enum
        => ((TEnum?)value).GetColor<TEnum>();

        // Method to get hex color code(ApexCharts-required) for charting purposes
        public static string GetChartColorHex<TEnum>(this TEnum value)
                                             where TEnum : struct, Enum
        {
            return value switch
            {
                TicketStatus.New => "#1EC8A5",
                TicketStatus.InDevelopment => "#5D81C9",
                TicketStatus.InTesting => "#FF993B",
                TicketStatus.Resolved => "#90BE6D",

                TicketPriority.Low => "#1EC8A5",
                TicketPriority.Medium => "#5D81C9",
                TicketPriority.High => "#FF993B",
                TicketPriority.Urgent => "#E63946",

                ProjectPriority.Low => "#1EC8A5",
                ProjectPriority.Medium => "#5D81C9",
                ProjectPriority.High => "#FF993B",
                ProjectPriority.Urgent => "#E63946", 

                _ => "#0077B6"
            };
        }
    }


}
