using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

       
        public string? FullName => $"{FirstName} {LastName}".Trim();

        [Description("Url pointing to the user's image")]
        public string? ImageUrl { get; set; }

        public string Initials { get; set; } = "?";

        public bool HasProfileImage => !string.IsNullOrWhiteSpace(ImageUrl);

        [Description("User's assigned role in the company")]
        public Role? Role {  get; set; } 

    }
}