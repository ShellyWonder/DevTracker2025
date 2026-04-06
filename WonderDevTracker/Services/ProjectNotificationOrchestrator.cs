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


        public async Task ProjectMemberAddedAsync(int projectId, string addedUserId, UserInfo actor)
        {
            var project = await projectRepository.GetProjectForNotificationsAsync(projectId, actor.CompanyId)
                ?? throw new KeyNotFoundException("Project not found or access denied.");

            var notifications = new List<Notification>();
            var recipients = CreateRecipientSet();

            // 1. Added user
            var addedUserRecipient = projectRecipientService.GetAffectedMemberRecipient(addedUserId, actor);

            var (titleAdded, messageAdded) = ProjectNotificationTemplates.MemberAddedForAddedUser(project);

            AddNotificationOnce(notifications, recipients,
                addedUserRecipient,
                titleAdded,
                messageAdded,
                NotificationType.Project,
                actor.UserId,
                projectId);

            // 2. Project Manager
            var pmRecipient = await projectRecipientService.GetProjectManagerRecipientAsync(projectId, actor);

            var (titlePm, messagePm) = ProjectNotificationTemplates.MemberAddedForProjectManager(project);

            AddNotificationOnce(notifications, recipients,
                pmRecipient,
                titlePm,
                messagePm,
                NotificationType.Project,
                actor.UserId,
                projectId);

            // 3. Other members (excluding actor + added user + PM handled via dedupe)
            var memberRecipients = await projectRecipientService
                .GetProjectMemberRecipientsExcludingAsync(projectId, actor, addedUserId, pmRecipient);

            var (titleMembers, messageMembers) = ProjectNotificationTemplates.MemberAddedForProjectMembers(project);

            foreach (var memberId in memberRecipients)
            {
                AddNotificationOnce(notifications, recipients,
                    memberId,
                    titleMembers,
                    messageMembers,
                    NotificationType.Project,
                    actor.UserId,
                    projectId);
            }

            if (notifications.Count > 0)
                await notificationRepository.AddRangeAsync([.. notifications]);
        }

        public async Task ProjectMemberRemovedAsync(int projectId, string removedUserId, UserInfo actor)
        {
            var project = await projectRepository.GetProjectForNotificationsAsync(projectId, actor.CompanyId)
                ?? throw new KeyNotFoundException("Project not found or access denied.");

            var notifications = new List<Notification>();
            var recipients = CreateRecipientSet();

            // 1. Removed user
            var removedUserRecipient = projectRecipientService.GetAffectedMemberRecipient(removedUserId, actor);

            var (titleRemoved, messageRemoved) = ProjectNotificationTemplates.MemberRemovedForRemovedUser(project);

            AddNotificationOnce(
                notifications,
                recipients,
                removedUserRecipient,
                titleRemoved,
                messageRemoved,
                NotificationType.Project,
                actor.UserId,
                projectId);

            // 2. Project Manager
            var pmRecipient = await projectRecipientService.GetProjectManagerRecipientAsync(projectId, actor);

            var (titlePm, messagePm) = ProjectNotificationTemplates.MemberRemovedForProjectManager(project);

            AddNotificationOnce(
                notifications,
                recipients,
                pmRecipient,
                titlePm,
                messagePm,
                NotificationType.Project,
                actor.UserId,
               projectId);

            // 3. Remaining project members
            var memberRecipients = await projectRecipientService
                .GetProjectMemberRecipientsExcludingAsync(projectId, actor, removedUserId, pmRecipient);

            var (titleMembers, messageMembers) = ProjectNotificationTemplates.MemberRemovedForProjectMembers(project);

            foreach (var memberId in memberRecipients)
            {
                AddNotificationOnce(
                    notifications,
                    recipients,
                    memberId,
                    titleMembers,
                    messageMembers,
                    NotificationType.Project,
                    actor.UserId,
                    projectId);
            }

            if (notifications.Count > 0)
                await notificationRepository.AddRangeAsync(notifications);
        }

        public async Task ProjectManagerAssignedAsync(int projectId, string pmUserId, UserInfo actor)
        {
            var project = await projectRepository.GetProjectForNotificationsAsync(projectId, actor.CompanyId)
                ?? throw new KeyNotFoundException("Project not found or access denied.");
            var notifications = new List<Notification>();
            var recipients = CreateRecipientSet();
            // 1. Notify Assigned PM
            var pmRecipient = projectRecipientService.GetAffectedMemberRecipient(pmUserId, actor);
            var (titlePm, messagePm) = ProjectNotificationTemplates.ProjectManagerAssignedForAssignedPm(project);
            
                AddNotificationOnce(
                    notifications,
                    recipients,
                    pmRecipient,
                    titlePm,
                    messagePm,
                    NotificationType.Project,
                    actor.UserId,
                    projectId);

            //2. Notify the rest of the project members 
            var memberRecipients = await projectRecipientService
                .GetProjectMemberRecipientsExcludingAsync(projectId, actor, pmUserId);

            var (memberTitle, memberMessage) = ProjectNotificationTemplates.ProjectManagerAssignedForProjectMembers(project);

            foreach (var memberId in memberRecipients)
            {
                AddNotificationOnce(
                    notifications,
                    recipients,
                    memberId,
                    memberTitle,
                    memberMessage,
                    NotificationType.Project,
                    actor.UserId,
                    projectId);
            }
            if (notifications.Count > 0)
                await notificationRepository.AddRangeAsync(notifications);
        }

        public async Task ProjectManagerRemovedAsync(int projectId, string previousPmUserId, UserInfo actor)
        {
            var project = await projectRepository.GetProjectForNotificationsAsync(projectId, actor.CompanyId)
        ?? throw new KeyNotFoundException("Project not found or access denied.");

            var notifications = new List<Notification>();
            var recipients = CreateRecipientSet();

            // 1. Notify the previous PM directly
            var previousPmRecipient = projectRecipientService.GetAffectedMemberRecipient(previousPmUserId, actor);

            var (pmTitle, pmMessage) = ProjectNotificationTemplates.ProjectManagerRemovedForPreviousPm(project);

            AddNotificationOnce(
                notifications,
                recipients,
                previousPmRecipient,
                pmTitle,
                pmMessage,
                NotificationType.Project,
                actor.UserId,
                projectId);

            // 2. Notify remaining project members
            var memberRecipients = await projectRecipientService
                .GetProjectMemberRecipientsExcludingAsync(projectId, actor, previousPmUserId);

            var (memberTitle, memberMessage) = ProjectNotificationTemplates.ProjectManagerRemovedForProjectMembers(project);

            foreach (var memberId in memberRecipients)
            {
                AddNotificationOnce(
                    notifications,
                    recipients,
                    memberId,
                    memberTitle,
                    memberMessage,
                    NotificationType.Project,
                    actor.UserId,
                    projectId);
            }

            if (notifications.Count > 0)
                await notificationRepository.AddRangeAsync(notifications);
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
        #endregion

    }
}
