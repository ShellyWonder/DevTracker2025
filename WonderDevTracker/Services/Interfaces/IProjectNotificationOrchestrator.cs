using WonderDevTracker.Client;

namespace WonderDevTracker.Services.Interfaces
{
    public interface IProjectNotificationOrchestrator
    {
        /// <summary>
        /// Project Member Added Notification
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="addedUserId">The identifier of the user to whom the project is being assigned. Cannot be null or empty.</param>
        /// <param name="actor">Current user's claims. Cannot be null.</param>
        /// <returns>Handles the project assignment to a specified user asynchronously..</returns>
        Task ProjectMemberAddedAsync(int projectId, string addedUserId, UserInfo actor);

        /// <summary>
        /// project Member Removed Notification
        /// </summary>
        /// <param name="projectId">project id.</param>
        /// <param name="removedUserId">The user ID of the developer removed from the project. Cannot be null or empty.</param>
        /// <param name="actor">Current user who performed the unassignment action. Cannot be null.</param>
        /// <remarks>Handles notification when a project is unassigned from a developer.</remarks>
        Task ProjectMemberRemovedAsync(int projectId, string removedUserId, UserInfo actor);

        /// <summary>
        /// Notifies the system that a project manager has been assigned to a project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project to which the manager is being assigned.</param>
        /// <param name="pmUserId">Project manager id. Cannot be null or empty.</param>
        /// <param name="actor">Information about the user performing the assignment operation. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ProjectManagerAssignedAsync(int projectId, string pmUserId, UserInfo actor);

        /// <summary>
        /// Handles the removal of a project manager from the specified project asynchronously.
        /// </summary>
        /// <param name="projectId">Project id from which the project manager is being removed.</param>
        /// <param name="previousPmUserId">The id of the project manager who is being removed. Cannot be null or empty.</param>
        /// <param name="actor">Information about the user performing the removal action. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ProjectManagerRemovedAsync(int projectId, string previousPmUserId, UserInfo actor);

        /// <summary>
        /// Project Archived Notification
        /// </summary>
        /// <param name="projectId">project id</param>
        /// <param name="user">Current user who archived project. Cannot be null.</param>
        /// <remarks>Handles project archive notification to assignee.</remarks>
        public Task ProjectArchivedAsync(int projectId, UserInfo user);

        /// <summary>
        /// Project Restored Notification
        /// </summary>
        /// <param name="projectId">project id. Must correspond to an existing archived project.</param>
        /// <param name="user">Information about the user performing the restore operation. Cannot be null.</param>
        /// <remarks>Restores a previously archived project with the specified identifier on behalf of the given user.</remarks>
        public Task ProjectRestoredAsync(int projectId, UserInfo user);
    }
}
