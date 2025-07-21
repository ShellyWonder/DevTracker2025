using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface IProjectDTOService
    {
        /// <summary>
        /// Gets all projects for a specific company asynchronously from db.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns>An enumerable of projects</returns>
        public Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(int companyId);
    }
}
