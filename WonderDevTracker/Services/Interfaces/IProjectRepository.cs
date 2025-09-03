using WonderDevTracker.Client;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Interfaces
{
    public interface IProjectRepository
    {
        #region Get Methods
        /// <summary>
        /// retrieves all active projects for a specific company asynchronously from the database.
        /// </summary>
       
        /// <param name="user">The current users claims</param>
        /// <returns>All company projects</returns>
        public Task<IEnumerable<Project>> GetAllProjectsAsync(UserInfo user);

        /// <summary>
        /// Retrieves a specific company project by its ID asynchronously from the database.
        /// </summary>
        /// <param name="projectId">Specific company project's id</param>
        /// <param name="user">Current user claims</param>
        /// <returns>Requested Project or Null</returns>
        public Task<Project?> GetProjectByIdAsync(int projectId, UserInfo user);
        
        public Task<Project?> GetProjectsByPriorityAsync(Project priority, UserInfo user);
        public Task<IEnumerable<Project>>GetUnassignedProjectsAsync(UserInfo user);
        public Task<ApplicationUser?> GetProjectManagerAsync(int projectId, UserInfo user);
        public Task<IEnumerable<ApplicationUser>> GetProjectDevelopersAsync(int projectId, UserInfo user);
        public Task<IEnumerable<ApplicationUser>> GetProjectSubmittersAsync(int projectId, UserInfo user);
        public Task<IEnumerable<ApplicationUser>> GetProjectMembersByRoleAsync(int projectId, UserInfo user);

        /// <summary>
        /// Retrieves all project members (except the Project Manager) assigned to project.
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="user">Current user's claims</param>
        /// <returns>A collection of users</returns>
        public Task<IEnumerable<ApplicationUser>> GetProjectMembersAsync(int projectId, UserInfo user);
        public Task<IEnumerable<ApplicationUser>>GetUserProjectsAsync(UserInfo user);
        public Task<IEnumerable<ApplicationUser>> GetUsersNotOnProjectAsync(UserInfo user);
        public Task<IEnumerable<Project>> GetAllArchivedProjectsAsync(UserInfo user);
        #endregion

        #region Create Methods
        /// <summary>
        /// Creates a new company project in the database asynchronously.
        /// </summary>
        /// <remarks>
        /// Only user with 'Admin' or "ProjectManager" roles can create a new project.
        /// </remarks>
        /// <param name="project">New project to be saved in Db</param>
        /// <param name="user">Current user's claims</param>
        /// <returns>New project after being saved in Db</returns>
        public Task<Project?> CreateProjectAsync(Project project, UserInfo user);
        #endregion

        #region Update (& Add/Remove) Methods
        /// <summary>
        /// Updates an existing project's details; Roles: User must be assigned ProjectManager or Admin.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="user"></param>

        public Task UpdateProjectAsync(Project project, UserInfo user);
        public Task<bool> AddProjectManagerAsync(int projectId, UserInfo user);
        public Task<bool> RemoveProjectManagerAsync(int projectId, UserInfo user);

        /// <summary>
        /// Add Project Member 
        /// /// </summary>
        /// <remarks>
        /// Assigns a company user to a specific project
        /// </remarks>
        /// <param name="projectId">Project Id</param>
        /// <param name="userId">User id</param>
        /// <param name="user">Current users claims</param>
        public Task AddProjectMemberAsync(int projectId, string userId, UserInfo user);

        /// <summary>
        /// Remove Project Member 
        /// /// </summary>
        /// <remarks>
        /// Removes a member from a specific project
        /// </remarks>
        /// <param name="projectId">Project Id</param>
        /// <param name="userId">User id</param>
        /// <param name="user">Current users claims</param>
        public Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user);

        #endregion

        #region Archive (Delete) Methods
        /// <summary>
        /// Archives a project (and  all  related tickets) by its ID asynchronously 
        /// </summary>
        /// <remarks>
        /// Only user with 'Admin' or "ProjectManager" roles can archive a project.
        /// </remarks>
        /// <param name="projectId">Specific company project's id</param>
        /// <param name="user">Current user claims</param>
        /// <returns>True if the project was archived, otherwise false</returns>
        public Task ArchiveProjectAsync(int projectId, UserInfo user);

        /// <summary>
        /// Restores an archived project (and all related {previously unarchived} tickets) by its ID asynchronously to active status.
        /// </summary>
        /// <remarks>
        /// Only user with 'Admin' or "ProjectManager" roles can restore a project.
        /// </remarks>
        /// <param name="projectId">Specific company project's id</param>
        /// <param name="user">Current user claims</param>

        public Task RestoreProjectAsync(int projectId, UserInfo user);
        #endregion
    }
}
