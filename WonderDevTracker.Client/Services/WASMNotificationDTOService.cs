using System.Net.Http.Json; 
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMNotificationDTOService(HttpClient http) : INotificationDTOService
    {
        public async Task ArchiveNotificationAsync(int notificationId, string currentUserId, bool isAdmin)
        {
            try
            {
               var response = await http.DeleteAsync($"api/notifications/{notificationId}");
                if(!response.IsSuccessStatusCode)
                    throw new Exception($"Failed to archive notification {notificationId}. Status: {response.StatusCode}");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        public async Task CreateNotificationAsync(NotificationDTO notificationDTO)
        {
            try
            {
                var response = await http.PostAsJsonAsync("api/notifications", notificationDTO);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Failed to create notification. Status: {response.StatusCode}");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
               
            }
        }

        public async Task<List<NotificationDTO>> GetForCurrentUserAsync(string currentUserId, int take = 20)
        {
            try
            {
                // currentUserId is unused on the client.
                // The API derives the current user from authenticated claims.
                var notifications = await http.GetFromJsonAsync<List<NotificationDTO>>($"api/notifications?take={take}")
                    ?? throw new Exception("Failed to fetch notifications.");
                return notifications;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<List<NotificationDTO>> GetArchivedForCurrentUserAsync(string currentUserId, int take = 20)
        {
            // currentUserId is unused on the client.
            // The API derives the current user from authenticated claims.
            try
            {
                List<NotificationDTO> archivedNotifications =
                    await http.GetFromJsonAsync<List<NotificationDTO>>($"api/notifications/archived?take={take}")
                    ?? throw new Exception("Failed to fetch archived notifications.");
                return archivedNotifications;

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error fetching archived notifications: {ex.Message}");
                return [];
            }
        }

        public async Task<List<NotificationDTO>> GetForUserAsAdminAsync(string targetUserId, UserInfo adminUserInfo, int take = 20)
        {
            try
            {
                // adminUserInfo not used — API derives admin identity/authorization from claims
                var adminNotifications = await http.GetFromJsonAsync<List<NotificationDTO>>($"api/notifications/user/{targetUserId}?take={take}")
                        ?? throw new Exception($"Failed to fetch notifications for user {targetUserId}.");
                return adminNotifications;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<int> GetUnreadCountForCurrentUserAsync(UserInfo userInfo)
        {
            try
            {
                int UnreadCount = await http.GetFromJsonAsync<int?>("api/notifications/unread-count") 
                    ?? throw new Exception("Failed to fetch unread count.");
                if (UnreadCount < 0) throw new Exception("Unread count cannot be negative.");
                
                return UnreadCount;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return 0;
            }
        }

        public async Task MarkViewedAsync(int notificationId, string currentUserId)
        {
            try
            {
                // currentUserId not used — server derives user from auth context
                var response = await http.PutAsync($"api/notifications/{notificationId}/viewed", null);
                if(!response.IsSuccessStatusCode)             
                    throw new Exception($"Failed to mark notification {notificationId} as viewed. Status: {response.StatusCode}");
                
            }

            catch (Exception ex)
            {

                Console.WriteLine($"Error marking notification {notificationId} as viewed: {ex.Message}");
            }
        }

        public async Task RestoreNotificationAsync(int notificationId, string currentUserId, bool isAdmin)
        {
            try
            {
                var response = await http.PutAsync($"api/notifications/{notificationId}/restore", null);
                if(!response.IsSuccessStatusCode)
                    throw new Exception($"Failed to restore notification {notificationId}. Status: {response.StatusCode}");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        
    }
}
