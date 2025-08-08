using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface IProjectDTOService
    {
        /// <summary>
        /// Gets all active projects for a specific company asynchronously from db.
        /// </summary>
        /// <param name="companyId"></param>
        /// /// <param name="user">The current users claims</param>
        /// <returns>An enumerable of projects</returns>
        public Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(UserInfo user);

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

        /// <summary>
        /// Retrieves a specific company project by its ID asynchronously from the database.
        /// </summary>
        /// <param name="projectId">Specific company project's id</param>
        /// <param name="user">Current user claims</param>
        /// <returns>Requested Project or Null</returns>
        public Task<ProjectDTO?> GetProjectByIdAsync(int projectId, UserInfo user);

        /// <summary>
        /// Updates an existing project; Roles: User must be assigned ProjectManager or Admin.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="user"></param>
        public Task UpdateProjectAsync(ProjectDTO project, UserInfo user);

    }
}
