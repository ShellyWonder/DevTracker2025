using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace WonderDevTracker.Infrastructure
{

    //SwaggerGen and Scalar configurations for the API project documentation and authentication
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
            // authentication scheme = cookies
            //Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey, saving for when I configure JWT

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

            options.SwaggerDoc("v1", new()
            {
                Title = "Wonder Dev Tracker API",
                Version = "v1",
                Description = """
                                                                        Wonder Dev Tracker is a project management tool designed to help developers and teams 
                                                                        track their work, manage tasks, and collaborate effectively. 
                                                                        This API provides endpoints for managing projects, tasks, and user accounts 
                                                                        within the application.

                                                                        This API uses cookie-based authentication. To access secured endpoints, you must first log in 
                                                                        through the application's login page to set a cookie in your browser. 
                                                                        A Demo User account is available for testing purposes.
                                                                        
                                                                        After login, you will be able to make requests to the API endpoints
                                                                        by pressing the "Test Request" buttons below. 
                                                                        
                                                                        
                                                       
                                                                        """,
                Contact = new()
                {
                    Name = "SJ Wonder",
                    Email = "SJ@DevTracker.com",//replace email b4 deployment
                    Url = new Uri("http://wonderdev.com") //replace with your website URL b4 deployment
                },
            });

                //TODO: Add Logo and favicon to Swagger UI page

                //exclude docs for built-in Account endpoints in Identity
                options.DocInclusionPredicate((docName, apiDesc) =>
            apiDesc.RelativePath is null || !apiDesc.RelativePath.StartsWith("Account"));

            });

            return services;
        }

       
        public static IApplicationBuilder UseApiDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger(options => options.RouteTemplate = "/openapi/{documentName}.json");
            
            return app;
        }
    }

}