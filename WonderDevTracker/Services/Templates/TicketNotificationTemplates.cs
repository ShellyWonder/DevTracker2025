using WonderDevTracker.Models.Records;

namespace WonderDevTracker.Services.Templates
{
    public static class TicketNotificationTemplates
    {
        #region Ticket Assigned
        public static (string Title, string Message) AssignedToDeveloper(TicketForNotification t)
            => ("Ticket Assigned",
                $"You were assigned to ticket #{t.Id}: {t.Title}");

        public static (string Title, string Message) AssignedToProjectManager(TicketForNotification t)
            => ("Ticket Assignment Update",
                $"Ticket #{t.Id}: {t.Title} was assigned.");

        public static (string Title, string Message) AssignedToSubmitter(TicketForNotification t)
            => ("Your ticket was Assigned",
                $"Your ticket #{t.Id} was assigned to a developer. Please check the ticket for details.");
        #endregion

        #region Ticket Unassigned
        public static (string Title, string Message) UnassignedForDeveloper(TicketForNotification t)
            => ("Ticket Unassigned",
                $"You were unassigned from ticket #{t.Id}: {t.Title}");
        public static (string Title, string Message) UnassignedForProjectManager(TicketForNotification t) 
            =>("Ticket Unassignment Update",
                $"Ticket #{t.Id}: {t.Title} was unassigned.");
        #endregion

        #region Ticket Created
        public static (string Title, string Message) CreatedForProjectManager(TicketForNotification t)
         => ("New ticket Created",
        $"New ticket #{t.Id}: {t.Title} was created. See the ticket for details.");
        #endregion

        #region Ticket Commented
        public static (string Title, string Message) CommentAddedForDeveloper(TicketForNotification t, string actorDisplayName)
            => ("New Ticket Comment",
                $"{actorDisplayName} added a new comment to ticket #{t.Id}: {t.Title}. See the comment for details.");

        public static (string Title, string Message) CommentAddedForSubmitter(TicketForNotification t, string actorDisplayName)
            => ("New Ticket Comment",
                $"{actorDisplayName} added a new comment to ticket #{t.Id}: {t.Title}. See the comment for details.");
        #endregion

        #region Ticket Resolved
        public static (string Title, string Message) ResolvedForSubmitter(TicketForNotification t)
                => ("Ticket Resolved",
                    $"Your ticket #{t.Id}: {t.Title} was marked as resolved.");

        public static (string Title, string Message) ResolvedForProjectManager(TicketForNotification t)
            => ("Ticket Resolved",
                $"Ticket #{t.Id}: {t.Title} was marked as resolved.");

        public static (string Title, string Message) ResolvedForDeveloper(TicketForNotification t)
            => ("Ticket Resolved",
                $"Ticket #{t.Id}: {t.Title} was marked as resolved.");
        #endregion

        #region Ticket Archived
        public static (string Title, string Message) ArchivedForDeveloper(TicketForNotification t)
    => ("Ticket Archived",
        $"Ticket #{t.Id}: {t.Title} was archived.");

        public static (string Title, string Message) ArchivedForProjectManager(TicketForNotification t)
            => ("Ticket Archived",
                $"Ticket #{t.Id}: {t.Title} was archived.");

        public static (string Title, string Message) ArchivedForSubmitter(TicketForNotification t)
            => ("Ticket Archived",
                $"Your ticket #{t.Id}: {t.Title} was archived.");
        #endregion

        #region Ticket Restored
        public static (string Title, string Message) RestoredForDeveloper(TicketForNotification t)
    => ("Ticket Restored",
        $"Ticket #{t.Id}: {t.Title} was restored.");

        public static (string Title, string Message) RestoredForProjectManager(TicketForNotification t)
            => ("Ticket Restored",
                $"Ticket #{t.Id}: {t.Title} was restored.");

        public static (string Title, string Message) RestoredForSubmitter(TicketForNotification t)
            => ("Ticket Restored",
                $"Your ticket #{t.Id}: {t.Title} was restored.");
        #endregion

    }
}
