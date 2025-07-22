using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WonderDevTracker.Client;
using WonderDevTracker.Models;

namespace WonderDevTracker.Components.Account
{
    public class CustomUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
                                                 RoleManager<IdentityRole> roleManager,
                                                 IOptions<IdentityOptions> options)
                                                 : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, options)
    {
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
            string profilePictureUrl = user.ProfilePictureId.HasValue
                ? $"/api/uploads/{user.ProfilePictureId}"
                : $"https://api.dicebear.com/9.x/glass/svg?seed={user.UserName}";

            var userRoles = await UserManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                identity.AddClaim(new Claim(nameof(UserInfo.Roles), role));
            }

            // Create and add custom claims individually
            identity.AddClaim(new Claim(nameof(UserInfo.FirstName), user.FirstName));
            identity.AddClaim(new Claim(nameof(UserInfo.LastName), user.LastName));
            identity.AddClaim(new Claim(nameof(UserInfo.CompanyId), user.CompanyId.ToString()));
            identity.AddClaim(new Claim(nameof(UserInfo.ProfilePictureUrl), profilePictureUrl));


            return identity;
        }
    }

}
