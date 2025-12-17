using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface INotificationDTOService
    {

        /// <summary>
        /// Create Notification
        /// </summary>
        /// <param name="notificationDTO">An object containing the details of the notification to create. Cannot be null.</param>
        /// <remarks>Asynchronously creates a new notification based on the specified data transfer object. </remarks>
        public Task CreateNotificationAsync(NotificationDTO notificationDTO);

       /// <summary>
       /// Asynchronously retrieves a list of notifications for the specified user.
       /// </summary>
       /// <param name="userInfo">The user information identifying the current user for whom notifications are to be retrieved. Cannot be null.</param>
       /// <param name="take">The maximum number of notifications to return. Must be a positive integer. The default value is 20.</param>
       /// <returns>A task that represents the asynchronous operation. The task result contains a list of notifications for the
       /// specified user. The list will be empty if no notifications are found.</returns>
        public Task<List<NotificationDTO>> GetForCurrentUserAsync(UserInfo userInfo, int take = 20);


        /// <summary>
        /// Asynchronously retrieves the number of unread messages for the specified user.
        /// </summary>
        /// <param name="userInfo">The user information identifying the current user for whom to retrieve the unread message count. Cannot be
        /// null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of unread messages
        /// for the specified user.</returns>
        public Task<int> GetUnreadCountForCurrentUserAsync(UserInfo userInfo);

        /// <summary>
        /// Marks the specified notification as viewed by the given user asynchronously.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification to mark as viewed.</param>
        /// <param name="userInfo">The user for whom the notification is being marked as viewed. Cannot be null.</param>
        /// <remarks>A task that represents the asynchronous operation.</remarks>
        public Task MarkViewedAsync(int notificationId, UserInfo userInfo);

        /// <summary>
        /// Archive Notification
        /// </summary>
        /// <param name="notificationId">Notification id to archive.</param>
        /// <param name="recipientId">Recipient id for whom the notification will be archived. Cannot be null or empty.</param>
        /// <remarks> Asynchronously archives(soft delete) a notification for a specified recipient. May be restored by the recepient or company admin.</remarks>
        public Task ArchiveNotificationAsync(int notificationId, string recipientId);

        /// <summary>
        /// Restore Archived Notification
        /// </summary>
        /// <param name="notificationId">Id of the notification to restore.</param>
        /// <param name="recipientId">Recipient id for whom the notification will be restored. Cannot be null or empty.</param>
        /// <remarks>Restores a previously archived(soft delete) notification for the specified recipient asynchronously.</remarks>
        public Task RestoreNotificationAsync(int notificationId, string recipientId);


        /// <summary>
        /// Admin Get Notifications For User
        /// </summary>
        /// <remarks>Retrieves a list of notifications for the specified user as an administrator. Ensure that
        /// appropriate permissions are enforced before calling this method.</remarks>
        /// <param name="targetUserId">The unique identifier of the user whose notifications are to be retrieved. Cannot be null or empty.</param>
        /// <param name="adminUserInfo">Information about the administrator performing the operation. Must represent a user with administrative
        /// privileges. Cannot be null.</param>
        /// <param name="take">The maximum number of notifications to retrieve. Must be a positive integer. The default value is 20.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of notifications for the
        /// specified user. The list will be empty if the user has no notifications.</returns>
        public Task<List<NotificationDTO>> GetForUserAsAdminAsync(string targetUserId, UserInfo adminUserInfo, int take = 20);
    }
}

