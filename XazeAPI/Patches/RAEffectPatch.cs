// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using CustomPlayerEffects;
using HarmonyLib;
using Utils;
using XazeAPI.API.EffectStacks;
using XazeAPI.API.Extensions;

namespace XazeAPI.Patches;

public static class RAEffectPatches
{
    [HarmonyPatchCategory(APILoader.PatchGroup)]
    [HarmonyPatch(typeof(EffectCommand), nameof(EffectCommand.Execute))]
    public static class EffectCmdPatch
    {
        public static bool Prefix(EffectCommand __instance, ArraySegment<string> arguments, ICommandSender sender, ref string response, ref bool __result)
        {
            if (!sender.CheckPermission(PlayerPermissions.Effects, out response))
            {
                __result = false;
                return false;
            }
            if (arguments.Count < 4)
            {
                response = "To execute this command provide at least 4 arguments!\nUsage: " + __instance.Command + " " + __instance.DisplayCommandUsage();
                __result = false;
                return false;
            }
            string effectName = arguments.At(0);
            if (!byte.TryParse(arguments.At(1), out var intensity))
            {
                response = string.Concat("Effect intensity must be a byte value between 0-255.\nUsage: ", __instance.Command, " ", __instance.DisplayCommandUsage(), "'");
                __result = false;
                return false;
            }

            if (!float.TryParse(arguments.At(2), out var duration))
            {
                response = string.Concat("Effect duration must be a valid float value.\nUsage: ", __instance.Command, " ", __instance.DisplayCommandUsage(), "'");
                __result = false;
                return false;
            }

            var list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 3, out _);
            if (list == null || list.Count == 0)
            {
                response = "Couldn't find any player(s) using the specified arguments.";
                __result = false;
                return false;
            }
            int num2 = 0;
            foreach (ReferenceHub hub in list)
            {
                if (hub == null || !hub.playerEffectsController.TryGetEffect(effectName, out var effect)) 
                    continue;

                var effectType = effect.GetType();
                if (EffectStackManager.BlacklistedEffects.Contains(effectType))
                    effect.ServerSetState(intensity, duration);
                else if (intensity > 0)
                    hub.AddEffect(effectType, intensity, duration);
                else
                    hub.RemoveEffect(effectType);
                
                ServerLogs.AddLog(ServerLogs.Modules.Administrative,
                    $"{sender.LogName} applied a status effect {effectName} for player {hub.LoggedNameFromRefHub()}. Intensity: {intensity} - Duration: {duration}.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                num2++;
            }
            response = $"Done! The request affected {num2} player{((num2 == 1) ? "!" : "s!")}";
            
            __result = true;
            return false;
        }
    }

    [HarmonyPatchCategory(APILoader.PatchGroup)]
    [HarmonyPatch(typeof(ClearEffectsCommand), nameof(ClearEffectsCommand.Execute))]
    public static class ClearEffectsCmdPatch
    {
        public static bool Prefix(ClearEffectsCommand __instance, ArraySegment<string> arguments, ICommandSender sender, ref string response, ref bool __result)
        {
            if (!sender.CheckPermission(PlayerPermissions.Effects, out response))
            {
                __result = false;
                return false;
            }
            if (arguments.Count == 0)
            {
                response = "To execute this command provide at least 1 arguments!\nUsage: " + __instance.Command + " " + string.Join(" ", __instance.Usage);
                __result = false;
                return false;
            }

            var list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out _);
            int num = 0;
            foreach (ReferenceHub hub in list)
            {
                if (hub == null) 
                    continue;
                
                foreach (var t in hub.playerEffectsController.AllEffects)
                {
                    hub.RemoveEffect(t.GetType());
                }
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " clear all effects for player " + hub.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                num++;
            }
            response = $"Done! The request affected {num} player{((num == 1) ? "!" : "s!")}";
            __result = true;
            return false;
        }
    }
}