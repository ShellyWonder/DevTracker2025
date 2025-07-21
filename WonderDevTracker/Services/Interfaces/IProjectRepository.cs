using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Interfaces
{
    public interface IProjectRepository
    {
        /// <summary>
        /// retrieves all projects for a specific company asynchronously from the database.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns>All company projects</returns>
        public Task<IEnumerable<Project>> GetAllProjectsAsync(string userId);
    }
}
