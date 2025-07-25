using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;


namespace WonderDevTracker.Services.Repositories
{
    public class ProjectRepository(IDbContextFactory<ApplicationDbContext> contextFactory) :  IProjectRepository
    {
        
        public async Task<IEnumerable<Project>> GetAllProjectsAsync(UserInfo user)
        {
            await using var context = contextFactory.CreateDbContext();

            IEnumerable<Project> projects = await context.Projects
                //match the company id of the user & also ensure the project is not archived
                .Where(p => p.CompanyId == user.CompanyId && p.Archived == false)
                .ToListAsync();
            return projects;

        }
    }
}
