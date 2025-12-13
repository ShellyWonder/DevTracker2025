using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Interfaces
{
    public interface INotificationRepository
    {
        public Task <List<Notification>>GetByRecipientAsync(string recipientId);
        public Task <int> GetUnreadCountAsync(string recipientId);
        public Task<Notification?> GetNotificationById(int id);
        public Task AddRangeAsync(IEnumerable<Notification> notifications);
        public Task MarkViewedAsync(int notificationId, string recipientId);
    }
}
