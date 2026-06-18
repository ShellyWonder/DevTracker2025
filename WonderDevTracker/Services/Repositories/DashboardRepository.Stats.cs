//DashboardRepository.Stats.cs
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Repositories
{
    public partial class DashboardRepository
    {
        #region Role-Specific Stats Calculators
        private static async Task<CompanyDashboardStatsDTO> GetCompanyDashboardStatsAsync(
                                                                IQueryable<Project> companyProjects,
                                                                IQueryable<Ticket> companyTickets)
        {
            return new CompanyDashboardStatsDTO
            {
                TotalProjectCount = await companyProjects.CountAsync(),
                TotalTicketCount = await companyTickets.CountAsync(),
                OpenTicketCount = await companyTickets
                                        .CountAsync(t => t.Status != TicketStatus.Resolved),
                ResolvedTicketCount = await companyTickets
                                        .CountAsync(t => t.Status == TicketStatus.Resolved)
            };
        }

        private static async Task<PMDashboardStatsDTO> GetPMDashboardStatsAsync(
                                                                IQueryable<Project> pmProjects,
                                                                IQueryable<Ticket> pmTickets)
        {
            return new PMDashboardStatsDTO
            {
                ManagedProjectCount = await pmProjects.CountAsync(),
                ActiveManagedTicketCount = await pmTickets.CountAsync(),
                OpenManagedTicketCount = await pmTickets
                                        .CountAsync(t => t.Status != TicketStatus.Resolved),
                ResolvedManagedTicketCount = await pmTickets
                                        .CountAsync(t => t.Status == TicketStatus.Resolved)
            };
        }
        private static async Task<DevDashboardStatsDTO> GetDeveloperDashboardStatsAsync(IQueryable<Ticket> devTickets)
        {
            return new DevDashboardStatsDTO
            {
                AssignedTicketCount = await devTickets.CountAsync(),
                OpenAssignedTicketCount = await devTickets
                    .CountAsync(t => t.Status != TicketStatus.Resolved),
                InProgressCount = await devTickets
                    .CountAsync(t =>
                        t.Status == TicketStatus.InDevelopment ||
                        t.Status == TicketStatus.InTesting),
                ResolvedCount = await devTickets
                    .CountAsync(t => t.Status == TicketStatus.Resolved)
            };
        }
        private static async Task<SubmitterDashboardStatsDTO> GetSubmitterDashboardStatsAsync(
                                                                IQueryable<Ticket> submitterTickets)
        {
            return new SubmitterDashboardStatsDTO
            {
                SubmittedTicketCount = await submitterTickets.CountAsync(),
                OpenSubmittedTicketCount = await submitterTickets
                                                .CountAsync(t => t.Status != TicketStatus.Resolved),
                ResolvedSubmittedTicketCount = await submitterTickets
                                                .CountAsync(t => t.Status == TicketStatus.Resolved)
            };
        }
        #endregion
    }
}
