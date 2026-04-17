using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class NotificationDTOService(INotificationRepository notificationRepository) : INotificationDTOService
    {
        public async Task CreateNotificationAsync(NotificationDTO dto)
        {
            var notification = new Notification
            {
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                SenderId = dto.SenderId,
                RecipientId = dto.RecipientId,
                TicketId = dto.TicketId,
                ProjectId = dto.ProjectId,
                Created = DateTimeOffset.UtcNow

            };
            await notificationRepository.AddNotificationAsync(notification);
        }

        public async Task<List<NotificationDTO>> GetForCurrentUserAsync(string currentUserId, int take = 20)
        {
            var notifications = await notificationRepository
                .GetByRecipientAsync(currentUserId, take);
            return [.. notifications.Select(n => new NotificationDTO
            {
                Id = n.Id,
                Title = n.Title!,
                Message = n.Message!,
                Type = n.Type,
                Created = n.Created,
                HasBeenViewed = n.HasBeenViewed,

                SenderId = n.SenderId!,
                RecipientId = n.RecipientId!,

                TicketId = n.TicketId,
                ProjectId = n.ProjectId
            })];
        }
        public async Task<List<NotificationDTO>> GetArchivedForCurrentUserAsync(string currentUserId, int take = 20)
        {
            var notifications = await notificationRepository
                .GetArchivedByRecipientAsync(currentUserId, take);
            return [.. notifications.Select(n => new NotificationDTO
            {
                Id = n.Id,
                Title = n.Title!,
                Message = n.Message!,
                Type = n.Type,
                Created = n.Created,
                HasBeenViewed = n.HasBeenViewed,
                ArchivedAt = n.ArchivedAt,

                SenderId = n.SenderId!,
                RecipientId = n.RecipientId!,

                TicketId = n.TicketId,
                ProjectId = n.ProjectId
            })];
        }

        public Task<List<NotificationDTO>> GetForUserAsAdminAsync(string targetUserId, UserInfo adminUserInfo, int take = 20)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUnreadCountForCurrentUserAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public async Task MarkViewedAsync(int notificationId, string recipientId)
        {
            await notificationRepository.MarkViewedAsync(notificationId, recipientId);
        }

        public async Task ArchiveNotificationAsync(int notificationId, string currentUserId, bool isAdmin)
        {
            await notificationRepository.ArchiveNotificationAsync(notificationId, currentUserId, isAdmin);
        }

        public async Task RestoreNotificationAsync(int notificationId, string currentUserId, bool isAdmin)
        {
            await notificationRepository.RestoreNotificationAsync(notificationId, currentUserId, isAdmin);
        }

    }
}
