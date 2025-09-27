using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class TicketRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
                                   UserManager<ApplicationUser> userManager) : ITicketRepository
    {
        public async Task<Ticket?> AddTicketAsync(Ticket ticket, UserInfo userInfo)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            //ensure the project is valid & belongs to the user's company
            Project? project = await db.Projects
                .Include(p => p.Members)
                 .FirstOrDefaultAsync(p => p.Id == ticket.ProjectId
                  && p.CompanyId == userInfo.CompanyId) ?? 
                             throw new InvalidOperationException("Invalid project."); 

            ticket.Created = DateTimeOffset.UtcNow;
            ticket.Status = TicketStatus.New;
            ticket.SubmitterUserId = userInfo.UserId;


            ApplicationUser? developer = null;
            if (!string.IsNullOrEmpty(ticket.DeveloperUserId))
            {
                bool IsManagerOfProject = userInfo.IsInRole(Role.ProjectManager)
                    && project!.Members!.Any(m => m.Id == userInfo.UserId);

                if (userInfo.IsInRole(Role.Admin) || IsManagerOfProject)
                {
                    developer = project!.Members!.FirstOrDefault(m => m.Id == ticket.DeveloperUserId);

                    if (developer is not null)
                    {
                        bool isDeveloper = await userManager.IsInRoleAsync(developer, nameof(Role.Developer));
                        if (!isDeveloper) developer = null;
                    }

                }
            }
            ticket.DeveloperUser = developer;
            ticket.DeveloperUserId = developer?.Id;

            db.Tickets.Add(ticket);
            await db.SaveChangesAsync();

            return ticket;
        }

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
