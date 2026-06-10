//CompanyRepository.DashboardCharts.cs

using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Repositories
{
    public partial class CompanyRepository
    {

        #region CHART AGGREGATOR METHODS 

        #region Company/Admin Dashboard Chart Data
        //calls individual queries and packages into single DTO for dashboard consumption
        //
        private static async Task<DashboardChartDataDTO> GetDashboardChartDataAsync(
                                                        ApplicationDbContext context,
                                                        int companyId)
        {
            return new DashboardChartDataDTO
            {
                TicketsOverTimeChart = await GetTicketsOverTimeDataAsync(context, companyId),
                TicketsByStatus = await GetTicketsByStatusDataAsync(context, companyId),
                TicketsByPriority = await GetTicketsByPriorityDataAsync(context, companyId),
                ProjectsByPriority = await GetProjectsByPriorityDataAsync(context, companyId),
                TicketsByProject = await GetTicketsByProjectDataAsync(context, companyId)
            };

        }
        #endregion

        #region Project Manager Dashboard Chart Data
        private static async Task<DashboardChartDataDTO> GetPMDashboardChartDataAsync(
                                                        IQueryable<Project> pmProjects,
                                                        IQueryable<Ticket> pmTickets)
        {
            return new DashboardChartDataDTO
            {
                TicketsByStatus = await GetTicketsByStatusDataAsync(pmTickets),
                TicketsByPriority = await GetTicketsByPriorityDataAsync(pmTickets),
                ProjectsByPriority = await GetProjectsByPriorityDataAsync(pmProjects),
                TicketsByProject = await GetTicketsByProjectDataAsync(pmTickets)

            };
        }
        #endregion
        #region Developer Dashboard Chart Data
        private static async Task<DashboardChartDataDTO> GetDeveloperDashboardChartDataAsync(
                                                        IQueryable<Ticket> devTickets)
        {
            return new DashboardChartDataDTO
            {
                TicketsByStatus = await GetDeveloperTicketsByStatusDataAsync(devTickets),
                TicketsByPriority = await GetDeveloperTicketsByPriorityDataAsync(devTickets),
                ProjectsByPriority = [],
                TicketsByProject = [],
                TicketsOverTimeChart = new DashboardTicketsOverTimeChartDTO()
            };
        }
        #endregion
        #region Stats methods for PM and Dev dashboards 
        private static async Task<List<DashboardTicketsByProjectDTO>> GetTicketsByProjectDataAsync(IQueryable<Ticket> pmTickets)
        {
            return await pmTickets
                   .Where(t => t.Project != null)
                   .GroupBy(t => new
                   {
                       t.ProjectId,
                       ProjectName = t.Project!.Name
                   })
                   .Select(g => new DashboardTicketsByProjectDTO
                   {
                       ProjectId = g.Key.ProjectId,
                       ProjectName = g.Key.ProjectName ?? "Unknown Project",
                       OpenTicketCount = g.Count(t => t.Status != TicketStatus.Resolved)
                   })
                   .OrderByDescending(x => x.OpenTicketCount)
                   .ThenBy(x => x.ProjectName)
                     .ToListAsync();
        }

        private static async Task<List<DashboardEnumCountDTO<ProjectPriority>>> GetProjectsByPriorityDataAsync(IQueryable<Project> pmProjects)
        {
            return await GetCountByCategoryAsync<ProjectPriority>(
                        pmProjects.Select(p => (ProjectPriority?)p.Priority));
        }

        private static async Task<List<DashboardEnumCountDTO<TicketPriority>>> GetTicketsByPriorityDataAsync(IQueryable<Ticket> pmTickets)
        {
            return await GetCountByCategoryAsync<TicketPriority>(
                        pmTickets.Select(t => (TicketPriority?)t.Priority));
        }

        private static async Task<List<DashboardEnumCountDTO<TicketStatus>>> GetTicketsByStatusDataAsync(IQueryable<Ticket> pmTickets)
        {
            return await GetCountByCategoryAsync<TicketStatus>(
                        pmTickets.Select(t => (TicketStatus?)t.Status));
        }
        private static async Task<List<DashboardEnumCountDTO<TicketStatus>>> GetDeveloperTicketsByStatusDataAsync(IQueryable<Ticket> devTickets)
        {
            List<DashboardEnumCountDTO<TicketStatus>> data = await GetCountByCategoryAsync<TicketStatus>(
                        devTickets.Select(t => (TicketStatus?)t.Status));
            return data;
        }
        private static async Task<List<DashboardEnumCountDTO<TicketPriority>>> GetDeveloperTicketsByPriorityDataAsync(IQueryable<Ticket> devTickets)
        {
            List<DashboardEnumCountDTO<TicketPriority>> data = await GetCountByCategoryAsync<TicketPriority>(
                        devTickets.Select(t => (TicketPriority?)t.Priority));
            return data;
        }
        #endregion
        #endregion


        private static async Task<DashboardTicketsOverTimeChartDTO> GetTicketsOverTimeDataAsync(ApplicationDbContext context, int companyId)
        {
            List<DashboardMonthlyTicketsDTO> ticketsOverTime = [];
            List<DashboardMonthlyTicketsDTO> resolvedTicketsOverTime = [];

            IQueryable<Ticket> allCompanyTickets = GetCompanyTicketsQuery(context, companyId);

            //1.Calculate the date range for the past 12 months
            DateTimeOffset now = DateTimeOffset.UtcNow;

            //loop over the previous 12 months
            for (int monthsAgo = 11; monthsAgo >= 0; monthsAgo--)
            {
                int year = now.Year;
                int month = now.Month - monthsAgo;

                if (month <= 0)
                {
                    year -= 1;
                    month += 11;
                }
                //Set to first day of month for consistent grouping
                var thisMonth = new DateTimeOffset(
                    year,
                    month,
                    day: 1,
                    hour: 0,
                    minute: 0,
                    second: 0,
                    TimeSpan.Zero);

                //Set the end of the month by adding 1 month 
                var nextMonth = new DateTimeOffset(
                    year: month == 11 ? year + 1 : year,
                    month: month == 11 ? 1 : month + 1,
                    day: 1,
                    hour: 0,
                    minute: 0,
                    second: 0,
                    TimeSpan.Zero
                    );

                //query db for tickets created in that month
                var createdCount = await allCompanyTickets.CountAsync(t => t.Created >= thisMonth && t.Created < nextMonth);

                //query db for tickets resolved and updatedin that month
                var resolvedCount = await allCompanyTickets.Where(t => t.Status == TicketStatus.Resolved)
                                                               .CountAsync(t => t.Updated.HasValue
                                                               ? (t.Updated.Value >= thisMonth && t.Updated.Value < nextMonth)
                                                               : (t.Created >= thisMonth && t.Created < nextMonth)
                                                                );
                //ignore first month if no data to show on chart
                if (ticketsOverTime.Count > 0 || resolvedTicketsOverTime.Count > 0 || createdCount > 0 || resolvedCount > 0)
                {
                    ticketsOverTime.Add(new DashboardMonthlyTicketsDTO
                    {
                        Month = thisMonth,
                        Count = createdCount
                    });
                    resolvedTicketsOverTime.Add(new DashboardMonthlyTicketsDTO
                    {
                        Month = thisMonth,
                        Count = resolvedCount
                    });
                }
            }

            return new DashboardTicketsOverTimeChartDTO
            {
                TicketsOverTime = ticketsOverTime,
                ResolvedTicketsOverTime = resolvedTicketsOverTime
            };
        }

        private static async Task<List<DashboardEnumCountDTO<TicketStatus>>> GetTicketsByStatusDataAsync(
                                ApplicationDbContext context,
                                int companyId)
        {
            IQueryable<Ticket> allCompanyTickets = GetCompanyTicketsQuery(context, companyId);

            List<DashboardEnumCountDTO<TicketStatus>> data = await GetCountByCategoryAsync<TicketStatus>(
       allCompanyTickets.Select(t => (TicketStatus?)t.Status));

            return [.. data.OrderBy(item => item.Value)];
        }

        private static async Task<List<DashboardEnumCountDTO<ProjectPriority>>> GetProjectsByPriorityDataAsync(
                        ApplicationDbContext context,
                        int companyId)
        {
            IQueryable<Project> allCompanyProjects = GetCompanyProjectsQuery(context, companyId);

            List<DashboardEnumCountDTO<ProjectPriority>> data = await GetCountByCategoryAsync<ProjectPriority>(
                allCompanyProjects.Select(p => (ProjectPriority?)p.Priority));

            return [.. data.OrderBy(item => item.Value)];
        }

        private static async Task<List<DashboardEnumCountDTO<TicketPriority>>> GetTicketsByPriorityDataAsync(
                                ApplicationDbContext context,
                                int companyId)
        {
            IQueryable<Ticket> allCompanyTickets = GetCompanyTicketsQuery(context, companyId);

            List<DashboardEnumCountDTO<TicketPriority>> data = await GetCountByCategoryAsync<TicketPriority>(
        allCompanyTickets.Select(t => (TicketPriority?)t.Priority));

            return [.. data.OrderBy(item => item.Value)];
        }

        private static async Task<List<DashboardTicketsByProjectDTO>> GetTicketsByProjectDataAsync(
                                                                        ApplicationDbContext context,
                                                                        int companyId)

        {
            //var cutoff = DateTimeOffset.UtcNow.AddDays(-daysBack);
            return await GetAdminCompanyTicketsQuery(context, companyId)
            //    .Where(t => t.Created >= cutoff)
                .GroupBy(t => new
                {
                    t.ProjectId,
                    ProjectName = t.Project!.Name
                })
                .Select(g => new DashboardTicketsByProjectDTO
                {
                    ProjectId = g.Key.ProjectId,
                    ProjectName = g.Key.ProjectName ?? "Unnamed Project",
                    TotalTicketCount = g.Count(),
                    OpenTicketCount = g.Count(t => t.Status != TicketStatus.Resolved),
                    ResolvedTicketCount = g.Count(t => t.Status == TicketStatus.Resolved)
                })
                .OrderByDescending(x => x.TotalTicketCount)
                .ToListAsync();
        }

        #region Helper Methods
        private static async Task<List<DashboardEnumCountDTO<TEnum>>> GetCountByCategoryAsync<TEnum>(
            IQueryable<TEnum?> query)
            where TEnum : struct, Enum
        {
            var groupedData = await query
        .GroupBy(e => e)
        .Select(g => new
        {
            Value = g.Key,
            Count = g.Count()
        })
        .ToListAsync();

            return [.. groupedData
                .Select(d => new DashboardEnumCountDTO<TEnum>
                {
                    Value = d.Value,
                    Label = d.Value.HasValue
                ? d.Value.Value.GetDisplayName()
                : "Unspecified",
                    Count = d.Count
                })];
        }

        #endregion


    }
}
