// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using CustomPlayerEffects;
using HarmonyLib;
using Mirror;
using UnityEngine;

namespace XazeAPI.Patches;

public static class EffectPatches
{
    [HarmonyPatchCategory(APILoader.PatchGroup)]
    [HarmonyPatch(typeof(StatusEffectBase), nameof(StatusEffectBase.ServerSetState))]
    public static class ServerSetStatePatch
    {
        public static bool Prefix(StatusEffectBase __instance, byte intensity, float duration, bool addDuration)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning($"[Server] function '{nameof(StatusEffectBase.ServerSetState)}' called when server was not active");
            }
            
            __instance.ServerChangeDuration(duration, addDuration);
            __instance.Intensity = intensity;
            return false;
        }
    }
}