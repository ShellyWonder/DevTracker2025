using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardsController(IDashboardDTOService dashboardService) : ControllerBase
    {
        private UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

        [HttpGet]
        public async Task<ActionResult<DashboardDTO>> GetDashboardData()
        {
            if (UserInfo is null) return Unauthorized();

            DashboardDTO dashboardData = await dashboardService.GetDashboardDataAsync(UserInfo);
            return Ok(dashboardData);
        }
    }
}
