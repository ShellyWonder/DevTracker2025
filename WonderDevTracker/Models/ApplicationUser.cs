using Microsoft.AspNetCore.Identity;

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

}
