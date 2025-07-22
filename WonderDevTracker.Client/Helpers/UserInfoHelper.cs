using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace WonderDevTracker.Client.Helpers
{
    public static class UserInfoHelper
    {
        //UserInfo derive from:
        // -> Task<AuthenticationState> 
        //-> AuthenticationState
        // -> ClaimsPrincipal

        /// <summary>
        /// Use with a cascading parameter(AuthenticatedComponentBase) 
        /// </summary>
        /// <param name="authStateTask"></param>
        /// <returns></returns>
        public static async Task<UserInfo> GetUserInfoAsync(Task<AuthenticationState> authStateTask)
        {
            if (authStateTask is null) return null!;

            AuthenticationState authState = await authStateTask;
            return GetUserInfo(authState.User);

        }

        /// <summary>
        /// Retrieves user information based on the provided authentication state.
        /// </summary>
        /// <param name="authState">The authentication state containing the user's claims principal.</param>
        /// <returns>A <see cref="UserInfo"/> object representing the user's information.</returns>
        public static UserInfo GetUserInfo(AuthenticationState authState)
        {
            ClaimsPrincipal user = authState.User;
            return GetUserInfo(user);
        }

        /// <summary>
        /// use with API Controllers
        /// </summary>
        /// <param name="user"></param>
        /// <returns>UserInfo</returns>
        public static UserInfo GetUserInfo(ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = user.FindFirst(ClaimTypes.Email)?.Value;
                var firstName = user.FindFirst(nameof(UserInfo.FirstName))?.Value;
                var lastName = user.FindFirst(nameof(UserInfo.LastName))?.Value;
                var companyId = user.FindFirst(nameof(UserInfo.CompanyId))?.Value;
                var profilePictureUrl = user.FindFirst(nameof(UserInfo.ProfilePictureUrl))?.Value;
                var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);

                if (string.IsNullOrEmpty(userId) ||
                    string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(firstName) ||
                    string.IsNullOrEmpty(lastName) ||
                    string.IsNullOrEmpty(companyId) ||
                    string.IsNullOrEmpty(profilePictureUrl) 
                    )
                {
                    return null!;
                }
                return new UserInfo
                {
                    UserId = userId,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    CompanyId = int.Parse(companyId),
                    ProfilePictureUrl = profilePictureUrl,
                    Roles = [.. roles]
                };
            }
            catch (Exception)
            {

                return null!;
            }
        }

    }
}
