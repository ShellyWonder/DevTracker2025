using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using WonderDevTracker.Client.Services;
using WonderDevTracker.Client.Services.Interfaces;
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
            services.AddScoped<ITicketRepository, TicketRepository>();

            services.AddScoped<IInviteRepository, InviteRepository>();
            services.AddScoped<IInviteDTOService, InviteDTOService>();

            // MailGun for development purposes, replace with a real implementation in production
            services.AddSingleton<IEmailSender<ApplicationUser>, MailGunEmailSender>();  //use in identity pages
            services.AddSingleton<IEmailSender, MailGunEmailSender>();// use for application contact emails

            services.AddSingleton<IProjectPatchBuilder, ProjectPatchBuilder>();
            return services;
        }
    }
}
