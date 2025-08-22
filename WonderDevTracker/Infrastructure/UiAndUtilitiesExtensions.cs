using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using WonderDevTracker.Client.Helpers.Animation;
using WonderDevTracker.Client.Services;
using WonderDevTracker.Components.Account;

namespace WonderDevTracker.Infrastructure
{
    public static class UiAndUtilitiesExtensions
    {
        public static IServiceCollection AddUiAndUtilities(this IServiceCollection services)
        {
            services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
            }

            );
            services.AddBlazoredLocalStorage();
            services.AddControllers();
            services.AddHttpClient();
            //to cache images in the browser
            services.AddOutputCache();

            services.AddCascadingAuthenticationState();
            services.AddScoped<IdentityUserAccessor>();
            services.AddScoped<IdentityRedirectManager>();
            services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();


            services.AddScoped<ThemeManagerService>();
            services.AddScoped<IndexTrackerHelper>();

            return services;

        }
    }
}