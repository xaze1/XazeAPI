// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerStatsSystem;

namespace XazeAPI.Patches
{
    [HarmonyPatch(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.ProcessDamage))]
    public class AttackerDamageHandlerPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            var ldarg1Indices = Enumerable.Range(0, newInstructions.Count)
                .Where(i => newInstructions[i].opcode == OpCodes.Ldarg_1)
                .ToList();

            if (ldarg1Indices.Count < 3)
            {
                // Not enough occurrences
                foreach (var instr in newInstructions)
                    yield return instr;

                ListPool<CodeInstruction>.Shared.Return(newInstructions);
                yield break;
            }

            int insertIndex = ldarg1Indices[2]; // third ldarg.1

            newInstructions.InsertRange(insertIndex, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AttackerDamageHandlerPatch), nameof(CustomEffectsMethod)))
            });
            
            foreach (var t in newInstructions)
                yield return t;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        public static void CustomEffectsMethod(AttackerDamageHandler __instance)
        {
            /*
            try
            {
                if (CustomEffectsController.TryGet(ply, out var controller))
                {
                    foreach (CustomEffectBase customEffect in controller.AllEffects)
                    {
                        if (customEffect is not IFriendlyFireModifier ffMod || !customEffect.IsEnabled ||
                            !ffMod.AllowFriendlyFire(__instance.Damage, __instance, __instance.Hitbox))
                        {
                            continue;
                        }
                        
                        __instance.ForceFullFriendlyFire = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error($"[CustomEffects - AtkHandlerPatch] Failed applying custom effects\n" + ex);
            }*/
        }
    }
}
