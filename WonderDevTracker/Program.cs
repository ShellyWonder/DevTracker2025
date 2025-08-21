using System.Reflection;
using MudBlazor;
using MudBlazor.Services;
using Scalar.AspNetCore;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client.Helpers.Animation;
using WonderDevTracker.Client.Services;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Components;
using WonderDevTracker.Components.Account;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services;
using WonderDevTracker.Services.Interfaces;
using WonderDevTracker.Services.Repositories;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSwaggerGen(options =>
{
    // authentication scheme = cookies
    options.AddSecurityDefinition("cookie", new OpenApiSecurityScheme
    {
        Name = "AspNetCore.Identity.Application",
        Description = "Cookie used for authentication",
        In = ParameterLocation.Cookie,
        Type = SecuritySchemeType.Http,
        Scheme = "cookie",
    });
    //display which endpoints require authentication
    options.OperationFilter<SecurityRequirementsOperationFilter>();
    // Include XML comments for Swagger documentation
    var XMLFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, XMLFileName));

    options.SwaggerDoc("v1", new() { Title = "Wonder Dev Tracker API", Version = "v1" });

    //exclude docs for built-in Identity Razor components
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Exclude Identity Razor components from Swagger documentation
        if (apiDesc.ActionDescriptor.RouteValues.TryGetValue("area", out var area) && area == "Identity")
        {
            return false;
        }
        return true;
    });


    //Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey, saving for when I configure JWT
    //options.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddScoped<IProjectRepository , ProjectRepository>();
builder.Services.AddScoped<IProjectDTOService, ProjectDTOService>();

builder.Services.AddControllers();
builder.Services.AddHttpClient();
//to cache images in the browser
builder.Services.AddOutputCache();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
}
    
    );
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ThemeManagerService>();
builder.Services.AddScoped<IndexTrackerHelper>();
builder.Services.AddSingleton<IProjectPatchBuilder, ProjectPatchBuilder>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = DataUtility.GetConnectionString(builder.Configuration) ?? throw new InvalidOperationException("Connection string 'DbConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        ));


builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

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
app.MapScalarApiReference();
app.MapGet("/hello", () => "Hello world!");
app.MapGet("/debug/swagger", () => Results.Redirect("/openapi/v1.json"));


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
