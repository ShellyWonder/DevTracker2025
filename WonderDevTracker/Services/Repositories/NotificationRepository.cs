using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class NotificationRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : INotificationRepository
    {
        
        public Task AddRangeAsync(IEnumerable<Notification> notifications)
        {
            throw new NotImplementedException();
        }

        public Task<List<Notification>> GetByRecipientAsync(string recipientId)
        {
            throw new NotImplementedException();
        }

        public Task<Notification?> GetNotificationById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUnreadCountAsync(string recipientId)
        {
            throw new NotImplementedException();
        }

        public Task MarkViewedAsync(int notificationId, string recipientId)
        {
            throw new NotImplementedException();
        }
    }
}
