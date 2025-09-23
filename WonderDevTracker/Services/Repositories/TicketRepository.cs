using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class TicketRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : ITicketRepository
    {
        public async Task<IEnumerable<Ticket>> GetArchivedTicketsAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            IEnumerable<Ticket> tickets = await db.Tickets
                     //match the company id of the user 
                     .Where(t => t.Project!.CompanyId == userInfo.CompanyId
                       && t.Archived)
                        .Include(t => t.Project)
                        .Include(t => t.SubmitterUser)
                        .Include(t => t.DeveloperUser)
                        .ToListAsync();
            return tickets;
        }
        public async Task<IEnumerable<Ticket>> GetOpenTicketsAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            IEnumerable<Ticket> tickets = await db.Tickets
                     //match the company id of the user & also ensure the project is not archived
                     .Where(t => t.Project!.CompanyId == userInfo.CompanyId
                       && !t.Archived
                       && t.Status != TicketStatus.Resolved)
                     .Include(t => t.Project)
                     .Include(t => t.SubmitterUser)
                     .Include(t => t.DeveloperUser)
                     .ToListAsync();
            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetResolvedTicketsAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();

            IEnumerable<Ticket> tickets = await db.Tickets
                     .Include(t => t.Project)
                     .Include(t => t.SubmitterUser)
                     .Include(t => t.DeveloperUser)
                      .Where(t => t.Project!.CompanyId == userInfo.CompanyId
                        && !t.Archived
                        && t.Status == TicketStatus.Resolved)
                     .ToListAsync();
            return tickets;
        }
    }
}
