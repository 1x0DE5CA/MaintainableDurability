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

            if (EffectiveMaxHitPoints == 0 && parent is Apparel apparel)
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

                var postArDamage = damage * (1 - armorRating);

                finalDamage = postArDamage * MaintainableDurabilitySettings.MdDamageFactor;
            }

            var newDurability = finalDamage / victim.MaxHitPoints;
            _currentDurability = Mathf.Max(0, _currentDurability - newDurability);
            EffectiveMaxHitPoints = Mathf.RoundToInt(victim.MaxHitPoints * _currentDurability);
        }

        public override string CompTipStringExtra()
        {
            return $"Maintenance: {_currentDurability.ToStringPercent()}";
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref EffectiveMaxHitPoints, "effectiveMaxHitPoints");
        }
    }
}