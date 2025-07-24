using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Security.Claims;
using System.Threading.Tasks;
using WonderDevTracker.Client.Components.UIComponents.GeneralComponents;
using static MudBlazor.Colors;

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
            Console.WriteLine($" IsAuthenticated: {authState.User.Identity?.IsAuthenticated}");
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
                var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();


                foreach (var claim in user.Claims)
                {
                    Console.WriteLine($"CLAIM: {claim.Type} = {claim.Value}");
                }
                if (!IsValidUserInfo(userId, email, firstName, lastName, companyId, profilePictureUrl, roles)) return null!;

                return new UserInfo
                {
                    UserId = userId!,
                    Email = email!,
                    FirstName = firstName!,
                    LastName = lastName!,
                    CompanyId = int.Parse(companyId!),
                    ProfilePictureUrl = profilePictureUrl!,
                    Roles = [.. roles]
                };
            }
            catch (Exception ex)
            {

                Console.WriteLine($" Exception in UserInfoHelper.GetUserInfo: {ex.Message}");
                return null!;
            }
        }
        private static bool IsValidUserInfo(
            string? userId,
            string? email,
            string? firstName,
            string? lastName,
            string? companyId,
            string? profilePictureUrl,
            List<string> roles)
        {
            List<string> missing = [];

            if (string.IsNullOrEmpty(userId)) missing.Add("userId");
            if (string.IsNullOrEmpty(email)) missing.Add("email");
            if (string.IsNullOrEmpty(firstName)) missing.Add("firstName");
            if (string.IsNullOrEmpty(lastName)) missing.Add("lastName");
            if (string.IsNullOrEmpty(companyId)) missing.Add("companyId");
            if (string.IsNullOrEmpty(profilePictureUrl)) missing.Add("profilePictureUrl");
            if (roles is null || roles.Count == 0) missing.Add("roles");

            if (missing.Any())
            {
                Console.WriteLine($"UserInfoHelper: Missing claims — {string.Join(", ", missing)}");
                return false;
            }

            return true;
        }

    }
}
