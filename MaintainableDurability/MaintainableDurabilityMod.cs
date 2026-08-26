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

            MaintainableDurabilitySettings.MdDamageFactor = DrawSlider(
                listingStandard,
                "Maintenance durability damage factor",
                MaintainableDurabilitySettings.MdDamageFactor,
                0f,
                1f);

            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory() => "Maintainable Durability";

        private static float DrawSlider(Listing_Standard listingStandard, string label, float val, float minVal, float maxVal)
        {
            var lineRect = listingStandard.GetRect(30f);

            var labelRect = new Rect(lineRect.x, lineRect.y, lineRect.width * 0.4f, lineRect.height);
            var sliderRect = new Rect(lineRect.x + lineRect.width * 0.5f, lineRect.y - 2.7f, lineRect.width * 0.4f, lineRect.height);
            var inputRect = new Rect(lineRect.x + lineRect.width * 0.92f, lineRect.y, lineRect.width * 0.05f, lineRect.height - 4f);

            Widgets.Label(labelRect, label);

            var newVal = Widgets.HorizontalSlider(sliderRect, val, minVal, maxVal, true);
            var textBuffer = newVal.ToString("F2");

            Widgets.TextFieldNumeric(inputRect, ref newVal, ref textBuffer, minVal, maxVal);

            return newVal;
        }
    }
}