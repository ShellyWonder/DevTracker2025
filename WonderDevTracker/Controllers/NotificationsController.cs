using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;

namespace WonderDevTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController(INotificationDTOService notificationService) : ControllerBase
    {
        // Check if the user is authenticated
        private UserInfo? UserInfo => UserInfoHelper.GetUserInfo(User);

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications([FromQuery]int take = 20)
        {
            if (UserInfo is null) return Unauthorized();
            var items = await notificationService.GetForCurrentUserAsync(UserInfo, take);
            return Ok(items);

        }

        [HttpGet("/unread-count")]
        public async Task<IActionResult>GetUserUnreadCount()
        {

            if (UserInfo is null) return Unauthorized();
            var count = await notificationService.GetUnreadCountForCurrentUserAsync(UserInfo);
            return Ok(count);
        }

        [HttpPut("{id:int}/viewed")]
        public async Task<IActionResult> MarkViewed([FromRoute]int id)
        {
            if (UserInfo is null) return Unauthorized();
            await notificationService.MarkViewedAsync(id, UserInfo);
            return NoContent();
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetForUser(
            [FromRoute] string userId,
            [FromQuery] int take = 50)
        {
            var userInfo = UserInfoHelper.GetUserInfo(User);
            if (userInfo is null) return Unauthorized();

            var items = await notificationService.GetForUserAsAdminAsync(userId, userInfo, take);
            return Ok(items);
        }
    }
}
