using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class DashboardDTOService(IDashboardRepository repository) : IDashboardDTOService
    {
        public async Task<DashboardDTO> GetDashboardDataAsync(UserInfo userInfo)
        {
            DashboardDTO data = await repository.GetDashboardDataAsync(userInfo);
            return data;
        }

    }
}
