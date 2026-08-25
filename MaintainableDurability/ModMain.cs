using HarmonyLib;
using Verse;

namespace MaintainableDurability
{
    [StaticConstructorOnStartup]
    public static class ModMain
    {
        static ModMain()
        {
            var harmony = new Harmony("MaintainableDurability");

            harmony.PatchAll();
        }
    }
}