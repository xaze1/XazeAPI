// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using HarmonyLib;
using PlayerStatsSystem;
using XazeAPI.API.AudioCore.FakePlayers;

namespace XazeAPI.Patches
{
    [HarmonyPatchCategory(APILoader.PatchGroup)]
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.DealDamage))]
    public class DamagePatch
    {
        public static bool Prefix(PlayerStats __instance, DamageHandlerBase handler)
        {
            if (!AudioManager.ActiveFakes.Contains(__instance._hub))
            {
                return true;
            }

            return false;
        }
    }
    
    [HarmonyPatchCategory(APILoader.PatchGroup)]
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    public class KillPatch
    {
        public static bool Prefix(PlayerStats __instance, DamageHandlerBase handler)
        {
            if (!AudioManager.ActiveFakes.Contains(__instance._hub))
            {
                return true;
            }

            return false;
        }
    }
}
