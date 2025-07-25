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
    }
}
