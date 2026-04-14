using WonderDevTracker.Models.Records;

namespace WonderDevTracker.Services.Templates
{

    public static class ProjectNotificationTemplates
    {
        #region Project Member Added
        public static (string Title, string Message) MemberAddedForAddedUser(ProjectForNotification p)
            => ("Added to project",
                $"You were added to project #{p.Id}: {p.Name}");

        public static (string Title, string Message) MemberAddedForProjectManager(ProjectForNotification p)
            => ("Project team updated",
                $"A user was added to project #{p.Id}: {p.Name}");

        public static (string Title, string Message) MemberAddedForProjectMembers(ProjectForNotification p)
            => ("Project team updated",
                $"A new member was added to project #{p.Id}: {p.Name}");
        #endregion

        #region Project Member Removed
        public static (string Title, string Message) MemberRemovedForRemovedUser(ProjectForNotification p)
            => ("Removed from project",
                $"You were removed from project #{p.Id}: {p.Name}. If you think this is a mistake, please contact your project manager.");

        public static (string Title, string Message) MemberRemovedForProjectManager(ProjectForNotification p)
            => ("Project team updated",
                $"A user was removed from project #{p.Id}: {p.Name}");

        public static (string Title, string Message) MemberRemovedForProjectMembers(ProjectForNotification p)
            => ("Project team updated",
                $"A member was removed from project #{p.Id}: {p.Name}");
        #endregion

        #region Project Manager Assigned
        public static (string Title, string Message) ProjectManagerAssignedForAssignedPm(ProjectForNotification p)
            => ("Project manager assigned",
                $"You were assigned as project manager for project #{p.Id}: {p.Name}. " +
            $"If you think this is a mistake, please contact the company administrator.");

        public static (string Title, string Message) ProjectManagerAssignedForProjectMembers(ProjectForNotification p, string? pmName)
            => ("Project manager assigned",
                $"{ResolveName(pmName)}, was assigned to project #{p.Id}: {p.Name} as project manager.");
        public static (string Title, string Message) ProjectManagerAssignedForAdmins(ProjectForNotification p, string? pmName)
            => ("Project manager assigned",
                $"{ResolveName(pmName)}, was assigned to project #{p.Id}: {p.Name} as project manager.");
        #endregion

        #region Project Manager Removed
        public static (string Title, string Message) ProjectManagerRemovedForPreviousPm(ProjectForNotification p)
            => ("Project manager removed",
                $"You were removed as project manager for project #{p.Id}: {p.Name}. " +
            $"If you think this is a mistake, please contact the company administrator.");

        public static (string Title, string Message) ProjectManagerRemovedForProjectMembers(ProjectForNotification p, string? previousPmName)
            => ("Project manager removed",
                $"{ResolveName(previousPmName)} was removed from project # {p.Id} :  {p.Name}  as project manager.");
        public static (string Title, string Message) ProjectManagerRemovedForAdmins(ProjectForNotification p, string? previousPmName)
            => ("Project manager removed",
                $"{ResolveName(previousPmName)}, was removed from project #{p.Id}: {p.Name} as project manager.");
        #endregion

        #region Project Archived
        public static (string Title, string Message) ArchivedForProjectManager(ProjectForNotification p)
            => ("Project archived",
                $"Project #{p.Id}: {p.Name} was archived.");
        public static (string Title, string Message) ArchivedForAdmins(ProjectForNotification p)
            => ("Project archived",
                $"Project #{p.Id}: {p.Name} was archived.");

        public static (string Title, string Message) ArchivedForProjectMembers(ProjectForNotification p)
            => ("Project archived",
                $"Project #{p.Id}: {p.Name} was archived.");
        #endregion

        #region Project Restored
        public static (string Title, string Message) RestoredForProjectManager(ProjectForNotification p)
            => ("Project restored",
                $"Project #{p.Id}: {p.Name} was restored.");

        public static (string Title, string Message) RestoredForProjectMembers(ProjectForNotification p)
            => ("Project restored",
                $"Project #{p.Id}: {p.Name} was restored.");
        public static (string Title, string Message) RestoredForAdmins(ProjectForNotification p)
            => ("Project restored",
                $"Project #{p.Id}: {p.Name} was restored.");
        #endregion

        private static string ResolveName(string? name)
    => string.IsNullOrWhiteSpace(name) ? "Unknown user" : name;
    }
}

