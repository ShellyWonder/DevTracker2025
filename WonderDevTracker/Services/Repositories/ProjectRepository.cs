using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;


namespace WonderDevTracker.Services.Repositories
{
    public class ProjectRepository(IDbContextFactory<ApplicationDbContext> contextFactory) :  IProjectRepository
    {
        
        public async Task<IEnumerable<Project>> GetAllProjectsAsync(string userId)
        {
            await using var context = contextFactory.CreateDbContext();

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if(user is null) return [];

            IEnumerable<Project> projects = await context.Projects
                .Where(p => p.CompanyId == user.CompanyId && p.Archived == false)
                .ToListAsync();
            return projects;

        }
    }
}
