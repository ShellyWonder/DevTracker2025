using WonderDevTracker.Models.Records;

namespace WonderDevTracker.Services.Templates
{

    public static class ProjectNotificationTemplates
    {
        #region PROJECT CREATED
        public static (string Title, string Message) ProjectCreationForAdmin(ProjectForNotification p)
            => ("New Project Created",
                $"Project #{p.Id}: {p.Name} was created.");
        #endregion

        #region Project Member Added
        public static (string Title, string Message) MemberAddedForAddedUser(ProjectForNotification p)
            => ("Project Addition",
                $"You've been added to project #{p.Id}: {p.Name}. Please contact the project manager with any questions.");

        public static (string Title, string Message) MemberAddedForProjectManager(ProjectForNotification p, string? addedUserId)
            => ("Project Team Updated",
                $"Team member {ResolveName(addedUserId)} is added to project #{p.Id}: {p.Name}.");

        public static (string Title, string Message) MemberAddedForProjectMembers(ProjectForNotification p, string? addedUserId)
            => ("Project Team Updated",
                $"Team member {ResolveName(addedUserId)} is added to project #{p.Id}: {p.Name}.");
        #endregion

        #region Project Member Removed
        public static (string Title, string Message) MemberRemovedForRemovedUser(ProjectForNotification p)
            => ("Removed from Project",
                $"You were removed from project #{p.Id}: {p.Name}. If you think this is a mistake, please contact your project manager.");

        public static (string Title, string Message) MemberRemovedForProjectManager(ProjectForNotification p, string? removedUserName)
            => ("Project Team Updated",
                $"Team member {ResolveName(removedUserName)} was removed from project #{p.Id}: {p.Name}");

        public static (string Title, string Message) MemberRemovedForProjectMembers(ProjectForNotification p, string? removedUserName)
            => ("Project Team Updated",
                $"Team member {ResolveName(removedUserName)} was removed from project #{p.Id}: {p.Name}.");
        #endregion

        #region Project Manager Assigned
        public static (string Title, string Message) ProjectManagerAssignedForAssignedPm(ProjectForNotification p)
            => ("Project Manager Assigned",
                $"You've been assigned project manager for project #{p.Id}: {p.Name}. " +
            $"If you think this is a mistake, please contact the company administrator.");

        public static (string Title, string Message) ProjectManagerAssignedForProjectMembers(ProjectForNotification p, string? pmName)
            => ("Project Manager Assigned",
                $"{ResolveName(pmName)} was assigned to project #{p.Id}: {p.Name} as project manager.");
        public static (string Title, string Message) ProjectManagerAssignedForAdmins(ProjectForNotification p, string? pmName)
            => ("Project Manager Assigned",
                $"{ResolveName(pmName)} was assigned to project #{p.Id}: {p.Name} as project manager.");
        #endregion

        #region Project Manager Removed
        public static (string Title, string Message) ProjectManagerRemovedForPreviousPm(ProjectForNotification p)
            => ("Project Manager Removed",
                $"You've been removed as project manager for project #{p.Id}: {p.Name}. " +
            $"If you think this is a mistake, please contact the company administrator.");

        public static (string Title, string Message) ProjectManagerRemovedForProjectMembers(ProjectForNotification p, string? previousPmName)
            => ("Project Manager Removed",
                $"{ResolveName(previousPmName)} was removed from project #{p.Id}: {p.Name} as project manager.");
        public static (string Title, string Message) ProjectManagerRemovedForAdmins(ProjectForNotification p, string? previousPmName)
            => ("Project Manager Removed",
                $"{ResolveName(previousPmName)} was removed from project #{p.Id}: {p.Name} as project manager.");
        #endregion

        #region Project Archived
        public static (string Title, string Message) ArchivedForProjectManager(ProjectForNotification p)
            => ("Project Archived",
                $"Project #{p.Id}: {p.Name} was archived.");
        public static (string Title, string Message) ArchivedForAdmins(ProjectForNotification p)
            => ("Project Archived",
                $"Project #{p.Id}: {p.Name} was archived.");

        public static (string Title, string Message) ArchivedForProjectMembers(ProjectForNotification p)
            => ("Project Archived",
                $"Project #{p.Id}: {p.Name} was archived.");
        #endregion

        #region Project Restored
        public static (string Title, string Message) RestoredForProjectManager(ProjectForNotification p)
            => ("Project Restored",
                $"Project #{p.Id}: {p.Name} was restored.");

        public static (string Title, string Message) RestoredForProjectMembers(ProjectForNotification p)
            => ("Project Restored",
                $"Project #{p.Id}: {p.Name} was restored.");
        public static (string Title, string Message) RestoredForAdmins(ProjectForNotification p)
            => ("Project Restored",
                $"Project #{p.Id}: {p.Name} was restored.");
        #endregion

        private static string ResolveName(string? name)
        => string.IsNullOrWhiteSpace(name) ? "Unknown user" : name;
    }
}

