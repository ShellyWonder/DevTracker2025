using Microsoft.AspNetCore.Identity;
using WonderDevTracker.Components.Account;
using WonderDevTracker.Models;

namespace WonderDevTracker.Infrastructure
{
    /// <summary>
    /// Provides a minimal Api to refresh auth cookie for Account/Manage/Index when in interactive render mode
    /// </summary>
    /// <remarks>
    /// Provides extension methods for mapping account-related endpoints to an endpoint route builder.
    /// This class contains extension methods that add account management endpoints, such as sign-in
    /// refresh, to ASP.NET Core endpoint routing. These endpoints typically require user authentication.</remarks>
    public static class AccountEndpointsExtensions
    {
        public static IEndpointConventionBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var account = endpoints.MapGroup("/Account");

            account.MapGet("/RefreshSignin", async (
               HttpContext httpContext,
               UserManager<ApplicationUser> userManager,
               SignInManager<ApplicationUser> signInManager) =>
            {
                var user = await userManager.GetUserAsync(httpContext.User);
                if (user is null)
                    return Results.Unauthorized();

                await signInManager.RefreshSignInAsync(user);

                // navigate back to Manage page after cookie refresh
                return Results.LocalRedirect("/Account/Manage?updated=profile");
            })
               .RequireAuthorization(); // user must be logged in

            return account;
        }
    }
}
