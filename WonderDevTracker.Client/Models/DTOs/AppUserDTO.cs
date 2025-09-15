using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class AppUserDTO
    {
        //Default Identity behavior is to set Id as a string

        [Display(Name = "User ID")]
        [Description("Id of the user")]
        public required string Id { get; set; }

        [Display(Name = "First Name")]
        [Description("User First Name")]
        public required string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Description("User Last Name")]
        public required string LastName { get; set; }

        [JsonIgnore]
        public string? FullName => $"{FirstName} {LastName}";

        [Description("Url pointing to the user's image")]
        public string ImageUrl { get; set; } = $"https://api.dicebear.com/9.x/glass/svg?seed={Random.Shared.Next()}";

        [Description("User's assigned role in the company")]
        public Role? Role {  get; set; } 

    }
}