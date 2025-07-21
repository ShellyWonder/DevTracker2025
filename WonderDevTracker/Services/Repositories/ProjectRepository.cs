using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;


namespace WonderDevTracker.Services.Repositories
{
    public class ProjectRepository(IDbContextFactory<ApplicationDbContext> contextFactory) :  IProjectRepository
    {
        
        public async Task<IEnumerable<Project>> GetAllProjectsAsync(int companyId)
        {
            await using var context = contextFactory.CreateDbContext();
            IEnumerable<Project>projects = await context.Projects.ToListAsync();
            return projects;

        }
    }
}
