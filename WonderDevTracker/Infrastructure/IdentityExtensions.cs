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
            .AddIdentityCookies();

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
