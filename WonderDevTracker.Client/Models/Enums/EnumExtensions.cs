using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WonderDevTracker.Client.Models.Enums
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(Enum value)
        {
            string? displayName = value.GetType()
                                       .GetMember(value.ToString())
                                       .FirstOrDefault()?
                                       .GetCustomAttribute<DisplayAttribute>()?.GetName();

            if (string.IsNullOrEmpty(displayName)) return value.ToString();
           
            return displayName;
        }
    }
}
