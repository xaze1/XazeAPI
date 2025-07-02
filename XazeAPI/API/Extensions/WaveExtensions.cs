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

            if (wave.Timer.IsPaused)
            {
                return true;
            }

            if (wave is ILimitedWave limitedWave && limitedWave.RespawnTokens > 0)
            {
                return false;
            }

            return false;
        }
    }
}
