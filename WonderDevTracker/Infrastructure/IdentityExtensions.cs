using Microsoft.AspNetCore.Identity;
using WonderDevTracker.Components.Account;
using WonderDevTracker.Models;

namespace WonderDevTracker.Infrastructure
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityAndAuth(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(cookieBuilder =>
            {
                cookieBuilder.ApplicationCookie!.Configure(config =>
                {
                    config.Events.OnRedirectToLogin += (context) =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api") || context.Request.HasJsonContentType())
                        {
                            context.Response.StatusCode = 401; // Unauthorized(user not authenticated)
                        }
                        return Task.CompletedTask;
                    };

                    config.Events.OnRedirectToAccessDenied += (context) =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api") || context.Request.HasJsonContentType())
                        {
                            context.Response.StatusCode = 403; // Forbidden (credentials not authorized)
                        }
                        return Task.CompletedTask;
                    };


                });
            });
            
           

            services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<WonderDevTracker.Data.ApplicationDbContext>()
                .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
                .AddSignInManager()
                .AddDefaultTokenProviders();


            return services;
        }
    }
}
