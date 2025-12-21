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

    public class NotificationOrchestrator(INotificationRepository notificationRepository,
                                          ITicketRepository ticketRepository,
                                          ITicketNotificationRecipientService ticketRecipientService) : INotificationOrchestrator
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
            var devId = await ticketRecipientService
                .GetAssignedDeveloperRecipient(ticket.ProjectId, assignedUserId, user);

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

        public Task TicketResolvedAsync(int ticketId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task TicketArchivedAsync(int ticketId, UserInfo user)
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
