using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Helpers
{
    public static class DateTimeHelper
    {
        public static string FormatTimestamp(DateTimeOffset created)
        {
            var now = DateTimeOffset.Now;
            var local = created.ToLocalTime();
            var diff = now - local;

            if (diff.TotalSeconds < 60)
                return "Just now";

            if (diff.TotalMinutes < 60)
                return $"{(int)diff.TotalMinutes}m ago";

            if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours}h ago";

            if (local.Date == now.Date.AddDays(-1))
                return "Yesterday";

            if (local.Date == now.Date)
                return $"Today, {local:t}";

            if (local.Year == now.Year)
                return local.ToString("MMM d");

            return local.ToString("MMM d, yyyy");
        }


            private static string GetCommentTimestamp(TicketCommentDTO comment)
            {
                //if (comment.Edited == true)
                //return $"Edited {DateTimeHelper.FormatTimestamp(comment.Edited.Value)}";

                return DateTimeHelper.FormatTimestamp(comment.Created);
            }
    }
}

