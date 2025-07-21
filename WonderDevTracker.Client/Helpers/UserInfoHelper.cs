using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace WonderDevTracker.Client.Helpers
{
    public static class UserInfoHelper
    {
        //called server-side
        public static UserInfo GetUserInfo(AuthenticationState authState)
        {
            var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = authState.User.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = authState.User.FindFirst("FirstName")?.Value;
            var lastName = authState.User.FindFirst("LastName")?.Value;

            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(lastName))
                return null!;

            UserInfo userInfo = new()
            {
                UserId = userId,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
            return userInfo;
        }


        //called client-side
        public static async Task<UserInfo> GetUserInfoAsync(Task<AuthenticationState> authStateTask)
        {
            if (authStateTask is null) return null!;
            var authState = await authStateTask;
            System.Security.Claims.ClaimsPrincipal user = authState.User;

            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = user.FindFirst(ClaimTypes.Email)?.Value;
                var firstName = user.FindFirst("FirstName")?.Value;
                var lastName = user.FindFirst("LastName")?.Value;
                return new UserInfo
                {
                    UserId = userId ?? string.Empty,
                    Email = email ?? string.Empty,
                    FirstName = firstName ?? string.Empty,
                    LastName = lastName ?? string.Empty
                };
            }
            catch (Exception)
            {

                return null!;
            }
        }
    }
}
