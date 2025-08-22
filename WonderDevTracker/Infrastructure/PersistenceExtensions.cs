using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Data;

namespace WonderDevTracker.Infrastructure
{
    
    public static class PersistenceExtensions
    {
        /// <summary>
        /// Used to house the db context registration for dependency injection
        /// </summary>
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = DataUtility.GetConnectionString(config) ?? throw new InvalidOperationException("Connection string 'DbConnection' not found.");
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    ));


            return services;
        }
    }
}
