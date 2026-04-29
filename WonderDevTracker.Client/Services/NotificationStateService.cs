//NotificationStateService

using WonderDevTracker.Client.Services.Interfaces;


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
        //Note: The following method will be implemented in prod
        //if needed and if testing proves it is accurate,
        //Used to increment or decrement
        //the unread count without refreshing from the server.
       
        //public void DecrementUnreadCount()
        //{
        //    if (UnreadCount > 0)
        //    {
        //        UnreadCount--;
        //        NotifyStateChanged();
        //    }
        //}

        public void SetUnreadCount(int count)
        {
            UnreadCount = count;
            NotifyStateChanged();
        }
        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
