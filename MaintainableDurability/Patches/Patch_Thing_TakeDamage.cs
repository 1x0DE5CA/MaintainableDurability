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
        public static bool Prefix(Thing __instance, ref DamageWorker.DamageResult __result, DamageInfo dinfo)
        {
            if (__instance is Apparel apparel && apparel.TryGetComp<CompMaintenanceDurability>(out var comp))
            {
                comp.TakeDamage(dinfo, apparel);

                if (apparel.HitPoints > comp.EffectiveMaxHitPoints)
                    apparel.HitPoints = comp.EffectiveMaxHitPoints;

                if (dinfo.Def == DamageDefOf.Deterioration)
                {
                    __result = new DamageWorker.DamageResult();
                    return false;
                }
            }

            return true;
        }
    }
}