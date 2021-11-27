using System;
using Harmony;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Geoscape.Core;

namespace PhoenixRising.SkillRework
{
    class HarmonyPatches
    {
        // Get config, definition repository and shared data
        //private static readonly Settings Config = SkillReworkMain.Config;
        //private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();

        // This "tag" allows Harmony to find this class and apply it as a patch.
        [HarmonyPatch(typeof(FactionCharacterGenerator), "GeneratePersonalAbilities", new Type[] { typeof(int), typeof(LevelProgressionDef) })]

        // Class can be any name, but must be static.
        internal static class GeneratePersonalAbilities_Patches
        {
            // Called before 'GeneratePersonalAbilities' -> PREFIX.
            private static void Prefix(FactionCharacterGenerator __instance, int abilitiesCount, LevelProgressionDef levelDef)
            {
                Logger.Always("PREFIX GeneratePersonalAbilities called:");
                Logger.Always("          instance: " + __instance);
                Logger.Always("     abilitiesCout: " + abilitiesCount);
                Logger.Always("          levelDef: " + levelDef.ToString());
            }

            // Called after 'GeneratePersonalAbilities' -> POSTFIX.
            private static void Postfix(FactionCharacterGenerator __instance)
            {
                Logger.Always("POSTFIX GeneratePersonalAbilities called:");
                Logger.Always("     ___instance.toString(): " + __instance.ToString());
            }
        }
    }
}
