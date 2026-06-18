// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of __instance source tree.
// //
// // I <3 🦈s :3c

using HarmonyLib;
using PlayerStatsSystem;
using XazeAPI.API.Events;
using XazeAPI.API.Events.Handler;

namespace XazeAPI.Patches;

[HarmonyPatchCategory(APILoader.PatchGroup)]
[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.DealDamage))]
public class PlayerStatsPatch
{
    public static void Prefix(PlayerStats __instance, ref DamageHandlerBase handler)
    {
        ReferenceHub attacker = null;
        if (handler is AttackerDamageHandler attck)
            attacker = attck.Attacker.Hub;
        
        var ev = new PlayerHurting(attacker, __instance._hub, handler);
        XazeEvents.OnPlayerHurting(ev);
        handler = ev.DamageHandler;
    }
}