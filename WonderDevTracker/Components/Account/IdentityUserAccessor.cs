using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WonderDevTracker.Models;

namespace WonderDevTracker.Components.Account
{
    internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
    {
        public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            }

            return user;
        }

        // ClaimsPrincipal-based overload for interactive components
        public async Task<ApplicationUser> GetRequiredUserAsync(ClaimsPrincipal principal)
        {
            ApplicationUser? user = await userManager.GetUserAsync(principal);

            if (user is null)
            {
                var userId = userManager.GetUserId(principal);
                throw new InvalidOperationException($"Unable to load user with ID '{userId}'.");
            }

            return user;
        }
    }
}
