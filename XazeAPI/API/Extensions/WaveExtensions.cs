// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using Respawning;
using Respawning.Waves;
using Respawning.Waves.Generic;

namespace XazeAPI.API.Extensions
{
    public static class WaveExtensions
    {
        public static bool IsBlocked(this TimeBasedWave wave)
        {
            if (WaveManager.State == WaveQueueState.WaveSpawning)
            {
                return true;
            }

            if (wave.Timer.IsPaused || wave.Timer.IsForcefullyPaused)
            {
                return true;
            }

            if (wave is not ILimitedWave { RespawnTokens: > 0 })
            {
                return true;
            }

            return false;
        }
    }
}
