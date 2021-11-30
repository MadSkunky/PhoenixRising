using System;
using Harmony;
using PhoenixPoint.Geoscape.Core;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Geoscape.Entities;
using Base.Defs;
using Base.Core;
using System.Collections.Generic;
using System.Linq;
using Base;
using PhoenixPoint.Common.Entities;

namespace PhoenixRising.SkillRework
{
    class HarmonyPatches
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = SkillReworkMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
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
                // Save faction for postfix call, necessary? 
                Faction = faction;
                ProgressionIsValid = template.Data.LevelProgression.IsValid;
                Logger.Debug("-------------------------------------------------------------");
                Logger.Debug("PREFIX GenerateUnit called:");
                Logger.Debug("           faction: " + Faction.GetPPName());
                Logger.Debug("LvlProgression is valid: " + ProgressionIsValid);
                Logger.Debug("-------------------------------------------------------------");

                return true;
            }

            // Called after 'GenerateUnit' -> POSTFIX.
            private static void Postfix(FactionCharacterGenerator __instance, ref GeoUnitDescriptor __result)
            {
                string faction = __result.Faction.GetPPName();
                string className = __result.ClassTag.className;
                BaseCharacterStats stats = __result.BonusStats;
                Logger.Debug("POSTFIX GenerateUnit called:");
                Logger.Debug("       Faction: " + faction);
                Logger.Debug("         Class: " + className);
                Logger.Debug("       Endurance: " + stats.Endurance);
                Logger.Debug("        Strength: " + stats.Strength);
                Logger.Debug("       Willpower: " + stats.Willpower);
                Logger.Debug("           Speed: " + stats.Speed);

                string[] c1skills = Config.ClassSkills[className][Helper.Row[0]];
                string[] c3Skills = Config.ClassSkills[className][Helper.Row[1]];
                string[] f3Skills = Config.FactionSkills[faction];
                string[] allUsedSkills = c1skills.AddRangeToArray(c3Skills).AddRangeToArray(f3Skills);
                int safeguard = 0;
                KeyValuePair<string, string[]> kvProfSkills = Config.ProficiencySkills.GetRandomElement();
                bool usedFound = allUsedSkills.Contains(kvProfSkills.Value[0]) || allUsedSkills.Contains(kvProfSkills.Value[1]);
                while (usedFound && safeguard < Config.ProficiencySkills.Count)
                {
                    kvProfSkills = Config.ProficiencySkills.GetRandomElement();
                    usedFound = allUsedSkills.Contains(kvProfSkills.Value[0]) || allUsedSkills.Contains(kvProfSkills.Value[1]);
                    safeguard++;
                }
                string[] p3Skills = kvProfSkills.Value;
                allUsedSkills = allUsedSkills.AddRangeToArray(p3Skills);
                safeguard = 0;
                string b3Skill = Config.BackgroundPerks.GetRandomElement();
                usedFound = allUsedSkills.Contains(b3Skill);
                while (usedFound && safeguard < Config.BackgroundPerks.Length)
                {
                    b3Skill = Config.BackgroundPerks.GetRandomElement();
                    usedFound = allUsedSkills.Contains(b3Skill);
                    safeguard++;
                }

                Logger.Debug("valid progression: " + ProgressionIsValid);
                Logger.Debug("         MainSpec 1: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[0]?.Ability?.ViewElementDef.DisplayName1.LocalizeEnglish());
                Logger.Debug("         MainSpec 2: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[1]?.Ability?.ViewElementDef.DisplayName1.LocalizeEnglish());
                Logger.Debug("         MainSpec 3: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[2]?.Ability?.ViewElementDef.DisplayName1.LocalizeEnglish());
                Logger.Debug("         MainSpec 4: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[3]?.Ability?.ViewElementDef.DisplayName1.LocalizeEnglish());
                Logger.Debug("         MainSpec 5: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[4]?.Ability?.ViewElementDef.DisplayName1.LocalizeEnglish());
                Logger.Debug("         MainSpec 6: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[5]?.Ability?.ViewElementDef.DisplayName1.LocalizeEnglish());
                Logger.Debug("         MainSpec 7: " + __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel[6]?.Ability?.ViewElementDef.DisplayName1.LocalizeEnglish());
                Logger.Debug("Additional class skill 1: " + c3Skills[0]);
                Logger.Debug("Additional class skill 2: " + c3Skills[1]);
                Logger.Debug("         Faction skill 1: " + f3Skills[0]);
                Logger.Debug("         Faction skill 2: " + f3Skills[1]);
                Logger.Debug("     Proficiency skill 1: " + p3Skills[0]);
                Logger.Debug("     Proficiency skill 2: " + p3Skills[1]);
                Logger.Debug("        Background skill: " + b3Skill);

                //Logger.Debug("     PersonalSpec 1: " + (__result.Progression.PersonalAbilities.ContainsKey(0) ? __result.Progression.PersonalAbilities[0].ViewElementDef.Name : "none"));
                //Logger.Debug("     PersonalSpec 2: " + (__result.Progression.PersonalAbilities.ContainsKey(1) ? __result.Progression.PersonalAbilities[1].ViewElementDef.Name : "none"));
                //Logger.Debug("     PersonalSpec 3: " + (__result.Progression.PersonalAbilities.ContainsKey(2) ? __result.Progression.PersonalAbilities[2].ViewElementDef.Name : "none"));
                //Logger.Debug("     PersonalSpec 4: " + (__result.Progression.PersonalAbilities.ContainsKey(3) ? __result.Progression.PersonalAbilities[3].ViewElementDef.Name : "none"));
                //Logger.Debug("     PersonalSpec 5: " + (__result.Progression.PersonalAbilities.ContainsKey(4) ? __result.Progression.PersonalAbilities[4].ViewElementDef.Name : "none"));
                //Logger.Debug("     PersonalSpec 6: " + (__result.Progression.PersonalAbilities.ContainsKey(5) ? __result.Progression.PersonalAbilities[5].ViewElementDef.Name : "none"));
                //Logger.Debug("     PersonalSpec 7: " + (__result.Progression.PersonalAbilities.ContainsKey(6) ? __result.Progression.PersonalAbilities[6].ViewElementDef.Name : "none"));
                //Logger.Debug("-------------------------------------------------------------");
            }
        }
    }
}
