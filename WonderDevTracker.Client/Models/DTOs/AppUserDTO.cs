using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.Enums;
using static MudBlazor.CategoryTypes;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class AppUserDTO
    {
        //Default Identity behavior is to set Id as a string
        public required string Id { get; set; }
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        public string? FullName => $"{FirstName} {LastName}";

        public string ImageUrl { get; set; } = $"https://api.dicebear.com/9.x/glass/svg?seed={Random.Shared.Next()}";

        public Role? Role {  get; set; } 

    }
}