using System;
using System.Collections.Generic;
using HarmonyLib;
using MaintainableDurability.Comps;
using RimWorld;
using UnityEngine;
using Verse;

namespace MaintainableDurability.Patches
{
    [HarmonyPatch(nameof(WorkGiver_DoBill), "TryFindBestBillIngredients")]
    public class Patch_WorkGiver_DoBill_TryFindBestBillIngredients
    {
        private static readonly FastInvokeHandler TryFindBestIngredientsHelper =
            MethodInvoker.GetHandler(AccessTools.Method(typeof(WorkGiver_DoBill), "TryFindBestIngredientsHelper"));

        private static readonly FastInvokeHandler IsUsableIngredient =
            MethodInvoker.GetHandler(AccessTools.Method(typeof(WorkGiver_DoBill), "IsUsableIngredient"));

        private static readonly FastInvokeHandler TryFindBestBillIngredientsInSet =
            MethodInvoker.GetHandler(AccessTools.Method(typeof(WorkGiver_DoBill), "TryFindBestBillIngredientsInSet"));

        private static readonly FastInvokeHandler GetBillGiverRootCell =
            MethodInvoker.GetHandler(AccessTools.Method(typeof(WorkGiver_DoBill), "GetBillGiverRootCell"));

        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, Bill bill, Pawn pawn, Thing billGiver, List<ThingCount> chosen,
            List<IngredientCount> missingIngredients)
        {
            if (bill.recipe.defName != "MendApparel")
                return true;

            IngredientCount apparelIngredient = null;
            foreach (var ingredient in bill.recipe.ingredients)
            {
                if (ingredient.filter != null && ingredient.SummaryFilterFirst.StartsWith("Apparel"))
                {
                    apparelIngredient = ingredient;
                    break;
                }
            }

            if (apparelIngredient == null)
                return true;

            var chosenApparel = new List<ThingCount>();

            var apparelOnlyIngredients = new List<IngredientCount> { apparelIngredient };

            Predicate<Thing> apparelThingValidator = t => (bool)IsUsableIngredient(null, t, bill);

            Predicate<List<Thing>> foundApparelIngredientsAndChoose = foundThings =>
                (bool) TryFindBestBillIngredientsInSet(
                    null,
                    foundThings,
                    bill,
                    chosenApparel,
                    (IntVec3) GetBillGiverRootCell(null, billGiver, pawn),
                    billGiver is Pawn,
                    missingIngredients
                );

            var apparelFound = (bool) TryFindBestIngredientsHelper(
                    null,
                    apparelThingValidator,
                    foundApparelIngredientsAndChoose,
                    apparelOnlyIngredients,
                    pawn,
                    billGiver,
                    chosenApparel,
                    bill.ingredientSearchRadius
                );

            if (!apparelFound || chosenApparel.Count == 0)
            {
                __result = false;
                return false;
            }

            var apparelThing = chosenApparel[0].Thing;

            if (!apparelThing.TryGetComp<CompMaintenanceDurability>(out var comp))
                return true;

            var costStuffCount = apparelThing.def.CostStuffCount;
            var currentHp = apparelThing.HitPoints;
            var effectiveMaxHp = comp.EffectiveMaxHitPoints;
            var maxHp = apparelThing.MaxHitPoints;

            var effectiveCostStuff =
                Mathf.RoundToInt(costStuffCount * (Math.Abs(currentHp - effectiveMaxHp) / (float)maxHp));
            var finalCostStuff = Math.Max(1, effectiveCostStuff);

            Log.Message($"Final material cost would be: {finalCostStuff}");

            var thingStuffFilter = new ThingFilter();
            thingStuffFilter.SetAllow(apparelThing.Stuff, true);

            var thingStuffIngredient = new IngredientCount { filter = thingStuffFilter };
            thingStuffIngredient.SetBaseCount(finalCostStuff);

            var stuffOnlyIngredients = new List<IngredientCount> { thingStuffIngredient };

            Predicate<Thing> stuffThingValidator = t =>
                t.def == apparelThing.Stuff &&
                (bool)IsUsableIngredient(null, t, bill);

            var chosenStuff = new List<ThingCount>();
            var missingStuffIngredients = new List<IngredientCount>();

            var tryFindBestIngredientsInSet_NoMixHelperMI = AccessTools.Method(typeof(WorkGiver_DoBill), "TryFindBestIngredientsInSet_NoMixHelper");
            var tryFindBestIngredientsInSet_NoMixHelper = MethodInvoker.GetHandler(tryFindBestIngredientsInSet_NoMixHelperMI);

            Predicate<List<Thing>> foundStuffIngredientsAndChoose1 = foundThings =>
                (bool)tryFindBestIngredientsInSet_NoMixHelper(
                    null,
                    foundThings,
                    stuffOnlyIngredients,
                    chosenStuff,
                    (IntVec3) GetBillGiverRootCell(null, billGiver, pawn),
                    billGiver is Pawn,
                    missingStuffIngredients,
                    null
                );

            var stuffFound = (bool)TryFindBestIngredientsHelper(
                    null,
                    stuffThingValidator,
                    foundStuffIngredientsAndChoose1,
                    stuffOnlyIngredients,
                    pawn,
                    billGiver,
                    chosenStuff,
                    bill.ingredientSearchRadius
                );

            if (stuffFound)
                Log.Message("Stuff found");
            if (chosenStuff.Count > 0)
                Log.Message($"{chosenStuff[0].Thing}");

            if (stuffFound && chosenStuff.Count > 0)
            {
                chosen.Clear();
                chosen.Add(chosenApparel[0]);
                chosen.AddRange(chosenStuff);

                __result = true;
                return false;
            }

            __result = false;
            return false;
        }
    }
}