using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Models;
using WonderDevTracker.Models.Records;
using WonderDevTracker.Services.Interfaces;
using WonderDevTracker.Services.Templates;

namespace WonderDevTracker.Services
{
    ///Coordinates project notification delivery and management within the application.
    ///Responsibilities:
    //Resolve recipients(PM, assigned devs, project members, admins, etc.)
    //Build notification objects(title/message/type/entity ids)
    //Persist via INotificationRepository.AddRangeAsync(...)

    public class ProjectNotificationOrchestrator(INotificationRepository notificationRepository,
                                          IProjectRepository projectRepository,
                                          IProjectNotificationRecipientService projectRecipientService) : IProjectNotificationOrchestrator
    {

        public async Task ProjectMemberAddedAsync(int projectId, string addedUserId, UserInfo actor)
        {
            var context = await InitializeNotificationContextAsync(projectId, actor);

            // 1. Added user
            var addedUserRecipient = projectRecipientService.GetAffectedMemberRecipient(addedUserId, actor);

            var (titleAdded, messageAdded) = ProjectNotificationTemplates.MemberAddedForAddedUser(context.Project);

            AddNotificationOnce(context.Notifications, context.Recipients, addedUserRecipient, titleAdded,
                messageAdded, NotificationType.Project, actor.UserId, projectId);

            // 2. Project Manager
            var pmRecipient = await projectRecipientService.GetProjectManagerRecipientAsync(projectId, actor);

            var (titlePm, messagePm) = ProjectNotificationTemplates.MemberAddedForProjectManager(context.Project);

            AddNotificationOnce(context.Notifications, context.Recipients, pmRecipient, titlePm, messagePm,
                NotificationType.Project, actor.UserId, projectId);

            // 3. Other members (excluding actor + added user + PM handled via dedupe)
            var memberRecipients = await projectRecipientService
                .GetProjectMemberRecipientsExcludingAsync(projectId, actor, addedUserId, pmRecipient);

            var (titleMembers, messageMembers) = ProjectNotificationTemplates.MemberAddedForProjectMembers(context.Project);

            foreach (var memberId in memberRecipients)
            {
                AddNotificationOnce(context.Notifications, context.Recipients, memberId, titleMembers, messageMembers,
                    NotificationType.Project, actor.UserId, projectId);
            }

            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);
        }

