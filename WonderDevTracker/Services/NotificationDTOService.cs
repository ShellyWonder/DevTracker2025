using Microsoft.AspNetCore.Identity;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class NotificationDTOService(UserManager<ApplicationUser> userManager,
                                    INotificationRepository notificationRepository ) : INotificationDTOService
    {
        public Task<List<NotificationDTO>> GetForCurrentUserAsync(UserInfo userInfo, int take = 50)
        {
            throw new NotImplementedException();
        }

        public Task<List<NotificationDTO>> GetForUserAsAdminAsync(string targetUserId, UserInfo adminUserInfo, int take = 50)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUnreadCountForCurrentUserAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task MarkViewedAsync(int notificationId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}
