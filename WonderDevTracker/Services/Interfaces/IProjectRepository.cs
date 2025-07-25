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
    }
}