        public async Task ProjectMemberRemovedAsync(int projectId, string removedUserId, UserInfo actor)
        {
            var context = await InitializeNotificationContextAsync(projectId, actor);

            // 1. Removed user
            var removedUserRecipient = projectRecipientService.GetAffectedMemberRecipient(removedUserId, actor);

            var (titleRemoved, messageRemoved) = ProjectNotificationTemplates.MemberRemovedForRemovedUser(context.Project);

            AddNotificationOnce(context.Notifications, context.Recipients, removedUserRecipient, titleRemoved,
                messageRemoved, NotificationType.Project, actor.UserId, projectId);

            // 2. Project Manager
            var pmRecipient = await projectRecipientService.GetProjectManagerRecipientAsync(projectId, actor);

            var (titlePm, messagePm) = ProjectNotificationTemplates.MemberRemovedForProjectManager(context.Project);

            AddNotificationOnce(context.Notifications, context.Recipients, pmRecipient, titlePm, messagePm,
                NotificationType.Project, actor.UserId, projectId);

            // 3. Remaining project members
            var memberRecipients = await projectRecipientService
                .GetProjectMemberRecipientsExcludingAsync(projectId, actor, removedUserId, pmRecipient);

            var (titleMembers, messageMembers) = ProjectNotificationTemplates.MemberRemovedForProjectMembers(context.Project);

            foreach (var memberId in memberRecipients)
            {
                AddNotificationOnce(context.Notifications, context.Recipients, memberId, titleMembers, messageMembers,
                    NotificationType.Project, actor.UserId, projectId);
            }

            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);
        }

        public async Task ProjectManagerAssignedAsync(int projectId, string pmUserId, UserInfo actor)
        {
            var context = await InitializeNotificationContextAsync(projectId, actor);

            // 1. Notify Assigned PM
            var pmRecipient = projectRecipientService.GetAffectedMemberRecipient(pmUserId, actor);
            var (titlePm, messagePm) = ProjectNotificationTemplates.ProjectManagerAssignedForAssignedPm(context.Project);
            
                AddNotificationOnce(context.Notifications, context.Recipients, pmRecipient, titlePm, messagePm,
                    NotificationType.Project, actor.UserId,
                    projectId);

            //2. Notify the rest of the project members 
            var memberRecipients = await projectRecipientService
                .GetProjectMemberRecipientsExcludingAsync(projectId, actor, pmUserId);

            var (memberTitle, memberMessage) = ProjectNotificationTemplates.ProjectManagerAssignedForProjectMembers(context.Project);

            foreach (var memberId in memberRecipients)
            {
                AddNotificationOnce(context.Notifications, context.Recipients, memberId, memberTitle, memberMessage,
                    NotificationType.Project, actor.UserId, projectId);
            }
            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);
        }

        public async Task ProjectManagerRemovedAsync(int projectId, string previousPmUserId, UserInfo actor)
        {
            var context = await InitializeNotificationContextAsync(projectId, actor);
            // 1. Notify the previous PM directly
            var previousPmRecipient = projectRecipientService.GetAffectedMemberRecipient(previousPmUserId, actor);

            var (pmTitle, pmMessage) = ProjectNotificationTemplates.ProjectManagerRemovedForPreviousPm(context.Project);

            AddNotificationOnce(context.Notifications, context.Recipients, previousPmRecipient, pmTitle, pmMessage,
                NotificationType.Project, actor.UserId, projectId);

            // 2. Notify remaining project members
            var memberRecipients = await projectRecipientService
                .GetProjectMemberRecipientsExcludingAsync(projectId, actor, previousPmUserId);

            var (memberTitle, memberMessage) = ProjectNotificationTemplates.ProjectManagerRemovedForProjectMembers(context.Project);

            foreach (var memberId in memberRecipients)
            {
                AddNotificationOnce(context.Notifications, context.Recipients, memberId, memberTitle, memberMessage,
                    NotificationType.Project, actor.UserId, projectId);
            }

            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);
        }

        public async Task ProjectArchivedAsync(int projectId, UserInfo actor)
        {
            var context = await InitializeNotificationContextAsync(projectId, actor);

            // Notify all project members about archiving
            var memberRecipients = await projectRecipientService.GetProjectMemberRecipientsAsync(projectId, actor);
            var (title, message) = ProjectNotificationTemplates.ArchivedForProjectMembers(context.Project);
            foreach (var memberId in memberRecipients)
            {
                AddNotificationOnce(context.Notifications, context.Recipients, memberId, title, message,
                    NotificationType.Project, actor.UserId, projectId);
            }
            
            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);
        }

        public async Task ProjectRestoredAsync(int projectId, UserInfo actor)
        {
            var context = await InitializeNotificationContextAsync(projectId, actor);

            // Notify all project members about archiving
            var memberRecipients = await projectRecipientService.GetProjectMemberRecipientsAsync(projectId, actor);
            var(title, message) = ProjectNotificationTemplates.RestoredForProjectMembers(context.Project);
            foreach (var memberId in memberRecipients)
            {
                AddNotificationOnce(context.Notifications, context.Recipients, memberId, title, message,
                    NotificationType.Project, actor.UserId, projectId);
            }

            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);
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
                ProjectId = projectId,
                Created = DateTimeOffset.UtcNow
            });
        }

        private async Task<ProjectNotificationContext> InitializeNotificationContextAsync(int projectId, UserInfo actor)
        {
            var project = await projectRepository.GetProjectForNotificationsAsync(projectId, actor.CompanyId)
                ?? throw new KeyNotFoundException("Project not found or access denied.");
            return new ProjectNotificationContext(
                Project: project,
                Notifications: [],
                Recipients: CreateRecipientSet());
        }

        private sealed record ProjectNotificationContext(
        ProjectForNotification Project,
        List<Notification> Notifications,
        HashSet<string> Recipients);
        #endregion

    }
}
