using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WonderDevTracker.Client.Infrastructure;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//Register repositories and domain services
builder.Services.AddRepositoriesAndDomain();

//Register UI components and utilities including HTTP client and mudblazor
builder.Services.AddUiAndUtilities();

//Register identity services
builder.Services.AddIdentityServices();

await builder.Build().RunAsync();
