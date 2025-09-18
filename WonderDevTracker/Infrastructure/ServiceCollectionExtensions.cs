using Microsoft.AspNetCore.Identity;
using WonderDevTracker.Client.Services;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Components.Account;
using WonderDevTracker.Models;
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
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IProjectDTOService, ProjectDTOService>();
            services.AddScoped<ICompanyDTOService, CompanyDTOService>();
            services.AddScoped<ITicketDTOService, TicketDTOService>();
            // No-op email sender for development purposes, replace with a real implementation in production
            services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
            services.AddSingleton<IProjectPatchBuilder, ProjectPatchBuilder>();
            return services;
        }
    }
}
