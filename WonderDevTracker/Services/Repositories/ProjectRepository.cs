using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Components.UIComponents.ProjectComponents.ProjectDetailsComponents;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;


namespace WonderDevTracker.Services.Repositories
{
    public class ProjectRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
                                    UserManager<ApplicationUser> userManager)
                                             : IProjectRepository
    {

        #region CREATE METHODS
        public async Task<Project?> CreateProjectAsync(Project project, UserInfo user)
        {
            bool isAdmin = user.Roles.Any(r => r == nameof(Role.Admin));
            bool isPM = user.Roles.Any(r => r == nameof(Role.ProjectManager));

            if (!isAdmin && !isPM) throw new UnauthorizedAccessException($"User {user.Email} does not have permission to create a project.");

            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();

            project.Created = DateTimeOffset.UtcNow;
            project.CompanyId = user.CompanyId;

            if (isPM == true)
            {
                //if the user is a PM, add them as a member of the project
                ApplicationUser projectManager = await db.Users.FirstAsync(u => u.Id == user.UserId);
                project.Members?.Add(projectManager);
            }
            db.Add(project);
            await db.SaveChangesAsync();
            return project;
        }
        #endregion

        #region GET METHODS
        public async Task<IEnumerable<Project>> GetAllProjectsAsync(UserInfo user)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            IEnumerable<Project> projects = await db.Projects
                     //match the company id of the user & also ensure the project is not archived
                     .Where(p => p.CompanyId == user.CompanyId && p.Archived == false)
                     .Include(p => p.Members)
                     .ToListAsync();
            return projects;

        }

        public async Task<Project?> GetProjectByIdAsync(int projectId, UserInfo user)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            Project? project = await db.Projects
            .Include(p => p.Tickets)
                        .ThenInclude(t => t.SubmitterUser)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.DeveloperUser)
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            return project;
        }

        public async Task<IEnumerable<ApplicationUser>> GetProjectDevelopersAsync(int projectId, UserInfo user)
        {
            IEnumerable<ApplicationUser> members = await GetProjectMembersAsync(projectId, user);

           var developers = new List<ApplicationUser>();
            foreach (var member in members)
            {
                if (await userManager.IsInRoleAsync(member, nameof(Role.Developer)) == true)
                {
                    developers.Add(member);
                }
            }
            return developers.AsEnumerable();


        }

        public async Task<string?> GetProjectManagerIdAsync(int projectId, UserInfo user)
        {
            IEnumerable<ApplicationUser> members = await GetProjectMembersAsync(projectId, user);

            foreach (var member in members)
            {
                if (await userManager.IsInRoleAsync(member, nameof(Role.ProjectManager)))
                {
                    return member.Id; // <-- only return the userId, not the full ApplicationUser
                }
            }

            return null;
          
        }

        public async Task<ApplicationUser?> GetProjectManagerAsync(int projectId, UserInfo user)
        {
           IEnumerable<ApplicationUser> members = await GetProjectMembersAsync(projectId, user);
            foreach (var member in members)
            {
                if (await userManager.IsInRoleAsync(member, nameof(Role.ProjectManager)) == true)
                {
                    return member;
                }
            }
            return null;
        }

        public Task<IEnumerable<ApplicationUser>> GetProjectMembersByRoleAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ApplicationUser>> GetProjectMembersAsync(int projectId, UserInfo user)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();

            List<ApplicationUser> members = await db.Projects
                 .Where(p => p.Id == projectId && p.CompanyId == user.CompanyId)
                .SelectMany(p => p.Members!)
                                    .ToListAsync();

            return members.AsEnumerable();
        }

        public async Task<IEnumerable<Project>> GetAssignedProjectsAsync(UserInfo user)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            IEnumerable<Project> projects = await db.Projects
                     //match the company id of the user & also ensure the project is not archived
                     .Where(p => p.CompanyId == user.CompanyId && p.Archived == false
                                       && p.Members!.Any(m => m.Id == user.UserId))
                     .Include(p => p.Members)             // <-- ensure Members is populated
                     .AsNoTracking() //read-only query optimization
                     .ToListAsync();
            return projects;
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

        

        public Task<IEnumerable<ApplicationUser>> GetUsersNotOnProjectAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region UPDATE METHODS
        public async Task UpdateProjectAsync(Project project, UserInfo user)

        {

            bool IsRoleAuthorized = await IsUserAuthorizedToEditProject(project.Id, user);
            if (!IsRoleAuthorized) throw new UnauthorizedAccessException($"User {user.Email} does not have permission to update project {project.Id}.");
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();

            var current = await db.Projects.FirstAsync(p => p.Id == project.Id);


            if (!string.Equals(project.Name, current.Name, StringComparison.Ordinal))
            {
                db.Entry(current).Property(p => p.Name).CurrentValue = project.Name;
                db.Entry(current).Property(p => p.Name).IsModified = true;

            }

            if (project.Description != current.Description)

            {

                db.Entry(current).Property(p => p.Description).CurrentValue = project.Description;
                db.Entry(current).Property(p => p.Description).IsModified = true;
            }

            if (project.StartDate != current.StartDate)
            {

                db.Entry(current).Property(p => p.StartDate).CurrentValue = project.StartDate;
                db.Entry(current).Property(p => p.StartDate).IsModified = true;

            }
            if (project.EndDate != current.EndDate)
            {
                db.Entry(current).Property(p => p.EndDate).CurrentValue = project.EndDate;
                db.Entry(current).Property(p => p.EndDate).IsModified = true;

            }

            if (project.Priority != current.Priority)
            {
                db.Entry(current).Property(p => p.Priority).CurrentValue = project.Priority;
                db.Entry(current).Property(p => p.Priority).IsModified = true;

            }

            await db.SaveChangesAsync();
        }
        #endregion

        #region ARCHIVE/RESTORE METHODS
        public async Task<IEnumerable<Project>> GetAllArchivedProjectsAsync(UserInfo user)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            IEnumerable<Project> projects = await db.Projects
                     //match the company id of the user & also ensure the project IS archived
                     .Where(p => p.CompanyId == user.CompanyId && p.Archived == true)
                     .Include(p => p.Members)
                     .ToListAsync();
            return projects;

        }

        public async Task ArchiveProjectAsync(int projectId, UserInfo user)
        {
            bool IsRoleAuthorized = await IsUserAuthorizedToEditProject(projectId, user);
            if (!IsRoleAuthorized) return;

            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();

            Project project = await db.Projects
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
            await db.SaveChangesAsync();
        }
        public async Task RestoreProjectAsync(int projectId, UserInfo user)
        {
            bool IsRoleAuthorized = await IsUserAuthorizedToEditProject(projectId, user);
            if (!IsRoleAuthorized) return;

            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            Project project = await db.Projects
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
            await db.SaveChangesAsync();
        }
        #endregion

        #region ADD/REMOVE METHODS
        //Remove any existing PM and assign the new PM (if any)
        public async Task SetProjectManagerAsync(int projectId, string? managerId, UserInfo user)
        {
            if (!await IsUserAuthorizedToEditProject(projectId, user)) return;

            await using var db = await contextFactory.CreateDbContextAsync();

            var project = await db.Projects
                .Include(p => p.Members)
                .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            // remove any current PMs
            var toRemove = new List<ApplicationUser>();
            foreach (var m in project.Members!)
                if (await userManager.IsInRoleAsync(m, nameof(Role.ProjectManager)))
                    toRemove.Add(m);

            foreach (var pm in toRemove)
                project.Members!.Remove(pm);

            // if managerId is null/empty → it's just a removal
            if (string.IsNullOrWhiteSpace(managerId))
            {
                await db.SaveChangesAsync();
                return;
            }
            // validate new PM
            var newPm = await db.Users
                .FirstOrDefaultAsync(u => u.Id == managerId && u.CompanyId == user.CompanyId);
            if (newPm is null) return;
            if (!await userManager.IsInRoleAsync(newPm, nameof(Role.ProjectManager))) return;

            if (!project.Members!.Any(m => m.Id == newPm.Id))
                project.Members!.Add(newPm);

            await db.SaveChangesAsync();
        }

        public async Task AddProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            bool IsRoleAuthorized = await IsUserAuthorizedToEditProject(projectId, user);
            if (!IsRoleAuthorized) return;

            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();

            //Look up project ~ already confirmed project is not null by IsUserAuthorizedToUpdateProject() 
            Project project = await db.Projects
                    .Include(p => p.Members)
                    .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            //is member already assigned to project
            if (project.Members!.Any(m => m.Id == userId)) return;

            //does member belong to the project's company
            ApplicationUser? newMember = await db.Users
                                        .FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == user.CompanyId);
            //Is member null, a project manager or admin? Cannot be added to the project; PM assignment handled by different means
            if (newMember == null
                || await userManager.IsInRoleAsync(newMember, nameof(Role.ProjectManager))
                || await userManager.IsInRoleAsync(newMember, nameof(Role.Admin)))
                return;


            //assign member to project and save to db
            project.Members!.Add(newMember);
            await db.SaveChangesAsync();

        }

        public async Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            bool IsRoleAuthorized = await IsUserAuthorizedToEditProject(projectId, user);
            if (!IsRoleAuthorized) return;
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            //Look up project ~ already confirmed project is not null by IsUserAuthorizedToEditProject() 
            Project project = await db.Projects
                    .Include(p => p.Members)
                    .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);
            //is member assigned to project
            ApplicationUser? member = project.Members!.FirstOrDefault(m => m.Id == userId);

            if (member == null || await userManager.IsInRoleAsync(member, nameof(Role.ProjectManager))) return;
            //remove member from project and save to db
            project.Members!.Remove(member);
            await db.SaveChangesAsync();

        }

        #endregion

        #region PRIVATE METHODS
        /// <summary>
        ///  Checks:
        ///  1. if project exists in the database
        ///  2. if the user is authorized to update the project
        /// Update Requirements:
        /// 1. User must belong to the same company as the project.
        /// 2. User role: Admin or ProjectManager.
        /// 3. If PM, User must be the existing assigned ProjectManager.
        /// </summary>
        /// <param name="projectId">The ID of the project to check.</param>
        /// <param name="user">The user attempting to update the project.</param>
        /// <returns>True if the user is authorized to update the project, otherwise false.</returns>

        private async Task<bool> IsUserAuthorizedToEditProject(int projectId, UserInfo user)
        {
            await using ApplicationDbContext db = await contextFactory.CreateDbContextAsync();
            // Check if the user is an Admin or ProjectManager
            bool isAdmin = user.Roles.Any(r => r == nameof(Role.Admin));
            bool isPM = user.Roles.Any(r => r == nameof(Role.ProjectManager));
            if (!isAdmin && !isPM) return false;

            bool IsRoleAuthorized = await db.Projects
                // Check if the project exists and belongs to the user's company
                .Where(p => p.Id == projectId && p.CompanyId == user.CompanyId)
                .AnyAsync(p => isAdmin || p.Members!.Any(m => m.Id == user.UserId));
            return IsRoleAuthorized;
        }


        #endregion
    }
}
