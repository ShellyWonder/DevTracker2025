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

    public class TicketNotificationOrchestrator(INotificationRepository notificationRepository,
                                          ITicketRepository ticketRepository,
                                          ITicketNotificationRecipientService ticketRecipientService) : ITicketNotificationOrchestrator
    {

        public async Task TicketAssignedAsync(int ticketId, string assignedUserId, UserInfo user)
        {
            // Load ticket details needed for messaging / validation
            var ticket = await ticketRepository.GetTicketForNotificationsAsync(ticketId, user.CompanyId)
                     ?? throw new InvalidOperationException($"Ticket with ID {ticketId} not found for company {user.CompanyId}.");
            if (ticket.DeveloperUserId != assignedUserId)
                throw new InvalidOperationException($"Assigned developer, {assignedUserId}, and Developer User, {ticket.DeveloperUserId}, do not match.");

            var notifications = new List<Notification>();
            var recipients = CreateRecipientSet();
            var devId = ticketRecipientService
                .GetAssignedDeveloperRecipient(assignedUserId, user);

            var pmId = await ticketRecipientService
                .GetProjectManagerRecipientAsync(ticket.ProjectId, user);

            var submitterId = ticketRecipientService
                .GetSubmitterRecipient(ticket.SubmitterUserId, user);

            var (devTitle, devMsg) = TicketNotificationTemplates.AssignedToDeveloper(ticket);
            AddNotificationOnce(notifications, recipients, devId, devTitle, devMsg, NotificationType.Ticket,
                user.UserId, ticket.Id, ticket.ProjectId);

            var (pmTitle, pmMsg) = TicketNotificationTemplates.AssignedToProjectManager(ticket);
            AddNotificationOnce(notifications, recipients, pmId, pmTitle, pmMsg, NotificationType.Ticket,
                user.UserId, ticket.Id, ticket.ProjectId);

            var (subTitle, subMsg) = TicketNotificationTemplates.AssignedToSubmitter(ticket);
            AddNotificationOnce(notifications, recipients, submitterId, subTitle, subMsg, NotificationType.Ticket,
                user.UserId, ticket.Id, ticket.ProjectId);


            if (notifications.Count > 0)
                await notificationRepository.AddRangeAsync(notifications);

        }

        public async Task TicketUnassignedAsync(int ticketId, string previousDevId, UserInfo actor)
        {
            // Load ticket projection
            var ticket = await ticketRepository.GetTicketForNotificationsAsync(ticketId, actor.CompanyId)
                ?? throw new InvalidOperationException($"Ticket with ID {ticketId} not found for company {actor.CompanyId}.");

            var notifications = new List<Notification>();
            var recipients = CreateRecipientSet();

            // Recipients
            var pmId = await ticketRecipientService.GetProjectManagerRecipientAsync(ticket.ProjectId, actor);
            string? oldDevRecipientId =  ticketRecipientService
                                .GetAssignedDeveloperRecipient(previousDevId, actor);


            var (devTitle, devMsg) = TicketNotificationTemplates.UnassignedForDeveloper(ticket);
            AddNotificationOnce(notifications, recipients, oldDevRecipientId, devTitle, devMsg,
                NotificationType.Ticket, actor.UserId, ticket.Id, ticket.ProjectId);

            var (pmTitle, pmMsg) = TicketNotificationTemplates.UnassignedForProjectManager(ticket);
            AddNotificationOnce(notifications, recipients, pmId, pmTitle, pmMsg,
                NotificationType.Ticket, actor.UserId, ticket.Id, ticket.ProjectId);

            if (notifications.Count > 0)
                await notificationRepository.AddRangeAsync(notifications);
        }

        public async Task TicketCreatedAsync(int ticketId, UserInfo user)
        {
            //Loads ticket projection
            var ticket = await ticketRepository.GetTicketForNotificationsAsync(ticketId, user.CompanyId)
                                ?? throw new InvalidOperationException($"Ticket with ID {ticketId} not found for company {user.CompanyId}.");

            // if already assigned → TicketAssignedAsync handled notifications
            //prevent duplicate notifications when ticket is created with an assigned dev
            if (!string.IsNullOrWhiteSpace(ticket.DeveloperUserId)) return;

            var notifications = new List<Notification>();
            var recipients = CreateRecipientSet();
            //Retrieves PM and submitter
            var pmId = await ticketRecipientService.GetProjectManagerRecipientAsync(ticket.ProjectId, user);
            var submitterId = ticketRecipientService.GetSubmitterRecipient(ticket.SubmitterUserId, user);

            // PM should always know about new tickets (especially unassigned)
            //Notifies PM only if PM ≠ submitter
            if (!string.Equals(pmId, submitterId, StringComparison.Ordinal))
            {
                var (pmTitle, pmMsg) = TicketNotificationTemplates.CreatedForProjectManager(ticket);
                AddNotificationOnce(notifications, recipients, pmId, pmTitle, pmMsg,
                    NotificationType.Ticket, user.UserId, ticket.Id, ticket.ProjectId);
            }

            if (notifications.Count > 0)
                await notificationRepository.AddRangeAsync(notifications);
        }

        public async Task TicketResolvedAsync(int ticketId, UserInfo user)
        {
            // Load ticket projection
            var ticket = await ticketRepository.GetTicketForNotificationsAsync(ticketId, user.CompanyId)
                ?? throw new InvalidOperationException($"Ticket with ID {ticketId} not found for company {user.CompanyId}.");

            var notifications = new List<Notification>();
            var recipients = CreateRecipientSet();

            // Recipients
            var pmId = await ticketRecipientService.GetProjectManagerRecipientAsync(ticket.ProjectId, user);
            var submitterId = ticketRecipientService.GetSubmitterRecipient(ticket.SubmitterUserId, user);

            // Dev recipient only if ticket is assigned
            string? devId = null;
            if (!string.IsNullOrWhiteSpace(ticket.DeveloperUserId))
            {
                devId = ticketRecipientService.GetAssignedDeveloperRecipient(ticket.DeveloperUserId, user);
            }

            // Templates
            var (submitterTitle, submitterMsg) = TicketNotificationTemplates.ResolvedForSubmitter(ticket);
            AddNotificationOnce(notifications, recipients, submitterId, submitterTitle, submitterMsg,
                NotificationType.Ticket, user.UserId, ticket.Id, ticket.ProjectId);

            var (pmTitle, pmMsg) = TicketNotificationTemplates.ResolvedForProjectManager(ticket);
            AddNotificationOnce(notifications, recipients, pmId, pmTitle, pmMsg,
                NotificationType.Ticket, user.UserId, ticket.Id, ticket.ProjectId);

            // notify dev (if not actor + not duplicate)
            var (devTitle, devMsg) = TicketNotificationTemplates.ResolvedForDeveloper(ticket);
            AddNotificationOnce(notifications, recipients, devId, devTitle, devMsg,
                NotificationType.Ticket, user.UserId, ticket.Id, ticket.ProjectId);

            if (notifications.Count > 0)
                await notificationRepository.AddRangeAsync(notifications);
        }

        public async Task TicketArchivedAsync(int ticketId, UserInfo actor)
        {
            var ticket = await ticketRepository.GetTicketForNotificationsAsync(ticketId, actor.CompanyId)
                ?? throw new InvalidOperationException($"Ticket with ID {ticketId} not found for company {actor.CompanyId}.");

            var notifications = new List<Notification>();
            var recipients = CreateRecipientSet();

            // Recipients
            var pmId = await ticketRecipientService.GetProjectManagerRecipientAsync(ticket.ProjectId, actor);

            string? devId = null;
            if (!string.IsNullOrWhiteSpace(ticket.DeveloperUserId))
            {
                devId = ticketRecipientService.GetAssignedDeveloperRecipient(ticket.DeveloperUserId, actor);
            }

            var submitterId = ticketRecipientService.GetSubmitterRecipient(ticket.SubmitterUserId, actor);

            // Templates
            var (devTitle, devMsg) = TicketNotificationTemplates.ArchivedForDeveloper(ticket);
            AddNotificationOnce(notifications, recipients, devId, devTitle, devMsg,
                NotificationType.Ticket, actor.UserId, ticket.Id, ticket.ProjectId);

            var (pmTitle, pmMsg) = TicketNotificationTemplates.ArchivedForProjectManager(ticket);
            AddNotificationOnce(notifications, recipients, pmId, pmTitle, pmMsg,
                NotificationType.Ticket, actor.UserId, ticket.Id, ticket.ProjectId);

            var (subTitle, subMsg) = TicketNotificationTemplates.ArchivedForSubmitter(ticket);
            AddNotificationOnce(notifications, recipients, submitterId, subTitle, subMsg,
                NotificationType.Ticket, actor.UserId, ticket.Id, ticket.ProjectId);

            if (notifications.Count > 0)
                await notificationRepository.AddRangeAsync(notifications);
        }

        public async Task TicketRestoredAsync(int ticketId, UserInfo actor)
        {
            var ticket = await ticketRepository.GetTicketForNotificationsAsync(ticketId, actor.CompanyId)
                ?? throw new InvalidOperationException($"Ticket with ID {ticketId} not found for company {actor.CompanyId}.");

            var notifications = new List<Notification>();
            var recipients = CreateRecipientSet();

            // Recipients
            var pmId = await ticketRecipientService.GetProjectManagerRecipientAsync(ticket.ProjectId, actor);

            string? devId = null;
            if (!string.IsNullOrWhiteSpace(ticket.DeveloperUserId))
            {
                devId = ticketRecipientService.GetAssignedDeveloperRecipient(ticket.DeveloperUserId, actor);
            }

            var submitterId = ticketRecipientService.GetSubmitterRecipient(ticket.SubmitterUserId, actor);

            // Templates
            var (devTitle, devMsg) = TicketNotificationTemplates.RestoredForDeveloper(ticket);
            AddNotificationOnce(notifications, recipients, devId, devTitle, devMsg,
                NotificationType.Ticket, actor.UserId, ticket.Id, ticket.ProjectId);

            var (pmTitle, pmMsg) = TicketNotificationTemplates.RestoredForProjectManager(ticket);
            AddNotificationOnce(notifications, recipients, pmId, pmTitle, pmMsg,
                NotificationType.Ticket, actor.UserId, ticket.Id, ticket.ProjectId);

            var (subTitle, subMsg) = TicketNotificationTemplates.RestoredForSubmitter(ticket);
            AddNotificationOnce(notifications, recipients, submitterId, subTitle, subMsg,
                NotificationType.Ticket, actor.UserId, ticket.Id, ticket.ProjectId);

            if (notifications.Count > 0)
                await notificationRepository.AddRangeAsync(notifications);
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
