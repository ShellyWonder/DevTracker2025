namespace WonderDevTracker.Client.Helpers.Animation
{
    public class IndexTrackerHelper
    {
        private int _index = 0;

        public int Next() => _index++;

        public void Reset() => _index = 0;
    }
}
