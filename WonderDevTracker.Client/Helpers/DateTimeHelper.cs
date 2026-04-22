namespace WonderDevTracker.Client.Helpers
{
    // NOTE: Do not call this directly in Razor.
    // Use TimestampComponent instead.
    public static class DateTimeHelper
    {
        public static DateTimeOffset GetNotificationDisplayTimestamp(
        DateTimeOffset created,
        DateTimeOffset? archivedAt)
        {
            return archivedAt ?? created;
        }

        public static string FormatTimestamp(DateTimeOffset timestamp)
        {
            var now = DateTimeOffset.Now;
            var local = timestamp.ToLocalTime();
            var diff = now - local;

            if (diff.TotalSeconds < 60)
                return "Just now";

            if (local.Date == now.Date)
            {
                if (diff.TotalMinutes < 60)
                    return $"{(int)diff.TotalMinutes}m ago";

                return $"{(int)diff.TotalHours}h ago";
            }

            if (local.Date == now.Date.AddDays(-1))
                return "Yesterday";

            if (local.Year == now.Year)
                return local.ToString("MMM d");

            return local.ToString("MMM d, yyyy");
        }
  
    }
}

