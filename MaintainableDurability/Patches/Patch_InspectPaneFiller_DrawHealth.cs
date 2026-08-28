using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MaintainableDurability.Comps;
using RimWorld;
using Verse;

namespace MaintainableDurability.Patches
{
    [HarmonyPatch(typeof(InspectPaneFiller), nameof(InspectPaneFiller.DrawHealth))]
    public class Patch_InspectPaneFiller_DrawHealth
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var matcher = new CodeMatcher(instructions, il);

            var overwriteWhatever = AccessTools.Method(typeof(Patch_InspectPaneFiller_DrawHealth), nameof(OverrideWhatever));

            matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldc_R4, 16f)
                )
                .ThrowIfInvalid("[MaintainableDurability] Transpiler failed to find target pattern")
                .Insert(
                        new CodeInstruction(OpCodes.Ldloca, 1),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Call, overwriteWhatever)
                    );

            return matcher.InstructionEnumeration();
        }

        public static float OverrideWhatever(float width, ref string label, Thing t)
        {
            if (!(t is Pawn) && t != null && t.TryGetComp<CompMaintenanceDurability>(out var comp))
            {
                label = $"{t.HitPoints.ToStringCached()} / {comp.EffectiveMaxHitPoints.ToStringCached()}";
                if (comp.EffectiveMaxHitPoints < t.MaxHitPoints)
                    label += $" ({t.MaxHitPoints.ToStringCached()})";

                width = 102f;
            }

            return width;
        }
    }
}