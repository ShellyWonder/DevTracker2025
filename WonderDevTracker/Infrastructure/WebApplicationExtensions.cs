using Scalar.AspNetCore;
using WonderDevTracker.Components;

namespace WonderDevTracker.Infrastructure
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseDevTrackerPipeline(this WebApplication app)
        {
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
            // Enable middleware to serve generated Swagger as a JSON endpoint and the Swagger UI
            app.UseApiDocumentation();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            // Redirect missing PNG attachments to default image
            //current use in TicketAttachment component
            app.Use(async (context, next) =>
            {
                 await next(context);
                if (context.Request.Path.StartsWithSegments("/img/attachmentsPNGs") && context.Response.StatusCode == 404) 
                {
                    context.Response.Redirect("/img/attachmentPNGs/png.default.png");
                }
            });
            app.UseOutputCache();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();
            return app;
        }
        public static WebApplication MapDevTrackerEndpoints(this WebApplication app)
        {
            // Test endpoints for development;
            // TODO: remove in production
            app.MapGet("/hello", () => "Hello world!");
            app.MapGet("/debug/swagger", () => Results.Redirect("/openapi/v1.json"));

            app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(WonderDevTracker.Client._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();
            app.MapControllers();

            // Scalar UI (now mapped here, not in UseApiDocumentation)
            app.MapScalarApiReference(options =>
            {
                options.WithFavicon("/favicon.ico")
                       .WithTitle("API Specifications | Dev Tracker")
                       .WithTheme(ScalarTheme.BluePlanet);
            });
            return app;
        }
    }
}
