using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers.Animation;
using WonderDevTracker.Client.Services;
using WonderDevTracker.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ThemeManagerService>();
builder.Services.AddScoped<IndexTrackerHelper>();
builder.Services.AddSingleton<IProjectPatchBuilder, ProjectPatchBuilder>();



await builder.Build().RunAsync();
