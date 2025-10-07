
using WonderDevTracker.Client.Services;

using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoriesAndDomain(this IServiceCollection services)
        {
            
            services.AddSingleton<IProjectPatchBuilder, ProjectPatchBuilder>();
            services.AddScoped<IProjectDTOService, WASMProjectDTOService>();
            services.AddScoped<ICompanyDTOService, WASMCompanyDTOService>();
            services.AddScoped<ITicketDTOService, WASMTicketDTOService>();
            services.AddScoped<IAppAuthorizationService, AppAuthorizationService>();

            return services;
        }
    }
}
