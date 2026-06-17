using System.Net.Http.Json;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMDashboardDTOService(HttpClient http) : IDashboardDTOService
    {
        public async Task<DashboardDTO> GetDashboardDataAsync(UserInfo userInfo)
        {
			try
			{
				return await http.GetFromJsonAsync<DashboardDTO>($"api/dashboards")
					                           ?? throw new InvalidOperationException("Failed to retrieve dashboard data.");
			}
			catch (Exception ex)
			{

				Console.WriteLine($"Error occurred while retrieving dashboard data: {ex.Message}");
				return null!;
			}
        }
    }
}
