using WonderDevTracker.Client;

namespace WonderDevTracker.Services.Interfaces
{
    public interface IProjectNotificationRecipientService
    {
        /// <summary>
        /// Asynchronously retrieves the recipient identifier for the project manager associated with the specified
        /// project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project for which to retrieve the project manager recipient.</param>
        /// <param name="actor">The user requesting the recipient information. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the recipient identifier as a
        /// string, or null if no project manager is found.</returns>
        Task<string?> GetProjectManagerRecipientAsync(int projectId, UserInfo actor);

        /// <summary>
        /// Gets the recipient identifier for the affected member based on the specified user ID and actor information 
        /// Relies on in-memory logic - no repository call.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose recipient information is to be retrieved. Can be null to indicate an
        /// anonymous or system-level operation.</param>
        /// <param name="actor">The user information representing the actor performing the operation. Cannot be null.</param>
        /// <returns>A string containing the recipient identifier for the affected member, or null if no recipient is applicable.</returns>
        string? GetAffectedMemberRecipient(string? userId, UserInfo actor);

        /// <summary>
        /// Asynchronously retrieves the list of all recipient members of the specified project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project whose member recipients are to be retrieved.</param>
        /// <param name="actor">The user requesting the recipient list. Cannot be null.</param>
        /// <returns>A list of the project's members. The list is empty if the project has no members.</returns>
        Task<List<string>> GetProjectMemberRecipientsAsync(int projectId, UserInfo actor);

        /// <summary>
        /// Asynchronously retrieves the user IDs of all project members who are eligible to receive notifications,
        /// excluding the specified users.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project whose member recipients are to be retrieved.</param>
        /// <param name="actor">The user performing the operation. This parameter is used to determine access permissions and context.</param>
        /// <param name="excludedUserIds">An array of user IDs to exclude from the recipient list. Any user ID specified in this array will not be
        /// included in the returned results. Elements may be null.</param>
        /// <returns>A task result list containing a list of user IDs for project
        /// members who are eligible recipients, excluding those specified in the excludedUserIds parameter.</returns>
        Task<List<string>> GetProjectMemberRecipientsExcludingAsync(
            int projectId,
            UserInfo actor,
            params string?[] excludedUserIds);
    }
}
