using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
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

        public async Task<IEnumerable<Ticket>> GetTicketsAssignedToUserAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            List<Ticket> tickets = new();

            if (userInfo.IsInRole(Role.ProjectManager))
            {

                List<int> AssignedProjectIds = await db.Users
                    //get the user
                    .Where(u => u.Id == userInfo.UserId)
                    //include their projects
                    .SelectMany(u => u.Projects!)
                    //get only the project ids
                    .Select(p => p.Id)
                    //make it a list for C# use
                    .ToListAsync();

                //get all tickets in those projects
                tickets = await db.Tickets
                    .Include(t => t.Project)
                    .Include(t => t.SubmitterUser)
                    .Include(t => t.DeveloperUser)
                    .Where(t => !t.Archived)
                    .Where(t => t.SubmitterUserId == userInfo.UserId  //ticket submitter
                                   || t.DeveloperUserId == userInfo.UserId //developer assingned to ticket
                                   || AssignedProjectIds.Contains(t.ProjectId)) //ticket is in a project assigned to the PM
                                        .ToListAsync();
            }

            else
            {
                //get all tickets user submitted || all tickets assigned to user
                tickets = await db.Tickets
                    .Include(t => t.Project)
                    .Include(t => t.SubmitterUser)
                    .Include(t => t.DeveloperUser)
                    .Where(t => !t.Archived)
                    .Where(t => t.SubmitterUserId == userInfo.UserId
                                   || t.DeveloperUserId == userInfo.UserId)
                    .ToListAsync();
            }
            return tickets;
        }
    }
}
