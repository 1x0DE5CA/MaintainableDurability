using HarmonyLib;
using MaintainableDurability.Comps;
using RimWorld;
using Verse;

namespace MaintainableDurability.Patches
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
    public static class Patch_Thing_TakeDamage
    {
        [HarmonyPrefix]
        public static void Prefix(Thing __instance, DamageInfo dinfo)
        {
            if (__instance is Apparel apparel && apparel.TryGetComp<CompMaintenanceDurability>(out var comp))
            {
                comp.TakeDamage(dinfo, apparel);

                if (apparel.HitPoints > comp.EffectiveMaxHitPoints)
                    apparel.HitPoints = comp.EffectiveMaxHitPoints;
            }
        }
    }
}