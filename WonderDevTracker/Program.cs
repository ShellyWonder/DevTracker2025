using WonderDevTracker.Data;
using WonderDevTracker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddUiAndUtilities(); // Mud, LocalStorage, IndexTracker, ThemeManager, RazorComponents
builder.Services.AddPersistence(builder.Configuration);// Register the database context and other persistence-related services
builder.Services.AddIdentityAndAuth(); // Register Identity services and authentication middleware
builder.Services.AddRepositoriesAndDomain(); // Register repositories and domain services
builder.Services.AddApiDocumentation(); // SwaggerGen + Scalar configuration for API documentation 

var app = builder.Build();
//seed the database with initial data from the DataUtility class
using (var scope = app.Services.CreateScope())
{
    await DataUtility.ManageDataAsync(scope.ServiceProvider);
}

//middleware pipeline configuration
app.UseDevTrackerPipeline();
//map endpoints 
app.MapDevTrackerEndpoints();

app.Run();
