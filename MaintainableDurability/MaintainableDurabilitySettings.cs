using Verse;

namespace MaintainableDurability
{
    public class MaintainableDurabilitySettings : ModSettings
    {
        public static float MdDamageFactor = 0.5f;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref MdDamageFactor, "mdDamageFactor", 0.5f);
        }
    }
}