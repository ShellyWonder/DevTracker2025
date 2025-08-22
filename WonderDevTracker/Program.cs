using Scalar.AspNetCore;
using WonderDevTracker.Components;
using WonderDevTracker.Data;
using WonderDevTracker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddUiAndUtilities(); // Mud, LocalStorage, IndexTracker, ThemeManager, RazorComponents
builder.Services.AddPersistence(builder.Configuration);// Register the database context and other persistence-related services
builder.Services.AddIdentityAndAuth(); // Register Identity services and authentication middleware
builder.Services.AddRepositoriesAndDomain(); // Register repositories and domain services
builder.Services.AddApiDocumentation(); // SwaggerGen + Scalar configuration for API documentation and authentication


var app = builder.Build();
//seed the database with initial data from the DataUtility class
using (var scope = app.Services.CreateScope())
{
    await DataUtility.ManageDataAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseRouting();
app.UseSwagger(options => options.RouteTemplate = "/openapi/{documentName}.json");
//Create documentation page at URL: /scalar/v1
app.MapScalarApiReference( options =>
  {
      // Set the favicon for the API documentation
      options.WithFavicon("/favicon.ico")
              .WithTitle("API Specifications | Dev Tracker")
              // Set the theme for the API documentation
              .WithTheme(ScalarTheme.BluePlanet); 
  });



app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WonderDevTracker.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();
app.MapControllers();
app.Run();
