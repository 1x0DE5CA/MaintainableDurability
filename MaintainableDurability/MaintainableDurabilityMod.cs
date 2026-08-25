using UnityEngine;
using Verse;

namespace MaintainableDurability
{
    public class MaintainableDurabilityMod : Mod
    {
        public MaintainableDurabilityMod(ModContentPack content) : base(content) { }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            MaintainableDurabilitySettings.MdDamageFactor = listingStandard.SliderLabeled(
                $"Maintenance Durability damage factor: {MaintainableDurabilitySettings.MdDamageFactor:F2}",
                MaintainableDurabilitySettings.MdDamageFactor,
                0f,
                1f);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory() => "Maintainable Durability";
    }
}