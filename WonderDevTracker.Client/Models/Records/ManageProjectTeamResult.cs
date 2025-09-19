namespace WonderDevTracker.Client.Models.Records
{
    public sealed record class ManageProjectTeamResult
    {
        public HashSet<string> DeveloperIds { get; init; } = [];
        public HashSet<string> SubmitterIds { get; init; } = [];
    }
}
