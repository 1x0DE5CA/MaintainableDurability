using Verse;

namespace MaintainableDurability
{
    public class MaintainableDurabilitySettings : ModSettings
    {
        public static float MdDamageFactor = 0.5f;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref MdDamageFactor, "MdDamageFactor", 0.5f);
            base.ExposeData();
        }
    }
}