using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Models;
using WonderDevTracker.Services.DemoSupport;

namespace WonderDevTracker.Infrastructure
{
    public static class DemoLoginEndpointExtensions
    {
        public static IEndpointRouteBuilder MapDemoLoginEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/account/demo-login", async (
                [FromForm] string demoRole,
                [FromForm] string? returnUrl,
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                IConfiguration configuration) =>
            {
                string? email = DemoUserCatalog.GetEmailForDemoRole(demoRole);

                if (string.IsNullOrWhiteSpace(email))
                {
                    return Results.LocalRedirect("/Account/Login?demoLoginFailed=true");
                }

                string defaultPassword = configuration["DemoUsers:DefaultPassword"]
                    ?? throw new InvalidOperationException("DemoUsers:DefaultPassword is missing.");

                ApplicationUser? user = await userManager.FindByEmailAsync(email);

                if (user is null)
                {
                    return Results.LocalRedirect("/Account/Login?demoLoginFailed=true");
                }

                bool isDemoUser = await userManager.IsInRoleAsync(user, nameof(Role.DemoUser));

                if (!isDemoUser)
                {
                    return Results.LocalRedirect("/Account/Login?demoLoginFailed=true");
                }

                Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(
                user,
                defaultPassword,
                isPersistent: false,
                lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    return Results.LocalRedirect("/Account/Login?demoLoginFailed=true");
                }

                string safeReturnUrl = GetSafeLocalReturnUrl(returnUrl);

                return Results.LocalRedirect(safeReturnUrl);
            })
        .AllowAnonymous();

            return app;

        }

        private static string GetSafeLocalReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return "/dashboard";
            }

            // Prevent open redirects.
            if (returnUrl.StartsWith('/') &&
                !returnUrl.StartsWith("//") &&
                !returnUrl.StartsWith("/\\"))
            {
                return returnUrl;
            }

            return "/dashboard";
        }
    }
}