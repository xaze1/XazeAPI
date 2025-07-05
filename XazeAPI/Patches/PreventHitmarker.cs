// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using XazeAPI.API.Events;

namespace XazeAPI.Patches
{
    [HarmonyPatchCategory(APILoader.PatchGroup)]
    [HarmonyPriority(800)]
    [HarmonyPatch(typeof(Hitmarker), nameof(Hitmarker.CheckHitmarkerPerms))]
    public class PreventHitmarker
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Remove the last two instructions (ldc.i4.1 + ret)
            // But first check that last instructions are what you expect to avoid errors
            int count = newInstructions.Count;
            if (count < 2 
                || newInstructions[count - 1].opcode != OpCodes.Ret 
                || newInstructions[count - 2].opcode != OpCodes.Ldc_I4_1)
            {
                // Just yield original if unexpected
                foreach (var instr in newInstructions)
                    yield return instr;
                ListPool<CodeInstruction>.Shared.Return(newInstructions);
                yield break;
            }

            newInstructions.RemoveAt(count - 1); // ret
            newInstructions.RemoveAt(count - 2); // ldc.i4.1

            // Inject your custom call and return
            newInstructions.AddRange(new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0), // Load adh
                new CodeInstruction(OpCodes.Ldarg_1), // Load victim
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PreventHitmarker), nameof(CustomPreventHitmarker))),
                new CodeInstruction(OpCodes.Ret)
            });

            foreach (var instr in newInstructions)
                yield return instr;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }


        public static bool CustomPreventHitmarker(AttackerDamageHandler adh, ReferenceHub victim)
        {
            var args = new PreventHitmarkerEvent(adh, victim);
            XazeEvents.OnServerPreventHitmarker(args);
            return args.IsAllowed;
        }
        
        /*
        public static bool Prefix(AttackerDamageHandler adh, ReferenceHub victim, ref bool __result)
        {
            __result = false;
            if (victim.roleManager.CurrentRole is IHitmarkerPreventer hitmarkerPreventer && hitmarkerPreventer.TryPreventHitmarker(adh))
            {
                return false;
            }

            if (CustomRoleManager.TryGet(victim, out var manager) && manager.CurrentRole is IHitmarkerPreventer customPreventer && customPreventer.TryPreventHitmarker(adh))
            {
                return false;
            }

            foreach (var effect in victim.playerEffectsController.AllEffects)
            {
                if (effect is IHitmarkerPreventer preventer && effect.IsEnabled && preventer.TryPreventHitmarker(adh))
                {
                    return false;
                }
            }

            if (CustomEffectsController.TryGet(victim, out var controller))
            {
                foreach(var customEffect in controller.AllEffects)
                {
                    if (customEffect.IsEnabled && customEffect is IHitmarkerPreventer preventer && preventer.TryPreventHitmarker(adh))
                    {
                        return false;
                    }
                }
            }

            __result = true;
            return false;
        }*/
    }
}
