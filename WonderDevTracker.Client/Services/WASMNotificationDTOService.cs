using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMNotificationDTOService : INotificationDTOService
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
