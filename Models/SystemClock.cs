using System.Diagnostics;

namespace HappyFarm.Models
{
    internal static class SystemClock
    {
        private static Stopwatch _stopwatch = new Stopwatch();

        static SystemClock()
        {
            _stopwatch.Start();
        }

        public static long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;
    }
}
