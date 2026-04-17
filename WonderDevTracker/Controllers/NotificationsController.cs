using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController(INotificationDTOService notificationService) : ControllerBase
    {
        // Check if the user is authenticated
        private UserInfo? UserInfo => UserInfoHelper.GetUserInfo(User);

        #region GET
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications([FromQuery] int take = 20)
        {
            if (UserInfo is null) return Unauthorized();
            var items = await notificationService.GetForCurrentUserAsync(UserInfo.UserId, take);
            return Ok(items);

        }
        [HttpGet("archived")]
        public async Task<IActionResult> GetUserArchivedNotifications([FromQuery] int take = 20)
        {
            if (UserInfo is null) return Unauthorized();
            var items = await notificationService.GetArchivedForCurrentUserAsync(UserInfo.UserId, take);
            return Ok(items);

        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUserUnreadCount()
        {

            if (UserInfo is null) return Unauthorized();
            var count = await notificationService.GetUnreadCountForCurrentUserAsync(UserInfo);
            return Ok(count);
        }

        
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetForUser([FromRoute] string userId, [FromQuery] int take = 20)
        {

            if (UserInfo is null) return Unauthorized();

            var items = await notificationService.GetForUserAsAdminAsync(userId, UserInfo, take);
            return Ok(items);
        }
        #endregion

        #region ARCHIVE/DELETE
        // SOFT DELETE 
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> ArchiveNotification([FromRoute] int id)
        {
            //archive status is handled in repository
            if (UserInfo is null) return Unauthorized();
            //permission check
            // Only the owner of the notification or an admin can archive it
            var currentUserId = UserInfo.UserId;        
            var isAdmin = UserInfo.IsInRole(Role.Admin); 

            await notificationService.ArchiveNotificationAsync(id, currentUserId, isAdmin);
            return NoContent();
        }
        #endregion

        #region PUT
        [HttpPut("{id:int}/restore")]
        public async Task<IActionResult> RestoreNotification([FromRoute] int id)
        {
            if (UserInfo is null) return Unauthorized();
            //permission check
            // Only the owner of the notification or an admin can restore it
            var currentUserId = UserInfo.UserId;
            var isAdmin = UserInfo.IsInRole(Role.Admin);
            await notificationService.RestoreNotificationAsync(id, currentUserId, isAdmin);
            return NoContent();
        }

        [HttpPut("{id:int}/viewed")]
        public async Task<IActionResult> MarkViewed([FromRoute] int id)
        {
            if (UserInfo is null) return Unauthorized();
            await notificationService.MarkViewedAsync(id, UserInfo.UserId);
            return NoContent();
        }
        #endregion
    }
}
