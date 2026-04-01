using WonderDevTracker.Client;

namespace WonderDevTracker.Services.Interfaces
{
    public interface ITicketNotificationRecipientService
    {
        /// <summary>
        /// Asynchronously retrieves the recipient identifier for the project manager associated with the specified
        /// project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project for which to retrieve the project manager recipient.</param>
        /// <param name="actor">The user requesting the recipient information. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the recipient identifier as a
        /// string, or null if no project manager is found.</returns>
        public Task<string?> GetProjectManagerRecipientAsync(int projectId, UserInfo actor);
        
        /// <summary>
        /// Retrieves the recipient identifier for the developer assigned to the specified project. In store memory; no repo call.    
        /// </summary>
        /// <param name="assignedUserId">The unique identifier of the assigned developer.</param>
        /// <param name="actor">The user requesting the recipient information. Cannot be null.</param>
        /// <returns>The recipient identifier as a string, or null if no assigned developer is found.</returns>
        public string? GetAssignedDeveloperRecipient(string? assignedUserId, UserInfo actor);
        
        /// <summary>
        /// Retrieves the recipient identifier for the submitter of a ticket.
        /// </summary>
        /// <param name="submitterUserId">The unique identifier of the submitter.</param>
        /// <param name="actor">The user requesting the recipient information. Cannot be null.</param>
        /// <returns>The recipient identifier as a string, or null if no submitter is found.</returns>
        public string? GetSubmitterRecipient(string? submitterUserId, UserInfo actor);
    }
}
