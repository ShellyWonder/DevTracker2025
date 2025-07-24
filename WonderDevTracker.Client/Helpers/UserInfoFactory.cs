using System.Security.Claims;

namespace WonderDevTracker.Client.Helpers
{
    public static class UserInfoFactory
    {
        public static UserInfo? FromClaims(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = user.FindFirst(nameof(UserInfo.FirstName))?.Value;
            var lastName = user.FindFirst(nameof(UserInfo.LastName))?.Value;
            var companyId = user.FindFirst(nameof(UserInfo.CompanyId))?.Value;
            var profilePictureUrl = user.FindFirst(nameof(UserInfo.ProfilePictureUrl))?.Value;
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var missing = new List<string>();
            if (string.IsNullOrEmpty(userId)) missing.Add("userId");
            if (string.IsNullOrEmpty(email)) missing.Add("email");
            if (string.IsNullOrEmpty(firstName)) missing.Add("firstName");
            if (string.IsNullOrEmpty(lastName)) missing.Add("lastName");
            if (string.IsNullOrEmpty(companyId)) missing.Add("companyId");
            if (string.IsNullOrEmpty(profilePictureUrl)) missing.Add("profilePictureUrl");
            if (!roles.Any()) missing.Add("roles");

            if (missing.Any())
            {
                Console.WriteLine($"❌ UserInfo.FromClaims: missing values — {string.Join(", ", missing)}");
                return null;
            }

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

        public static IEnumerable<Claim> ToClaims(this UserInfo userInfo)
        {
            yield return new Claim(ClaimTypes.NameIdentifier, userInfo.UserId);
            yield return new Claim(ClaimTypes.Name, userInfo.Email);
            yield return new Claim(ClaimTypes.Email, userInfo.Email);
            yield return new Claim(nameof(UserInfo.FirstName), userInfo.FirstName);
            yield return new Claim(nameof(UserInfo.LastName), userInfo.LastName);
            yield return new Claim(nameof(UserInfo.CompanyId), userInfo.CompanyId.ToString());
            yield return new Claim(nameof(UserInfo.ProfilePictureUrl), userInfo.ProfilePictureUrl);

            foreach (var role in userInfo.Roles)
            {
                yield return new Claim(ClaimTypes.Role, role);
            }
        }

    }
}
