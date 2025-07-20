using Microsoft.AspNetCore.Identity;
using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //extending IdentityUser with custom properties
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public Guid? ProfilePictureId { get; set; }

        //navigation property within Db for profile picture
        public virtual FileUpload? ProfilePicture { get; set; } //navigation property for profile picture

        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public virtual ICollection<Project>? Projects { get; set; } = [];
    }
    public static class ApplicationUserExtensions
    {
        public static AppUserDTO ToDTO(this ApplicationUser appUser)
        {
            AppUserDTO dto = new()
            {
                Id = appUser.Id,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                ImageUrl = appUser.ProfilePictureId.HasValue
                ? $"/api/uploads/{appUser.ProfilePictureId}"
                : $"https://api.dicebear.com/9.x/glass/svg?seed={appUser.UserName}",

            };
            return dto;
        }
    }
}
        