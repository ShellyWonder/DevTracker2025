using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;
using WonderDevTracker.Services.RepoBuilders;

namespace WonderDevTracker.Services.Repositories
{
    public partial class DashboardRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
                                             UserManager<ApplicationUser> userManager) : IDashboardRepository
    {
        
        public async Task<DashboardDTO> GetDashboardDataAsync(UserInfo userInfo)
        {
            await using var context = contextFactory.CreateDbContext();

            DashboardDTO dashboard = new()
            {
                CompanyInfo = await GetCompanyInfoAsync(context, userInfo.CompanyId),
                AdminDashboard = await GetAdminDashboardDataAsync(context, userInfo),
                PMDashboard = await GetPMDashboardDataAsync(context, userInfo.CompanyId, userInfo.UserId, userManager),
                DevDashboard = await GetDeveloperDashboardDataAsync(context, userInfo.CompanyId, userInfo.UserId),
                SubmitterDashboard = await GetSubmitterDashboardDataAsync(context, userInfo.CompanyId, userInfo.UserId),

            };

            return dashboard;
        }

        #region Role-Specific Dashboard Aggregation Methods

        #region Admin Dashboard
        private static async Task<AdminDashboardDTO> GetAdminDashboardDataAsync(ApplicationDbContext context, UserInfo userInfo)
        {

            IQueryable<Project> allCompanyProjects = GetCompanyProjectsQuery(context, userInfo.CompanyId);
            IQueryable<Ticket> allCompanyTickets = GetCompanyTicketsQuery(context, userInfo.CompanyId);
            IQueryable<Ticket> adminCompanyTickets = GetAdminCompanyTicketsQuery(context, userInfo.CompanyId);

            return new AdminDashboardDTO
            {
                CompanyStats = await GetCompanyDashboardStatsAsync(allCompanyProjects, allCompanyTickets),
                RecentActiveTickets = await GetRecentTicketSummariesAsync(
                                       GetRecentActiveTicketsQuery(adminCompanyTickets)),
                RecentResolvedTickets = await GetRecentTicketSummariesAsync(
                                        GetRecentResolvedTicketsQuery(adminCompanyTickets)),
                RecentUnassignedTickets = await GetRecentTicketSummariesAsync(
                                        GetRecentUnassignedTicketsQuery(adminCompanyTickets)),
                ChartData = await DashboardChartDataBuilder.GetDashboardChartDataAsync(context, userInfo.CompanyId),
                RecentProjects = await GetProjectSummariesAsync(GetActiveCompanyProjectsQuery(context, userInfo.CompanyId)),
                MySubmittedTickets = await GetMySubmittedTicketsAsync(context, userInfo.CompanyId, userInfo.UserId)

            };
        }
        #endregion

        #region PM Dashboard
        private static async Task<PMDashboardDTO> GetPMDashboardDataAsync(ApplicationDbContext context,
                                                                          int companyId, string userId,
                                                                          UserManager<ApplicationUser> userManager)
        {
            IQueryable<Project> pmProjects = GetPMProjectsQuery(context, companyId, userId);
            IQueryable<Ticket> pmTickets = GetPMProjectTicketsQuery(context, companyId, userId);

            return new PMDashboardDTO
            {
                PMStats = await GetPMDashboardStatsAsync(pmProjects, pmTickets),
                ManagedProjects = await GetProjectSummariesAsync(pmProjects),
                UnassignedTickets = await GetPMUnassignedTicketsAsync(pmTickets),
                PMChartData = await DashboardChartDataBuilder.GetPMDashboardChartDataAsync(pmProjects, pmTickets),
                TeamMembers = await GetPMTeamMembersAsync(context, companyId, userId, userManager),
                MySubmittedTickets = await GetMySubmittedTicketsAsync(context, companyId, userId)
            };
        }

        #endregion

        #region Developer Dashboard
        private static async Task<DeveloperDashboardDTO> GetDeveloperDashboardDataAsync(
                                                                    ApplicationDbContext context,
                                                                    int companyId,
                                                                    string userId)
        {
            var devTickets = GetDeveloperTicketsQuery(context, companyId, userId);


            return new DeveloperDashboardDTO
            {
                DevStats = await GetDeveloperDashboardStatsAsync(devTickets),
                DevProjects = await GetProjectSummariesAsync(GetDeveloperProjectsQuery(context, companyId, userId)),
                AssignedTickets = await GetDeveloperAssignedTicketSummariesAsync(devTickets),
                DevChartData = await DashboardChartDataBuilder.GetDeveloperDashboardChartDataAsync(devTickets),
                MySubmittedTickets = await GetMySubmittedTicketsAsync(context, companyId, userId)
            };
        }

         #endregion

        #region Submitter Dashboard
        private static async Task<SubmitterDashboardDTO> GetSubmitterDashboardDataAsync(ApplicationDbContext context, int companyId, string userId)
        {
            return new SubmitterDashboardDTO
            {
                SubmitterStats = await GetSubmitterDashboardStatsAsync(GetSubmitterTicketsQuery(context, companyId, userId)),
                MySubmittedTickets = await GetMySubmittedTicketsAsync(context, companyId, userId)
            };
        }
        #endregion

          #endregion

    }
}
