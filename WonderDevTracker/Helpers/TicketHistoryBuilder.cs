using WonderDevTracker.Models;

namespace WonderDevTracker.Helpers
{
    public sealed class TicketHistoryBuilder(List<TicketHistory> historyEntries, string userId, int ticketId, DateTimeOffset created)
    {
        private readonly List<TicketHistory> _historyEntries = historyEntries;
        private readonly string _userId = userId;
        private readonly int _ticketId = ticketId;
        private readonly DateTimeOffset _created = created;

        private TicketHistory CreateHistory(string description) => new()
        {
            UserId = _userId,
            TicketId = _ticketId,
            Created = _created,
            Description = description
        };

        public void AddIfChanged<T>(T oldValue, T newValue, Func<string> descriptionBuilder)
        {
            if (!Equals(oldValue, newValue))
            {
                _historyEntries.Add(CreateHistory(descriptionBuilder()));
            }
        }

        public void Add(string description)
            => _historyEntries.Add(CreateHistory(description));
    }

}
