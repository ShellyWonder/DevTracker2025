using WonderDevTracker.Client;
using WonderDevTracker.Models;
using WonderDevTracker.Models.Records;

namespace WonderDevTracker.Services.Interfaces
{
    public interface IProjectRepository
    {
        #region GET METHODS
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
        public Task<IEnumerable<Project>> GetUnassignedProjectsAsync(UserInfo user);

        /// <summary>
        /// Get Project Manager(PM) 
        /// </summary>
        /// <remarks>Retrieves PM  assigned to a specific project</remarks>
        /// <param name="projectId">Project's id</param>
        /// <param name="user">Current user's claims</param>
        /// <returns>Assigned PM or Null if one is not assigned</returns>
        public Task<ApplicationUser?> GetProjectManagerAsync(int projectId, UserInfo user);

        /// <summary>
        /// Get Project Manager Id
        /// </summary>
        /// <remarks>
        /// This DTO-oriented method exists to provide a lightweight, authoritative way of identifying the
        /// Project Manager without materializing an entire <see cref="ApplicationUser"/> entity.
        /// This method should always be used in conjunction 
        /// with <see cref="SetProjectManagerAsync"/>
        /// to guarantee that reads and writes of PM assignments remain consistent.</remarks>
        /// <param name="projectId">queried project's id</param>
        /// <param name="user">Current user's claims</param>
        /// <returns>ProjectManager id to the client</returns>
        public Task<string?> GetProjectManagerIdAsync(int projectId, UserInfo user);

        /// <summary>
        /// Asynchronously retrieves the collection of developers assigned to the specified project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project for which to retrieve developers.</param>
        /// <param name="user">The user context used to authorize and filter the results. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// developers assigned to the project. The collection is empty if no developers are assigned.</returns>
        public Task<IEnumerable<ApplicationUser>> GetProjectDevelopersAsync(int projectId, UserInfo user);

        /// <summary>
        /// Asynchronously retrieves the collection of users who have submitted items to the specified project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project for which to retrieve submitters.</param>
        /// <param name="user">The user context used to determine access permissions for the operation. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// users who have submitted items to the specified project. The collection is empty if there are no submitters.</returns>
        public Task<IEnumerable<ApplicationUser>> GetProjectSubmittersAsync(int projectId, UserInfo user);

        /// <summary>
        /// Asynchronously retrieves the collection of project members assigned to roles that are accessible to the
        /// specified user within the given project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project for which to retrieve members.</param>
        /// <param name="user">The user context used to determine accessible roles and permissions. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// users who are members of the project and match the accessible roles. The collection is empty if no such
        /// members are found.</returns>
        public Task<IEnumerable<ApplicationUser>> GetProjectMembersByRoleAsync(int projectId, UserInfo user);

        /// <summary>
        /// Get Project Members
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="user">Current user's claims</param>
        /// <remarks>Retrieves all project members assigned to project.</remarks>
        /// <returns>A collection of users</returns>
         public Task<IEnumerable<ApplicationUser>> GetProjectMembersAsync(int projectId, UserInfo user);

        /// <summary>
        /// Retrieves the project information required for sending notifications for the specified project and company.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project for which notification information is requested.</param>
        /// <param name="companyId">The unique identifier of the company that owns the project.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="ProjectForNotification"/> record with notification details for the project, or <see langword="null"/>
        /// if the project is not found or notifications are not applicable.</returns>
        Task<ProjectForNotification?> GetProjectForNotificationsAsync(int projectId, int companyId);

        /// <summary>
        /// Get Assigned Projects
        /// </summary>
        /// <remarks>Retrieves all projects assigned to current user.</remarks>
        /// <param name="user">Current user's claims</param>
        /// <returns>A collection of user's assigned projects</returns>
        public Task<IEnumerable<Project>> GetAssignedProjectsAsync(UserInfo user);

        /// <summary>
        /// Asynchronously retrieves a collection of users who are not assigned to the same project as the specified
        /// user.
        /// </summary>
        /// <param name="user">The user whose project membership is used to determine which users are not on the same project. Cannot be
        /// null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// users not assigned to the specified user's project.</returns>
        public Task<IEnumerable<ApplicationUser>> GetUsersNotOnProjectAsync(UserInfo user);

        /// <summary>
        /// Get All Archived Projects
        /// </summary>
        /// <param name="user">The current users claims</param>
        /// <remarks>Retrieves all archived projects for a specific company asynchronously from db.</remarks>
        /// <returns>An enumerable of archived projects</returns>
        public Task<IEnumerable<Project>> GetAllArchivedProjectsAsync(UserInfo user);
        #endregion

        #region CREATE METHODS
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

        #region UPDATE (& ADD/REMOVE) METHODS
        /// <summary>
        /// Updates an existing project's details; Roles: User must be assigned ProjectManager or Admin.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="user"></param>
        public Task UpdateProjectAsync(Project project, UserInfo user);

        #region PROJECT MANAGER(PM) & PROJECT MEMBERS
        /// <summary>
        /// Set Project Manager 
        /// </summary>
        /// <remarks>Sets or clears the PM for a specific project ; 
        /// If there is an existing PM on the project, than the existing PM is replaced by the new PM ; 
        /// Pass null to clear the PM assignment.
        /// Existing SetProjectManagerAsync and AssignProjectManagerAsync methods
        /// are retained for backward compatibility.
        /// </remarks>
        /// <param name="projectId">Project Id</param>
        /// <param name="managerId">PM user Id (or null to remove)</param>
        /// <param name="user">Current user's claims</param>
        public Task SetProjectManagerAsync(int projectId, string? managerId, UserInfo user);

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
        /// <param name="userId">Id of the member to remove from the project</param>
        /// <param name="user">Current users claims</param>
        public Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user);
        #endregion
        #endregion

        #region ARCHIVE (DELETE) METHODS
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
