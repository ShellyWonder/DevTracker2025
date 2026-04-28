//NotificationStateService

using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Client.Models.DTOs;


namespace WonderDevTracker.Client.Services
{
    public class NotificationStateService(INotificationDTOService notificationService)
    {
        public int UnreadCount { get; private set; }
        public event Action? OnChange;

        public async Task RefreshUnreadCountAsync(UserInfo userInfo)
        {
            UnreadCount = await notificationService.GetUnreadCountForCurrentUserAsync(userInfo);
            NotifyStateChanged();
        }

        public async Task MarkAllViewedAsync(UserInfo userInfo)
        {
            await notificationService.MarkAllViewedAsync(userInfo);

            UnreadCount = 0;
            NotifyStateChanged();
        }

        public void SetUnreadCount(int count)
        {
            UnreadCount = count;
            NotifyStateChanged();
        }
        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
