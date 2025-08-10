using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

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
        public static async Task<UserInfo?> GetUserInfoAsync(Task<AuthenticationState> authStateTask)
        {
            if (authStateTask is null) return null!;

            AuthenticationState authState = await authStateTask;
            Console.WriteLine($" IsAuthenticated: {authState.User.Identity?.IsAuthenticated}");
            return UserInfoFactory.FromClaims(authState.User);

        }

        /// <summary>
        /// Retrieves user information based on the provided authentication state.
        /// </summary>
        /// <param name="authState">The authentication state containing the user's claims principal.</param>
        /// <returns>A <see cref="UserInfo"/> object representing the user's information.</returns>
        public static UserInfo? GetUserInfo(AuthenticationState authState)
        {
            _ = authState.User;
            return UserInfoFactory.FromClaims(authState.User);
        }

        /// <summary>
        /// use with API Controllers
        /// </summary>
        /// <param name="user"></param>
        /// <returns>UserInfo</returns>
        public static UserInfo? GetUserInfo(ClaimsPrincipal user)
        {
            return UserInfoFactory.FromClaims(user);
        }
    }
}
