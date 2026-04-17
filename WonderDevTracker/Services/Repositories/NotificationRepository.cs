using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class NotificationRepository(IDbContextFactory<ApplicationDbContext> context) : INotificationRepository
    {
        public async Task AddNotificationAsync(Notification notification)
        {
            await using var db = await context.CreateDbContextAsync();
            db.Notifications.Add(notification);
            await db.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Notification> notifications)
        {
            await using var db = await context.CreateDbContextAsync();
            db.Notifications.AddRange(notifications);
            await db.SaveChangesAsync();
        }

        public async Task ArchiveNotificationAsync(int notificationId, string userId, bool isAdmin)
              => await SetNotificationArchivedStateAsync(notificationId, userId, isAdmin, archived: true);

        public async Task<List<Notification>> GetByRecipientAsync(string recipientId, int take = 20)
        {
            await using var db = await context.CreateDbContextAsync();
            return await db.Notifications
               .Where(n => n.RecipientId == recipientId && !n.IsArchived)
               .OrderByDescending(n => n.Created)
               .Take(take)
               .ToListAsync();
        }
        
        public async Task<List<Notification>> GetArchivedByRecipientAsync(string recipientId, int take = 20)
        {
            await using var db = await context.CreateDbContextAsync();

            return await db.Notifications
                .Where(n => n.RecipientId == recipientId && n.IsArchived)
                .OrderByDescending(n => n.Created)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Notification?> GetNotificationById(int id)
        {
            await using var db = await context.CreateDbContextAsync();
            return await db.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<int> GetUnreadCountAsync(string recipientId)
        {
            await using var db = await context.CreateDbContextAsync();
            return await db.Notifications
                .Where(n => n.RecipientId == recipientId && !n.HasBeenViewed && !n.IsArchived)
                .CountAsync();
        }

        public async Task MarkViewedAsync(int notificationId, string recipientId)
        {
            await using var db = await context.CreateDbContextAsync();
            var notification = await db.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientId == recipientId) 
                                     ?? throw new UnauthorizedAccessException(
                                      "Notification does not exist or does not belong to the current user.");
            notification.HasBeenViewed = true;

            await db.SaveChangesAsync();
        }

        public async Task RestoreNotificationAsync(int notificationId, string userId, bool isAdmin)
            => await SetNotificationArchivedStateAsync(notificationId, userId, isAdmin, archived: false);

        #region PRIVATE METHODS
        private async Task SetNotificationArchivedStateAsync(int notificationId,
                                                                string userId,
                                                                bool isAdmin,
                                                                bool archived)
        {
            await using var db = await context.CreateDbContextAsync();

            var notification = await db.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId) 
                ?? throw new InvalidOperationException("Notification not found.");

            bool isRecipient = notification.RecipientId == userId;

            if (!isRecipient && !isAdmin)
                throw new UnauthorizedAccessException(
                    archived
                        ? "User not authorized to archive this notification."
                        : "User not authorized to restore this notification.");

            // no-op guard
            // Guard against redundant operations.
            // "archived" represents the desired state:
            //   true  → archive the notification
            //   false → restore the notification
            //
            // If the notification is already in that state (IsArchived == archived),
            // then this is a no-op (e.g., archiving something already archived,
            // or restoring something already active), so we throw to prevent
            // unnecessary updates and incorrect downstream behavior (like notifications).
            if (notification.IsArchived == archived) throw new InvalidOperationException(
                archived
                    ? "Notification is already archived."
                    : "Notification is not archived.");


            notification.IsArchived = archived;

            // Set or clear the ArchivedAt timestamp
            notification.ArchivedAt = archived ? DateTimeOffset.UtcNow : null;

            await db.SaveChangesAsync();
        }

        #endregion
    }
}
