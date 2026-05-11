namespace WonderDevTracker.Client.Helpers
{
    public static class UserDisplayHelper
    {
        public static string GetInitials(string? firstName, string? lastName, string? fallbackName = null)
        {
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                return $"{firstName[0]}{lastName[0]}".ToUpperInvariant();
            }

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                return firstName[0].ToString().ToUpperInvariant();
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                return lastName[0].ToString().ToUpperInvariant();
            }

            if (!string.IsNullOrWhiteSpace(fallbackName))
            {
                var parts = fallbackName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 1)
                    return parts[0][0].ToString().ToUpperInvariant();

                return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
            }
            return "?";
        }
    }
}
