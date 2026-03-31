using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;
using WonderDevTracker.Services.Templates;

namespace WonderDevTracker.Services
{
    ///Coordinates notification delivery and management within the application.
    ///Responsibilities:
    //Resolve recipients(PM, assigned dev, submitter, project members, admins, etc.)
    //Build notification objects(title/message/type/entity ids)
    //Persist via INotificationRepository.AddRangeAsync(...)

    public class ProjectNotificationOrchestrator(INotificationRepository notificationRepository,
                                          IProjectRepository projectRepository,
                                          IProjectNotificationRecipientService projectRecipientService) : IProjectNotificationOrchestrator
    {

      
        public Task ProjectMemberAddedAsync(int projectId, string addedUserId, UserInfo actor)
        {
            throw new NotImplementedException();
        }

        public Task ProjectMemberRemovedAsync(int projectId, string removedUserId, UserInfo actor)
        {
            throw new NotImplementedException();
        }

        public Task ProjectManagerAssignedAsync(int projectId, string pmUserId, UserInfo actor)
        {
            throw new NotImplementedException();
        }

        public Task ProjectManagerRemovedAsync(int projectId, string previousPmUserId, UserInfo actor)
        {
            throw new NotImplementedException();
        }

        public Task ProjectArchivedAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task ProjectRestoredAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        #region PRIVATE HELPERS
        private static HashSet<string> CreateRecipientSet()
                                => new(StringComparer.Ordinal);
        private static void AddNotificationOnce(
            List<Notification> notifications,
            HashSet<string> recipients,
            string? recipientId,
            string title,
            string message,
            NotificationType type,
            string senderId,
            int ticketId,
            int projectId)
        {
            if (string.IsNullOrWhiteSpace(recipientId)) return;

            // only add the first time we see recipientId
            if (!recipients.Add(recipientId)) return;

            notifications.Add(new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                SenderId = senderId,
                RecipientId = recipientId,
                TicketId = ticketId,
                ProjectId = projectId,
                Created = DateTimeOffset.UtcNow
            });
        }
        #endregion

    }
}
