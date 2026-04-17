using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Interfaces
{
    public interface INotificationRepository
    {

        /// <summary>
        /// Asynchronously adds a notification to the system.
        /// </summary>
        /// <param name="notification">The notification to add. Cannot be null.</param>
        /// <remarks>A task that represents the asynchronous add operation.</remarks>
        public Task AddNotificationAsync(Notification notification);

        /// <summary>
        /// Asynchronously retrieves a list of notifications for the specified recipient.   
        /// </summary>
        /// <param name="recipientId">The unique identifier of the recipient whose notifications are to be retrieved. Cannot be null or empty.</param>
        /// <param name="take">The maximum number of notifications to return. Must be greater than zero.</param>
        /// <remarks>A task that represents the asynchronous operation. The task result contains a list of notifications for the
        /// specified recipient. The list will be empty if no notifications are found.</remarks>
        public Task<List<Notification>> GetByRecipientAsync(string recipientId, int take);

        /// <summary>
        /// Gets archived notifications for the specified recipient asynchronously.
        /// </summary>
        /// <param name="recipientId">The recipient identifier.</param>
        /// <param name="take">The maximum number of notifications to return.</param>
        /// <returns>A task that returns a list of archived notifications for the specified recipient.</returns>
        public Task<List<Notification>> GetArchivedByRecipientAsync(string recipientId, int take);


        /// <summary>
        /// Asynchronously retrieves the number of unread messages for the specified recipient.
        /// </summary>
        /// <param name="recipientId">The unique identifier of the recipient whose unread message count is to be retrieved. Cannot be null or
        /// empty.</param>
        /// <remarks>A task that represents the asynchronous operation. The task result contains the number of unread messages
        /// for the specified recipient.</remarks>
        public Task<int> GetUnreadCountAsync(string recipientId);

        /// <summary>
        /// Asynchronously retrieves a notification with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the notification to retrieve. Must be greater than zero.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the notification with the
        /// specified identifier, or <see langword="null"/> if no matching notification is found.</returns>
        public Task<Notification?> GetNotificationById(int id);

        /// <summary>
        /// Asynchronously adds a collection of notifications to the underlying data store.
        /// </summary>
        /// <param name="notifications">The collection of notifications to add. Cannot be null. Each notification in the collection must not be
        /// null.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        public Task AddRangeAsync(IEnumerable<Notification> notifications);

        /// <summary>
        /// Marks the specified notification as viewed for the given recipient asynchronously.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification to mark as viewed.</param>
        /// <param name="recipientId">The identifier of the recipient for whom the notification is being marked as viewed. Cannot be null or
        /// empty.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task MarkViewedAsync(int notificationId, string recipientId);

        /// <summary>
        /// Archives the specified notification for the given user asynchronously.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification to archive.</param>
        /// <param name="userId">The identifier of the user for whom the notification will be archived. Cannot be null or empty.</param>
        /// <param name="isAdmin">A value indicating whether the operation is performed with administrative privileges. If <see
        /// langword="true"/>, the operation may bypass certain user-specific restrictions.</param>
        /// <returns>A task that represents the asynchronous archive operation.</returns>
        public Task ArchiveNotificationAsync(int notificationId, string userId, bool isAdmin);

        /// <summary>
        /// Restores a previously deleted notification for the specified user.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification to restore.</param>
        /// <param name="userId">The identifier of the user for whom the notification will be restored. Cannot be null or empty.</param>
        /// <param name="isAdmin">A value indicating whether the operation is performed with administrative privileges. Set to <see
        /// langword="true"/> to restore notifications as an administrator; otherwise, <see langword="false"/>.</param>
        /// <returns>A task that represents the asynchronous restore operation.</returns>
        public Task RestoreNotificationAsync(int notificationId, string userId, bool isAdmin);

    }
}
