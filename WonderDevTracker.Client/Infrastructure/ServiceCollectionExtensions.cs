
using WonderDevTracker.Client.Services;

using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoriesAndDomain(this IServiceCollection services)
        {
            
            services.AddSingleton<IProjectPatchBuilder, ProjectPatchBuilder>();

            return services;
        }
    }
}
