using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;


namespace WonderDevTracker.Services.Repositories
{
    public class ProjectRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : IProjectRepository
    {

        #region CREATE METHODS
        public async Task<Project?> CreateProjectAsync(Project project, UserInfo user)
        {
            bool isAdmin = user.Roles.Any(r => r == nameof(Role.Admin));
            bool isPM = user.Roles.Any(r => r == nameof(Role.ProjectManager));

            if (!isAdmin && !isPM) throw new UnauthorizedAccessException($"User {user.Email} does not have permission to create a project.");

            await using var context = contextFactory.CreateDbContext();
            project.Created = DateTimeOffset.UtcNow;
            project.CompanyId = user.CompanyId;

            if (isPM == true)
            {
                //if the user is a PM, add them as a member of the project
                ApplicationUser projectManager = await context.Users.FirstAsync(u => u.Id == user.UserId);
                project.Members?.Add(projectManager);
            }
            context.Add(project);
            await context.SaveChangesAsync();
            return project;
        }
        #endregion

        #region GET METHODS
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
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            return project;
        }

        public Task<IEnumerable<ApplicationUser>> GetProjectDevelopersAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser?> GetProjectManagerAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetProjectMembersByRoleAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetProjectMembersExceptPMAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<Project?> GetProjectsByPriorityAsync(Project priority, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetProjectSubmittersAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Project>> GetUnassignedProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetUserProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetUsersNotOnProjectAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region UPDATE METHODS
        public async Task UpdateProjectAsync(Project project, UserInfo user)
        {

            bool IsRoleAuthorized = await IsUserAuthorizedToUpdateProject(project.Id, user);
            if (!IsRoleAuthorized) throw new UnauthorizedAccessException($"User {user.Email} does not have permission to update project {project.Id}.");

            await using var context = contextFactory.CreateDbContext();
            var current = await context.Projects.FirstAsync(p => p.Id == project.Id);


            if (!string.Equals(project.Name, current.Name, StringComparison.Ordinal))
            {
                context.Entry(current).Property(p => p.Name).CurrentValue = project.Name;
                context.Entry(current).Property(p => p.Name).IsModified = true;

            }

            if (project.Description != current.Description)

            {

                context.Entry(current).Property(p => p.Description).CurrentValue = project.Description;
                context.Entry(current).Property(p => p.Description).IsModified = true;
            }

            if (project.StartDate != current.StartDate)
            {

                context.Entry(current).Property(p => p.StartDate).CurrentValue = project.StartDate;
                context.Entry(current).Property(p => p.StartDate).IsModified = true;

            }
            if (project.EndDate != current.EndDate)
            {
                context.Entry(current).Property(p => p.EndDate).CurrentValue = project.EndDate;
                context.Entry(current).Property(p => p.EndDate).IsModified = true;

            }

            if (project.Priority != current.Priority)
            {
                context.Entry(current).Property(p => p.Priority).CurrentValue = project.Priority;
                context.Entry(current).Property(p => p.Priority).IsModified = true;

            }

            await context.SaveChangesAsync();
        }
        #endregion

        #region ARCHIVE/RESTORE METHODS
        public Task<IEnumerable<Project>> GetAllArchivedProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task ArchiveProjectAsync(int projectId, UserInfo user)
        {
            bool IsRoleAuthorized = await IsUserAuthorizedToUpdateProject(projectId, user);
            if (!IsRoleAuthorized) return;
            await using var context = contextFactory.CreateDbContext();

            Project project = await context.Projects
                                            .Include(p => p.Tickets)
                                            .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);
            project.Archived = true;
            foreach (var ticket in project.Tickets)
            {
                // Set ArchivedByProject to true for each open (active)ticket in the project being archived; batch archive
                ticket.ArchivedByProject = !ticket.Archived;
                // If ticket.Archived == True, ticket was archived by user
                ticket.Archived = true;

            }
            await context.SaveChangesAsync();
        }
        public  async Task RestoreProjectAsync(int projectId, UserInfo user)
        {
            bool IsRoleAuthorized = await IsUserAuthorizedToUpdateProject(projectId, user);
            if (!IsRoleAuthorized) return;
            await using var  context = contextFactory.CreateDbContext();
            Project project = await context.Projects
                                            .Include(p => p.Tickets)
                                            .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);
            project.Archived = false;

            foreach (Ticket ticket in project.Tickets)
            {
                // IF ArchivedByProject = true, for each ticket in the project previously batch archived, restore ticket to active status
                ticket.Archived = !ticket.ArchivedByProject;
                // If ticket.Archived == false, ticket was archived before project was archived; Therefore, ticket remains archived
                ticket.ArchivedByProject = false;
            }
            await context.SaveChangesAsync();
        }
        #endregion

        #region ADD/REMOVE METHODS

        public Task<bool> AddProjectManagerAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddProjectMembersAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveProjectManagerAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> RemoveProjectMemberAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PRIVATE METHODS
        /// <summary>
        ///  Checks:
        ///  1. if project exists in the database
        ///  2.if the user is authorized to update the project
        /// Update Requirements:
        //1. User must belong to the same company as the project.
        //2. User role: Admin or ProjectManager.
        //3. If PM, User must be the existing assigned ProjectManager .
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="user"></param>
        /// <returns>bool</returns>
        /// 
        private async Task<bool> IsUserAuthorizedToUpdateProject(int projectId, UserInfo user)
        {

            // Check if the user is an Admin or ProjectManager
            bool isAdmin = user.Roles.Any(r => r == nameof(Role.Admin));
            bool isPM = user.Roles.Any(r => r == nameof(Role.ProjectManager));
            if (!isAdmin && !isPM) return false;

            await using var context = contextFactory.CreateDbContext();
            bool IsRoleAuthorized = await context.Projects
                // Check if the project exists and belongs to the user's company
                .Where(p => p.Id == projectId && p.CompanyId == user.CompanyId)
                .AnyAsync(p => isAdmin || p.Members!.Any(m => m.Id == user.UserId));
            return IsRoleAuthorized;
        }


        #endregion
    }
}
