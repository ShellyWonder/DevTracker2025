using WonderDevTracker.Models.Records;

namespace WonderDevTracker.Services.Templates
{
    public static class TicketNotificationTemplates
    {
        // Ticket Assigned
        public static (string Title, string Message) AssignedToDeveloper(TicketForNotification t)
            => ("Ticket assigned",
                $"You were assigned to ticket #{t.Id}: {t.Title}");

        public static (string Title, string Message) AssignedToProjectManager(TicketForNotification t)
            => ("Ticket assignment update",
                $"Ticket #{t.Id}: {t.Title} was assigned.");

        public static (string Title, string Message) AssignedToSubmitter(TicketForNotification t)
            => ("Your ticket was assigned",
                $"Your ticket #{t.Id} was assigned to a developer.");

        //  Ticket Created 
        public static (string Title, string Message) CreatedForProjectManager(TicketForNotification t)
         => ("New ticket created",
        $"New ticket #{t.Id}: {t.Title} was created.");

        // Ticket Resolved 

        //Ticket Archived

        // Ticket Restored
    }
}
