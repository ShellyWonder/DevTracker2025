using WonderDevTracker.Client;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Interfaces
{
    public interface IProjectRepository
    {
        /// <summary>
        /// retrieves all active projects for a specific company asynchronously from the database.
        /// </summary>
        /// <param name="companyId"></param>
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
    }
}
