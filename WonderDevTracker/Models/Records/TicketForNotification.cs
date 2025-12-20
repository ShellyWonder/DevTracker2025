namespace WonderDevTracker.Models.Records
{
    public sealed record TicketForNotification
    (
    int Id,
    string Title,
    int ProjectId,
    string? SubmitterUserId,
    string? DeveloperUserId
    );

}
