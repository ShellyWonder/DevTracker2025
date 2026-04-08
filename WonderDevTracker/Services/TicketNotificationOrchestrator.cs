using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Models;
using WonderDevTracker.Models.Records;
using WonderDevTracker.Services.Interfaces;
using WonderDevTracker.Services.Templates;

namespace WonderDevTracker.Services
{
    ///Coordinates ticket notification delivery and management within the application.
    ///Responsibilities:
    //Resolve recipients(PM, assigned dev, submitter, project members, admins, etc.)
    //Build notification objects(title/message/type/entity ids)
    //Persist via INotificationRepository.AddRangeAsync(...)

    public class TicketNotificationOrchestrator(INotificationRepository notificationRepository,
                                          ITicketRepository ticketRepository,
                                          ITicketNotificationRecipientService ticketRecipientService) : ITicketNotificationOrchestrator
    {

        public async Task TicketAssignedAsync(int ticketId, string assignedUserId, UserInfo actor)
        {
            // Load ticket details needed for messaging / validation
            var context = await InitializeNotificationContextAsync(ticketId, actor);
                     
            if (context.Ticket.DeveloperUserId != assignedUserId)
                throw new InvalidOperationException($"Assigned developer, {assignedUserId}, and Developer User, {context.Ticket.DeveloperUserId}, do not match.");

            var devId = ticketRecipientService.GetAssignedDeveloperRecipient(assignedUserId, actor);

            var pmId = await ticketRecipientService
                .GetProjectManagerRecipientAsync(context.Ticket.ProjectId, actor);

            var submitterId = ticketRecipientService
                .GetSubmitterRecipient(context.Ticket.SubmitterUserId, actor);

            var (devTitle, devMsg) = TicketNotificationTemplates.AssignedToDeveloper(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, devId, devTitle, devMsg, NotificationType.Ticket,
                actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            var (pmTitle, pmMsg) = TicketNotificationTemplates.AssignedToProjectManager(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, pmId, pmTitle, pmMsg, NotificationType.Ticket,
                actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            var (subTitle, subMsg) = TicketNotificationTemplates.AssignedToSubmitter(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, submitterId, subTitle, subMsg, NotificationType.Ticket,
                actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);


            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);

        }

        public async Task TicketUnassignedAsync(int ticketId, string previousDevId, UserInfo actor)
        {
            // Load ticket projection
            var context = await InitializeNotificationContextAsync(ticketId, actor);

            // Recipients
            var pmId = await ticketRecipientService.GetProjectManagerRecipientAsync(context.Ticket.ProjectId, actor);
            string? oldDevRecipientId = ticketRecipientService.GetAssignedDeveloperRecipient(previousDevId, actor);


            var (devTitle, devMsg) = TicketNotificationTemplates.UnassignedForDeveloper(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, oldDevRecipientId, devTitle, devMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            var (pmTitle, pmMsg) = TicketNotificationTemplates.UnassignedForProjectManager(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, pmId, pmTitle, pmMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);
        }

        public async Task TicketCreatedAsync(int ticketId, UserInfo actor)
        {
            //Loads ticket projection
            var context = await InitializeNotificationContextAsync(ticketId, actor);

            // if already assigned → TicketAssignedAsync handled notifications
            //prevent duplicate notifications when ticket is created with an assigned dev
            if (!string.IsNullOrWhiteSpace(context.Ticket.DeveloperUserId)) return;

            //Retrieves PM and submitter
            var pmId = await ticketRecipientService.GetProjectManagerRecipientAsync(context.Ticket.ProjectId, actor);
            var submitterId = ticketRecipientService.GetSubmitterRecipient(context.Ticket.SubmitterUserId, actor);

            // PM should always know about new tickets (especially unassigned)
            //Notifies PM only if PM ≠ submitter
            if (!string.Equals(pmId, submitterId, StringComparison.Ordinal))
            {
                var (pmTitle, pmMsg) = TicketNotificationTemplates.CreatedForProjectManager(context.Ticket);
                AddNotificationOnce(context.Notifications, context.Recipients, pmId, pmTitle, pmMsg,
                    NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);
            }

            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);
        }

        public async Task TicketResolvedAsync(int ticketId, UserInfo actor)
        {
            // Load ticket projection
            var context = await InitializeNotificationContextAsync(ticketId, actor);

            // Recipients
            var pmId = await ticketRecipientService.GetProjectManagerRecipientAsync(context.Ticket.ProjectId, actor);
            var submitterId = ticketRecipientService.GetSubmitterRecipient(context.Ticket.SubmitterUserId, actor);

            // Dev recipient only if ticket is assigned
            string? devId = null;
            if (!string.IsNullOrWhiteSpace(context.Ticket.DeveloperUserId))
            {
                devId = ticketRecipientService.GetAssignedDeveloperRecipient(context.Ticket.DeveloperUserId, actor);
            }

            // Templates
            var (submitterTitle, submitterMsg) = TicketNotificationTemplates.ResolvedForSubmitter(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, submitterId, submitterTitle, submitterMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            var (pmTitle, pmMsg) = TicketNotificationTemplates.ResolvedForProjectManager(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, pmId, pmTitle, pmMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            // notify dev (if not actor + not duplicate)
            var (devTitle, devMsg) = TicketNotificationTemplates.ResolvedForDeveloper(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, devId, devTitle, devMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);
        }

        public async Task TicketArchivedAsync(int ticketId, UserInfo actor)
        {
            var context = await InitializeNotificationContextAsync(ticketId, actor);

            // Recipients
            var pmId = await ticketRecipientService.GetProjectManagerRecipientAsync(context.Ticket.ProjectId, actor);

            string? devId = null;
            if (!string.IsNullOrWhiteSpace(context.Ticket.DeveloperUserId))
            {
                devId = ticketRecipientService.GetAssignedDeveloperRecipient(context.Ticket.DeveloperUserId, actor);
            }

            var submitterId = ticketRecipientService.GetSubmitterRecipient(context.Ticket.SubmitterUserId, actor);

            // Templates
            var (devTitle, devMsg) = TicketNotificationTemplates.ArchivedForDeveloper(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, devId, devTitle, devMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            var (pmTitle, pmMsg) = TicketNotificationTemplates.ArchivedForProjectManager(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, pmId, pmTitle, pmMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            var (subTitle, subMsg) = TicketNotificationTemplates.ArchivedForSubmitter(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, submitterId, subTitle, subMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            if (context.Notifications.Count > 0)
                await notificationRepository.AddRangeAsync(context.Notifications);
        }

        public async Task TicketRestoredAsync(int ticketId, UserInfo actor)
        {
            var context = await InitializeNotificationContextAsync(ticketId, actor);

            // Recipients
            var pmId = await ticketRecipientService.GetProjectManagerRecipientAsync(context.Ticket.ProjectId, actor);

            string? devId = null;
            if (!string.IsNullOrWhiteSpace(context.Ticket.DeveloperUserId))
            {
                devId = ticketRecipientService.GetAssignedDeveloperRecipient(context.Ticket.DeveloperUserId, actor);
            }

            var submitterId = ticketRecipientService.GetSubmitterRecipient(context.Ticket.SubmitterUserId, actor);

            // Templates
            var (devTitle, devMsg) = TicketNotificationTemplates.RestoredForDeveloper(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, devId, devTitle, devMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            var (pmTitle, pmMsg) = TicketNotificationTemplates.RestoredForProjectManager(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, pmId, pmTitle, pmMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

            var (subTitle, subMsg) = TicketNotificationTemplates.RestoredForSubmitter(context.Ticket);
            AddNotificationOnce(context.Notifications, context.Recipients, submitterId, subTitle, subMsg,
                NotificationType.Ticket, actor.UserId, context.Ticket.Id, context.Ticket.ProjectId);

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
        private async Task<TicketNotificationContext> InitializeNotificationContextAsync(int ticketId, UserInfo actor)
        {
            var ticket = await ticketRepository.GetTicketForNotificationsAsync(ticketId, actor.CompanyId)
                ?? throw new InvalidOperationException($"Ticket with ID {ticketId} not found for company {actor.CompanyId}.");
            return new TicketNotificationContext(
                Ticket: ticket,
                Notifications: [],
                Recipients: CreateRecipientSet());
         
        }

        private sealed record TicketNotificationContext(
            TicketForNotification Ticket,
            List<Notification> Notifications,
            HashSet<string> Recipients);
        #endregion

    }
}
