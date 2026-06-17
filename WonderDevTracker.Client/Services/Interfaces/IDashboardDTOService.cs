using WonderDevTracker.Client.Models.DTOs.DashboardDTO;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface IDashboardDTOService
    {
        /// <summary>
        /// Retrieves dashboard data scoped to the specified user asynchronously.   
        /// </summary>
        /// <remarks>Performs asynchronous I/O and may throw exceptions for authorization failures or data
        /// access errors. Await the returned task.</remarks>
        /// <param name="userInfo">Authenticated user's claims.</param>
        /// <returns>A Task that yields a DashboardDTO containing the dashboard data for the specified user.</returns>
        public Task<DashboardDTO> GetDashboardDataAsync(UserInfo userInfo);
    }
}
