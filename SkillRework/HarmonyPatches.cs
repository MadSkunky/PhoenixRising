using System;
using Harmony;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Geoscape.Core;
using PhoenixPoint.Geoscape.Levels;
using static PhoenixPoint.Geoscape.Entities.GeoUnitDescriptor;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Geoscape.Entities;

namespace PhoenixRising.SkillRework
{
    class HarmonyPatches
    {
        // Get config, definition repository and shared data
        //private static readonly Settings Config = SkillReworkMain.Config;
        //private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();

        // This "tag" allows Harmony to find this class and apply it as a patch.
        [HarmonyPatch(typeof(FactionCharacterGenerator), "GenerateUnit", new Type[] { typeof(GeoFaction), typeof(TacCharacterDef) })]

        // Class can be any name, but must be static.
        internal static class GenerateUnit_Patches
        {
            private static GeoFaction Faction;
            private static bool ProgressionIsValid;
            // Called before 'GenerateUnit' -> PREFIX.
            private static bool Prefix(FactionCharacterGenerator __instance, GeoFaction faction, TacCharacterDef template)
            {
                Faction = faction;
                ProgressionIsValid = template.Data.LevelProgression.IsValid;
                Logger.Always("-------------------------------------------------------------");
                Logger.Always("PREFIX GenerateUnit called:");
                Logger.Always("           faction: " + Faction.GetPPName());
                Logger.Always("LvlProgression is valid: " + ProgressionIsValid);
                Logger.Always("-------------------------------------------------------------");

                return true;
            }

            // Called after 'GenerateUnit' -> POSTFIX.
            private static void Postfix(FactionCharacterGenerator __instance, ref GeoUnitDescriptor __result)
            {
                Logger.Always("POSTFIX GenerateUnit called:");
                Logger.Always("       Faction: " + Faction.GetPPName());
                Logger.Always("         Class: " + __result.ClassTag.className);
                Logger.Always("valid progression: " + ProgressionIsValid);
                Logger.Always("         MainSpec 1: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[0]?.Ability?.ViewElementDef.Name);
                Logger.Always("         MainSpec 2: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[1]?.Ability?.ViewElementDef.Name);
                Logger.Always("         MainSpec 3: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[2]?.Ability?.ViewElementDef.Name);
                Logger.Always("         MainSpec 4: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[3]?.Ability?.ViewElementDef.Name);
                Logger.Always("         MainSpec 5: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[4]?.Ability?.ViewElementDef.Name);
                Logger.Always("         MainSpec 6: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[5]?.Ability?.ViewElementDef.Name);
                Logger.Always("         MainSpec 7: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[6]?.Ability?.ViewElementDef.Name);
                Logger.Always("     PersonalSpec 1: " + (__result.Progression.PersonalAbilities.ContainsKey(0) ? __result.Progression.PersonalAbilities[0].ViewElementDef.Name : "none"));
                Logger.Always("     PersonalSpec 2: " + (__result.Progression.PersonalAbilities.ContainsKey(1) ? __result.Progression.PersonalAbilities[1].ViewElementDef.Name : "none"));
                Logger.Always("     PersonalSpec 3: " + (__result.Progression.PersonalAbilities.ContainsKey(2) ? __result.Progression.PersonalAbilities[2].ViewElementDef.Name : "none"));
                Logger.Always("     PersonalSpec 4: " + (__result.Progression.PersonalAbilities.ContainsKey(3) ? __result.Progression.PersonalAbilities[3].ViewElementDef.Name : "none"));
                Logger.Always("     PersonalSpec 5: " + (__result.Progression.PersonalAbilities.ContainsKey(4) ? __result.Progression.PersonalAbilities[4].ViewElementDef.Name : "none"));
                Logger.Always("     PersonalSpec 6: " + (__result.Progression.PersonalAbilities.ContainsKey(5) ? __result.Progression.PersonalAbilities[5].ViewElementDef.Name : "none"));
                Logger.Always("     PersonalSpec 7: " + (__result.Progression.PersonalAbilities.ContainsKey(6) ? __result.Progression.PersonalAbilities[6].ViewElementDef.Name : "none"));
                Logger.Always("-------------------------------------------------------------");
            }
        }
    }
}
