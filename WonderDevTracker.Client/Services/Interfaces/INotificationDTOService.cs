using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface INotificationDTOService
    {
        Task<List<NotificationDTO>> GetForCurrentUserAsync(UserInfo userInfo, int take = 50);
        Task<int> GetUnreadCountForCurrentUserAsync(UserInfo userInfo);

        Task MarkViewedAsync(int notificationId, UserInfo userInfo);

        // Admin-only
        Task<List<NotificationDTO>> GetForUserAsAdminAsync(string targetUserId, UserInfo adminUserInfo, int take = 50);
    }
}

