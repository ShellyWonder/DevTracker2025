using WonderDevTracker.Client.Helpers.Animation;
using WonderDevTracker.Client.Services;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Services;
using WonderDevTracker.Services.Interfaces;
using WonderDevTracker.Services.Repositories;

namespace WonderDevTracker.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoriesAndDomain(this IServiceCollection services)
        {
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectDTOService, ProjectDTOService>();
            
            services.AddSingleton<IProjectPatchBuilder, ProjectPatchBuilder>();
            return services;
        }
    }
}
