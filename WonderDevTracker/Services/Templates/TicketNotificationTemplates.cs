using WonderDevTracker.Models.Records;

namespace WonderDevTracker.Services.Templates
{
    public static class TicketNotificationTemplates
    {
        #region Ticket Assigned
        public static (string Title, string Message) AssignedToDeveloper(TicketForNotification t)
            => ("Ticket assigned",
                $"You were assigned to ticket #{t.Id}: {t.Title}");

        public static (string Title, string Message) AssignedToProjectManager(TicketForNotification t)
            => ("Ticket assignment update",
                $"Ticket #{t.Id}: {t.Title} was assigned.");

        public static (string Title, string Message) AssignedToSubmitter(TicketForNotification t)
            => ("Your ticket was assigned",
                $"Your ticket #{t.Id} was assigned to a developer.");
        #endregion

        #region Ticket Unassigned
        public static (string Title, string Message) UnassignedForDeveloper(TicketForNotification t)
            => ("Ticket unassigned",
                $"You were unassigned from ticket #{t.Id}: {t.Title}");
        public static (string Title, string Message) UnassignedForProjectManager(TicketForNotification t) 
            =>("Ticket unassignment update",
                $"Ticket #{t.Id}: {t.Title} was unassigned.");
        #endregion

        #region Ticket Created
        public static (string Title, string Message) CreatedForProjectManager(TicketForNotification t)
         => ("New ticket created",
        $"New ticket #{t.Id}: {t.Title} was created.");
        #endregion

        #region Ticket Resolved
        public static (string Title, string Message) ResolvedForSubmitter(TicketForNotification t)
    => ("Ticket resolved",
        $"Your ticket #{t.Id}: {t.Title} was marked as resolved.");

        public static (string Title, string Message) ResolvedForProjectManager(TicketForNotification t)
            => ("Ticket resolved",
                $"Ticket #{t.Id}: {t.Title} was marked as resolved.");

        public static (string Title, string Message) ResolvedForDeveloper(TicketForNotification t)
            => ("Ticket resolved",
                $"Ticket #{t.Id}: {t.Title} was marked as resolved.");
        #endregion

        #region Ticket Archived

        #endregion

        #region Ticket Restored

        #endregion
    }
}
