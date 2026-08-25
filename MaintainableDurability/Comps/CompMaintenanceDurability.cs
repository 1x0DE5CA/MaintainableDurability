using RimWorld;
using UnityEngine;
using Verse;

namespace MaintainableDurability.Comps
{
    public class CompMaintenanceDurability : ThingComp
    {
        private float _currentDurability = 1f;

        public int EffectiveMaxHitPoints;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            if (parent is Apparel apparel)
                EffectiveMaxHitPoints = apparel.MaxHitPoints;
        }

        public void TakeDamage(DamageInfo dinfo, Thing victim)
        {
            var damage = dinfo.Amount;
            float finalDamage;

            if (dinfo.Def == DamageDefOf.Deterioration)
            {
                finalDamage = dinfo.Amount;
            }
            else
            {
                var armorRatingStat = dinfo.Def.armorCategory.armorRatingStat;
                var armorRating = victim.GetStatValue(armorRatingStat);

                // var stuffStat = 0f;
                // if (armorRatingStat.label == StatDefOf.StuffPower_Armor_Blunt.label)
                // {
                //     stuffStat = victim.Stuff.statBases.GetStatValueFromList(StatDefOf.StuffPower_Armor_Blunt, 1);
                // }
                // if (armorRatingStat.label == StatDefOf.StuffPower_Armor_Sharp.label)
                // {
                //     stuffStat = victim.Stuff.statBases.GetStatValueFromList(StatDefOf.StuffPower_Armor_Sharp, 1);
                // }
                // if (armorRatingStat.label == StatDefOf.StuffPower_Armor_Heat.label)
                // {
                //     stuffStat = victim.Stuff.statBases.GetStatValueFromList(StatDefOf.StuffPower_Armor_Heat, 1);
                // }

                // if (stuffStat.CompareTo(1f) == 0)
                //     Log.Message($"No stuff stats for {armorRatingStat} on {victim.Label}, default value applied");
                // Log.Message($"Stats for {armorRatingStat}: {stuffStat}");

                var postArDamage = damage * (1 - armorRating);
                // var potDamage = damage * stuffStat;

                finalDamage = postArDamage * 0.25f;
            }

            var newDurability = finalDamage / victim.MaxHitPoints;
            _currentDurability = Mathf.Max(0, _currentDurability - newDurability);
            EffectiveMaxHitPoints = Mathf.RoundToInt(victim.MaxHitPoints * _currentDurability);
        }

        public override string CompTipStringExtra()
        {
            return $"Maintenance: {_currentDurability.ToStringPercent()}";
        }
    }
}