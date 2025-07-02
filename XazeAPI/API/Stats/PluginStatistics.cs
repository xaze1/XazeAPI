using System;

namespace XazeAPI.API.Stats
{
    public class PluginStatistics
    {
        public static int CriticalExceptions { get; private set; } = 0;
        public static int Exceptions { get; private set; } = 0;
        public static DateTimeOffset LoadTime { get; set; }
        public static DateTimeOffset LoadedTime { get; set; }
        public static TimeSpan StartTime => LoadedTime - LoadTime;

        public static void ExceptionCaught(bool critical)
        {
            if (critical)
            {
                CriticalExceptions += 1;
            }

            Exceptions += 1;
        }
    }
}
