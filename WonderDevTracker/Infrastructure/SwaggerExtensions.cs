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

                options.SwaggerDoc("v1", new() { Title = "Wonder Dev Tracker API", Version = "v1" });

                //exclude docs for built-in Identity Razor components
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (apiDesc.ActionDescriptor.RouteValues.TryGetValue("area", out var area) && area == "Identity")
                        return false;
                    return true;
                });
            });

            return services;
        }


        //public static IApplicationBuilder UseApiDocumentation(this IApplicationBuilder app)
        //{
        //    app.UseSwagger(options => options.RouteTemplate = "/openapi/{documentName}.json");
        //    //Create documentation page at URL: /scalar/v1
           
        //    return app;

        //}
    }

}