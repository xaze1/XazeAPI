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
using XazeAPI.API.EffectStacks;

namespace XazeAPI.Patches;

public static class EffectPatches
{
    [HarmonyPatchCategory(APILoader.PatchGroup)]
    [HarmonyPatch(typeof(StatusEffectBase))]
    public static class StatusEffectBasePatch
    {
        [HarmonyPatch(nameof(StatusEffectBase.ServerSetState))]
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

        [HarmonyPatch(nameof(StatusEffectBase.ForceIntensity))]
        public static bool Prefix(StatusEffectBase __instance, byte value)
        {
            if (EffectStackManager.IsInternalCall || !EffectStackManager.TryGet(__instance.Hub, out var manager) || __instance == null)
                return true;

            var effectType = __instance.GetType();
            if (EffectStackManager.BlacklistedEffects.Contains(effectType))
                return true;
            
            if (value == 0)
            {
                manager.RemoveStacks(effectType);
                return false;
            }

            manager.AddStack(effectType, new EffectStack
            {
                Intensity = value,
                Duration = __instance.Duration,
                MaxIntensity = __instance is CokeBase<ICokeStack> cokeBase? (byte)cokeBase.StackMultipliers.Length : __instance.MaxIntensity,
            });
            __instance.Duration = 0;
            return false;
        }
    }
}