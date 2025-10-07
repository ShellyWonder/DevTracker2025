using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;
using WonderDevTracker.Client.Helpers.Animation;
using WonderDevTracker.Client.Services;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Infrastructure
{
    public static class UiAndUtilitiesExtensions
    {
        public static IServiceCollection AddUiAndUtilities(this IServiceCollection services)
        {
            // Register UI components and utilities here
            services.AddBlazoredLocalStorage();
            services.AddScoped<ThemeManagerService>();
            services.AddScoped<IndexTrackerHelper>();
            services.AddScoped<IAppAuthorizationService, AppAuthorizationService>();

            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
            });

            services.AddScoped(sp =>
            new HttpClient { BaseAddress = new Uri(sp.GetRequiredService<NavigationManager>().BaseUri) });

            return services;
        }
    }
}
