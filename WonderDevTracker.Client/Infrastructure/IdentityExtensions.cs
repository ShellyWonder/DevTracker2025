using Microsoft.AspNetCore.Components.Authorization;

namespace WonderDevTracker.Client.Infrastructure
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            // Register identity services here
            services.AddAuthorizationCore();
            services.AddCascadingAuthenticationState();
            services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

            return services;
        }
    }
}
