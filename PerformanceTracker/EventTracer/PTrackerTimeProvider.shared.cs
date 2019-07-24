using System.Diagnostics;

namespace PerformanceTracker
{
    public static class PTrackerTimeProvider
    {
        public static Stopwatch Source { get; }

        static PTrackerTimeProvider()
        {
            Source = new Stopwatch();
            Source.Start();
        }
    }
}