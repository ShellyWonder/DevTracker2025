using System.Net.Http.Json; 
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMNotificationDTOService(HttpClient http) : INotificationDTOService
    {
        public Task ArchiveNotificationAsync(int notificationId, string currentUserId, bool isAdmin)
        {
            throw new NotImplementedException();
        }

        public Task CreateNotificationAsync(NotificationDTO notificationDTO)
        {
            throw new NotImplementedException();
        }

        public Task<List<NotificationDTO>> GetForCurrentUserAsync(UserInfo userInfo, int take = 20)
        {
            throw new NotImplementedException();
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

        public Task<List<NotificationDTO>> GetForUserAsAdminAsync(string targetUserId, UserInfo adminUserInfo, int take = 20)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUnreadCountForCurrentUserAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task MarkViewedAsync(int notificationId, string currentUserId)
        {
            throw new NotImplementedException();
        }

        public Task RestoreNotificationAsync(int notificationId, string currentUserId, bool isAdmin)
        {
            throw new NotImplementedException();
        }

        public Task<List<NotificationDTO>> GetForCurrentUserAsync(string currentUserId, int take = 20)
        {
            throw new NotImplementedException();
        }
    }
}
