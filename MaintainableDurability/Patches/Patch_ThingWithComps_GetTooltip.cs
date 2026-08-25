using HarmonyLib;
using MaintainableDurability.Comps;
using RimWorld;
using UnityEngine;
using Verse;

namespace MaintainableDurability.Patches
{
    [HarmonyPatch(typeof(ThingWithComps), nameof(ThingWithComps.GetTooltip))]
    public static class Patch_ThingWithComps_GetTooltip
    {
        [HarmonyPostfix]
        public static void Postfix(Thing __instance, ref TipSignal __result)
        {
            if (__instance is Apparel apparel)
            {
                var mdComp = apparel.TryGetComp<CompMaintenanceDurability>();
                if (mdComp != null)
                {
                    var hpDelimiter = __result.text.LastIndexOf("/");
                    var text = __result.text.Substring(0, hpDelimiter + 1);
                    text += $" {mdComp.EffectiveMaxHitPoints}";

                    if (mdComp.EffectiveMaxHitPoints < apparel.MaxHitPoints)
                        text += $" ({apparel.MaxHitPoints})".Colorize(Color.grey);

                    __result.text = text;
                }
            }
        }
    }
}