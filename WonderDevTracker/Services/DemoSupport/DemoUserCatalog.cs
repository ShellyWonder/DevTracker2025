using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Services.DemoSupport
{

    public static class DemoUserCatalog
    {
        public const string AdminEmail = "demoadmin@devtracker.com";
        public const string ProjectManagerEmail = "demopm@devtracker.com";
        public const string DeveloperEmail = "demodev@devtracker.com.";
        public const string SubmitterEmail = "demosub@devtracker.com.";

        public static string? GetEmailForDemoRole(string demoRole)
        {
            return demoRole switch
            {
                nameof(Role.Admin) => AdminEmail,
                nameof(Role.ProjectManager) => ProjectManagerEmail,
                nameof(Role.Developer) => DeveloperEmail,
                nameof(Role.Submitter) => SubmitterEmail,
                _ => null
            };
        }
    }
}
