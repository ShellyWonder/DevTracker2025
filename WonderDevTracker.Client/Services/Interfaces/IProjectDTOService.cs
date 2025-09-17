using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface IProjectDTOService
    {
        #region GET METHODS
        /// <summary>
        /// Gets all active projects for a specific company asynchronously from db.
        /// </summary>
        /// <param name="user">The current users claims</param>
        /// <returns>An enumerable of projects</returns>
        public Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(UserInfo user);
        public Task<ProjectDTO?> GetProjectsByPriorityAsync(ProjectDTO priority, UserInfo user);
        public Task<IEnumerable<ProjectDTO>> GetUnassignedProjectsAsync(UserInfo user);

        /// <summary>
        /// Get Project Manager(PM) 
        /// </summary>
        /// <remarks>Retrieves PM assigned to a specific project</remarks>
        /// <param name="projectId">Project's id</param>
        /// <param name="user">User's claims</param>
        /// <returns>Assigned PM or Null if one is not assigned</returns>
        public Task<AppUserDTO?> GetProjectManagerAsync(int projectId, UserInfo user);
        public Task<IEnumerable<AppUserDTO>> GetProjectDevelopersAsync(int projectId, UserInfo user);
        public Task<IEnumerable<AppUserDTO>> GetProjectSubmittersAsync(int projectId, UserInfo user);
        public Task<IEnumerable<AppUserDTO>> GetProjectMembersByRoleAsync(int projectId, UserInfo user);

        /// <summary>
        /// Retrieves all project members (except the Project Manager) assigned to project.
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="user">Current user's claims</param>
        /// <returns>A collection of users</returns>
        public Task<IEnumerable<AppUserDTO>> GetProjectMembersAsync(int projectId, UserInfo user);

        /// <summary>
        /// Get Assigned Projects
        /// </summary>
        /// <remarks>Retrieves all projects assigned to current user.</remarks>
        /// <param name="user">Current user's claims</param>
        /// <returns>A collection of user's assigned projects</returns>
        public Task<IEnumerable<ProjectDTO>> GetAssignedProjectsAsync(UserInfo user);
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
        /// Set Project Manager(PM)
        /// </summary>
        /// <remarks>Assigns a PM to a specific project ; If there is an existing PM on the project, 
        /// then the existing PM is replaced by the new PM. 
        /// If there is no PM selected, return null 
        /// & pm is unassigned</remarks>
        /// <param name="projectId">The unique identifier of the project to which the manager will be assigned.</param>
        /// <param name="managerId"> Id of PM being assigned to a project</param>
        /// <param name="user">Current user's claims</param>
        /// 
        public Task SetProjectManagerAsync(int projectId, string? managerId, UserInfo user);

        /// <summary>
        /// Add Project Member 
        /// /// </summary>
        /// <remarks>
        /// Assigns a company user to a specific project
        /// </remarks>
        /// <param name="projectId">The unique project identifier to which the member will be assigned.</param>
        /// <param name="userId">User id</param>
        /// <param name="user">Current user's claims</param>
        public Task AddProjectMemberAsync(int projectId, string userId, UserInfo user);

        /// <summary>
        /// Remove Project Member 
        /// /// </summary>
        /// <remarks>
        /// Removes a member from a specific project
        /// </remarks>
        /// <param name="projectId">Project Id</param>
        /// <param name="userId">User id of member to be removed</param>
        /// <param name="user">Current user's claims</param>
        public Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user);

        /// <summary>
        ///Assign Project Manager
        /// </summary>
        /// <remarks>This method assigns the specified user as the project manager for the given project. 
        /// The caller must ensure that the user has the necessary permissions to perform this operation.</remarks>
        /// <param name="projectId">The unique identifier of the existing project.</param>
        /// <param name="userId">The unique identifier of the user to be assigned as the project manager.</param>
        /// <param name="user">The current user's claims</param>
        
        Task AssignProjectManagerAsync(int projectId, string userId, UserInfo user);

        /// <summary>
        ///Remove Project Manager
        /// </summary>
        /// <remarks>This method removes the specified user as the project manager for the given project. 
        /// The caller must ensure that the user has the necessary permissions to perform this operation.</remarks>
        /// <param name="projectId">The unique identifier of the existing project.</param>
        /// <param name="user">The current user's claims</param>    
        Task RemoveProjectManagerAsync(int projectId, UserInfo user);

        #endregion

        #region ARCHIVE METHODS
        /// <summary>
        /// Gets all archived projects for a specific company asynchronously from db.
        /// </summary>
        /// <param name="user">The current users claims</param>
        /// <returns>An enumerable of archived projects</returns>
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
