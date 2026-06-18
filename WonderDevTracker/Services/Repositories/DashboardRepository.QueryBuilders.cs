//DashboardRepository.QueryBuilders.cs

using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Repositories
{
    public partial class DashboardRepository
    {
        #region ADMIN QUERIES
        public static IQueryable<Project> GetCompanyProjectsQuery(ApplicationDbContext context, int companyId)
        {
            return context.Projects
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId);
        }
        public static IQueryable<Project> GetActiveCompanyProjectsQuery(ApplicationDbContext context, int companyId)
        {
            return GetCompanyProjectsQuery(context, companyId)
                .Where(p => !p.Archived);
        }

        public static IQueryable<Ticket> GetCompanyTicketsQuery(ApplicationDbContext context, int companyId)
        {
            return context.Tickets
                .AsNoTracking()
                .Where(t => t.Project!.CompanyId == companyId);
        }
        #endregion

        #region PM QUERIES
        public static IQueryable<Project> GetPMProjectsQuery(ApplicationDbContext context, int companyId, string userId)
        {
            return context.Projects
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId && p.Members!.Any(m => m.Id == userId) && !p.Archived);
        }

        public static IQueryable<Ticket> GetPMProjectTicketsQuery(ApplicationDbContext context, int companyId, string userId)
        {
            return context.Tickets
                .AsNoTracking()
                .Where(t => t.Project!.CompanyId == companyId &&
                !t.Archived &&
                !t.Project!.Archived &&
                t.Project!.Members!.Any(m => m.Id == userId));
        }
        #endregion

        #region DEVELOPER QUERIES
        public static IQueryable<Project> GetDeveloperProjectsQuery(ApplicationDbContext context, int companyId, string userId)
        {
            return context.Projects
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId && p.Members!.Any(m => m.Id == userId) && !p.Archived);
        }

        public static IQueryable<Ticket> GetDeveloperTicketsQuery(ApplicationDbContext context, int companyId, string userId)
        {
            return context.Tickets
                     .AsNoTracking()
                     .Where(t =>
                             t.Project != null &&
                             t.Project.CompanyId == companyId &&
                             !t.Archived &&
                             !t.ArchivedByProject &&
                             !t.Project.Archived &&
                             t.DeveloperUserId == userId);
        }
        #endregion

        #region SUBMITTER QUERIES
        public static IQueryable<Ticket> GetSubmitterTicketsQuery(ApplicationDbContext context, int companyId, string userId)
        {
            return context.Tickets
                .AsNoTracking()
                .Where(t => t.Project!.CompanyId == companyId &&
                !t.Archived &&
                !t.Project!.Archived &&
                t.SubmitterUserId == userId);
        }
        #endregion

        #region TICKET QUERIES
        public static IQueryable<Ticket> GetAdminCompanyTicketsQuery(ApplicationDbContext context, int companyId)
        {
            return context.Tickets
                .AsNoTracking()
                .Where(t => t.Project!.CompanyId == companyId &&
                !t.Archived &&
                !t.Project!.Archived);
        }
        public static IQueryable<Ticket> GetRecentActiveTicketsQuery(IQueryable<Ticket> tickets)
        {
            return tickets.Where(t => t.Status != TicketStatus.Resolved);
        }
        public static IQueryable<Ticket> GetRecentResolvedTicketsQuery(IQueryable<Ticket> tickets)
        {
            return tickets.Where(t => t.Status == TicketStatus.Resolved);
        }
        public static IQueryable<Ticket> GetRecentUnassignedTicketsQuery(IQueryable<Ticket> tickets)
        {
            return tickets.Where(t => t.DeveloperUserId == null)
                .Where(t => t.Status != TicketStatus.Resolved)
                .Where(t => !t.Archived)
                .Where(t => !t.Project!.Archived);
        }
        #endregion
    }
}
