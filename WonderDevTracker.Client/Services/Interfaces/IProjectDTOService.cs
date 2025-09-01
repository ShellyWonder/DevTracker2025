using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface IProjectDTOService
    {
        #region GET METHODS
        /// <summary>
        /// Gets all active projects for a specific company asynchronously from db.
        /// </summary>
        /// <param name="companyId"></param>
        /// /// <param name="user">The current users claims</param>
        /// <returns>An enumerable of projects</returns>
        public Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(UserInfo user);
        public Task<ProjectDTO?> GetProjectsByPriorityAsync(ProjectDTO priority, UserInfo user);
        public Task<IEnumerable<ProjectDTO>> GetUnassignedProjectsAsync(UserInfo user);
        public Task<AppUserDTO?> GetProjectManagerAsync(int projectId, UserInfo user);
        public Task<IEnumerable<AppUserDTO>> GetProjectDevelopersAsync(int projectId, UserInfo user);
        public Task<IEnumerable<AppUserDTO>> GetProjectSubmittersAsync(int projectId, UserInfo user);
        public Task<IEnumerable<AppUserDTO>> GetProjectMembersByRoleAsync(int projectId, UserInfo user);
        public Task<IEnumerable<AppUserDTO>> GetProjectMembersExceptPMAsync(int projectId, UserInfo user);
        public Task<IEnumerable<AppUserDTO>> GetUserProjectsAsync(UserInfo user);
        public Task<IEnumerable<AppUserDTO>> GetUsersNotOnProjectAsync(UserInfo user);
        public Task<ProjectDTO?> GetProjectByIdAsync(int projectId, UserInfo user);
        #endregion

        #region CREATE METHODS
        /// <summary>
        /// Creates a new company project in the database asynchronously from a ProjectDTO.
        /// </summary>
        /// <remarks>
        /// Only user with 'Admin' or "ProjectManager" roles can create a new project.
        /// </remarks>
        /// <param name="project">New project to be saved in Db</param>
        /// <param name="user">Current user's claims</param>
        /// <returns>New project after being saved in Db</returns>
        public Task<ProjectDTO> CreateProjectAsync(ProjectDTO project, UserInfo user);
        #endregion

        #region UPDATE METHODS
        /// <summary>
        /// Retrieves a specific company project by its ID asynchronously from the database.
        /// </summary>
        /// <param name="projectId">Specific company project's id</param>
        /// <param name="user">Current user claims</param>
        /// <returns>Requested Project or Null</returns>

        /// <summary>
        /// Updates an existing project; Roles: User must be assigned ProjectManager or Admin.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="user"></param>
        public Task UpdateProjectAsync(ProjectDTO project, UserInfo user);
        #endregion

        #region ADD/REMOVE METHODS
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

        #region ARCHIVE METHODS
        public Task<IEnumerable<ProjectDTO>> GetAllArchivedProjectsAsync(UserInfo user);

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
        /// Restores an archived project (and  all  related{previously unarchived} tickets) by its ID asynchronously  to active status
        /// </summary>
        /// <remarks>
        /// Only user with 'Admin' or "ProjectManager" roles can archive a project.
        /// </remarks>
        /// <param name="projectId">Specific company project's id</param>
        /// <param name="user">Current user claims</param>
        
        public Task RestoreProjectByIdAsync(int projectId, UserInfo user);
        #endregion
    }
}
