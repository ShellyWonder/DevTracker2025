using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;


namespace WonderDevTracker.Services.Repositories
{
    public class ProjectRepository(IDbContextFactory<ApplicationDbContext> contextFactory) :  IProjectRepository
    {
        public async Task<Project?> CreateProjectAsync(Project project, UserInfo user)
        {
            bool isAdmin = user.Roles.Any(r => r == nameof(Role.Admin));
            bool isPM = user.Roles.Any(r => r == nameof(Role.ProjectManager));

            if (!isAdmin && !isPM) throw new UnauthorizedAccessException($"User {user.Email} does not have permission to create a project.");

            await using var context = contextFactory.CreateDbContext();
            project.Created = DateTimeOffset.UtcNow;
            project.CompanyId = user.CompanyId;

            if(isPM == true) 
            {
                //if the user is a PM, add them as a member of the project
               ApplicationUser projectManager = await context.Users.FirstAsync(u => u.Id == user.UserId);
               project.Members?.Add(projectManager);
            }
                context.Add(project);
                await context.SaveChangesAsync();
                return project;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync(UserInfo user)
        {
            await using var context = contextFactory.CreateDbContext();

            IEnumerable<Project> projects = await context.Projects
                //match the company id of the user & also ensure the project is not archived
                .Where(p => p.CompanyId == user.CompanyId && p.Archived == false)
                .ToListAsync();
            return projects;

        }

        public async Task<Project?> GetProjectByIdAsync(int projectId, UserInfo user)
        {
            await using var context = contextFactory.CreateDbContext();
            Project? project = await context.Projects
                .Include(p => p.Tickets)
                          .ThenInclude(t => t.SubmitterUser)
                 .Include(p => p.Tickets)
                   .ThenInclude(t => t.DeveloperUser)
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId && p.Archived == false);

            return project;
        }
    }
}
