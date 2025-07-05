// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

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
