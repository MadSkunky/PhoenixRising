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
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Common.Entities.Characters;

namespace PhoenixRising.SkillRework
{
    class HarmonyPatches
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = SkillReworkMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();

        // Change personal abilities (3rd row skills) by configured settings
        [HarmonyPatch(typeof(FactionCharacterGenerator), "GenerateUnit", new Type[] { typeof(GeoFaction), typeof(TacCharacterDef) })]
        internal static class GenerateUnit_Patches
        {
            private static bool ShouldGeneratePersonalAbilities;
            // Called before 'GenerateUnit' -> PREFIX.
            private static bool Prefix(TacCharacterDef template)
            {
                // Save ShouldGeneratePersonalAbilities for postfix call, necessary? 
                ShouldGeneratePersonalAbilities = template.Data.LevelProgression.ShouldGeneratePersonalAbilities;
                Logger.Debug("-------------------------------------------------------------");
                Logger.Debug("PREFIX GenerateUnit called:");
                Logger.Debug("ShouldGeneragePersonalAbilities: " + ShouldGeneratePersonalAbilities);
                Logger.Debug("-------------------------------------------------------------");

                return true;
            }

            // Called after 'GenerateUnit' -> POSTFIX.
            private static void Postfix(ref GeoUnitDescriptor __result)
            {
                try
                {
                    // Probably not necessary, just to be safe ;-)
                    if (__result != null && ShouldGeneratePersonalAbilities && __result.UnitType.IsHuman && !__result.UnitType.IsMutoid && !__result.UnitType.TemplateDef.IsAlien)
                    {
                        string faction = __result.Faction.GetPPName();
                        string className = __result.ClassTag.className;
                        BaseCharacterStats stats = __result.BonusStats;
                        Dictionary<string, Dictionary<string, string>> ConfigClassSpec = Config.ClassSpecs.First(cs => className.Contains(cs.Key)).Value;

                        // Temporary dictionary to collect the configured perks
                        Dictionary<string, string> tempDict = new Dictionary<string, string>();
                        tempDict.AddRange(ConfigClassSpec[ClassKey.PersSpecs]);
                        tempDict.AddRange(Config.FactionSkills[faction]);

                        // Select random proficiency perk(s) that doesn't conflict with existing class or already existant perks
                        int safeguard = 0;
                        bool usedFound = true;
                        Random rnd = new Random((int)DateTime.Now.Ticks);
                        KeyValuePair<string, Dictionary<string, string>> kvProfSkills;
                        do
                        {
                            kvProfSkills = Config.ProficiencySkills.GetRandomElement(rnd);
                            usedFound = kvProfSkills.Value.Values.Any(skill => tempDict.Values.Contains(skill))
                                        || (Config.RadomSkillExclusionMap.ContainsKey(kvProfSkills.Key) && Config.RadomSkillExclusionMap[kvProfSkills.Key].Contains(className));
                            safeguard++;
                        } while (usedFound && safeguard <= Config.ProficiencySkills.Count * 2);
                        tempDict.AddRange(kvProfSkills.Value);

                        // Select random background perk that doesn't conflict with class (e.g. Jetpack proficiency with Heavy) or existant perks
                        safeguard = 0;
                        usedFound = true;
                        string bgSkill;
                        do
                        {
                            bgSkill = Config.BackgroundPerks.GetRandomElement(rnd);
                            usedFound = tempDict.Values.Contains(bgSkill)
                                        || (Config.RadomSkillExclusionMap.ContainsKey(bgSkill) && Config.RadomSkillExclusionMap[bgSkill].Contains(className));
                            safeguard++;
                        } while (usedFound && safeguard <= Config.BackgroundPerks.Length * 2);
                        tempDict.Add(PersonalLevel.BGP, bgSkill);

                        // Place the collected skills at the configured position in the 3rd line (e.g. personal ability line)
                        int index = -1;
                        string ability;
                        TacticalAbilityDef tacticalAbilityDef;
                        foreach (KeyValuePair<string, string> kvp in tempDict)
                        {
                            index = Array.IndexOf(Config.OrderOfPersonalSkills, kvp.Key);
                            //Logger.Debug("Index: " + index + " Skill: " + kvp.Value);
                            ability = Helper.AbilityNameToDefMap[kvp.Value];
                            tacticalAbilityDef = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains(ability));
                            if (index >= 0 && index < 7 && tacticalAbilityDef != null)
                            {
                                // only necessary to give special SP to personal abilities. Be careful, SP cost are global per ability, regardless where this ability is set!
                                //tacticalAbilityDef.CharacterProgressionData.SkillPointCost = 10;
                                __result.Progression.PersonalAbilities[index] = tacticalAbilityDef;
                            }
                        }

                        // Soome debug outputs in logging file
                        if (Config.Debug >= 2)
                        {
                            Logger.Debug("POSTFIX GenerateUnit called:");
                            Logger.Debug("      Faction: " + faction);
                            Logger.Debug("        Class: " + className);
                            Logger.Debug("    Endurance: " + stats.Endurance);
                            Logger.Debug("     Strength: " + stats.Strength);
                            Logger.Debug("    Willpower: " + stats.Willpower);
                            Logger.Debug("        Speed: " + stats.Speed);
                            Logger.Debug("-------------------------------------------------------------");
                            AbilityTrackSlot[] ats = __result.Progression.MainSpecDef.AbilityTrack.AbilitiesByLevel;
                            for (int i = 0; i < ats.Length; i++)
                            {
                                Logger.Debug("    MainSpec 1: " + ats[i]?.Ability?.ViewElementDef.DisplayName1.LocalizeEnglish());
                            }
                            Dictionary<int, TacticalAbilityDef> tad = __result.Progression.PersonalAbilities;
                            for (int i = 0; i < ats.Length; i++)
                            {
                                Logger.Debug("PersonalSpec 1: " + (tad.ContainsKey(i) ? tad[i].ViewElementDef.DisplayName1.LocalizeEnglish() : "none"));
                            }
                            Logger.Debug("-------------------------------------------------------------");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        // Learn personal ability level 1 right by creation of the unit
        [HarmonyPatch(typeof(GeoUnitDescriptor), "GenerateProgression")]
        internal static class GenerateProgression_Patches
        {
            private static void Postfix(ref CharacterProgression __result)
            {
                try
                {
                    // Personal ability 0 = first skill in the row
                    TacticalAbilityDef persAbility0 = __result.PersonalAbilityTrack.AbilitiesByLevel[0].Ability;
                    if (!__result.Abilities.Contains(persAbility0))
                    {
                        __result.AddAbility(persAbility0);
                    }
                    Logger.Debug("Ability added: " + persAbility0);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

    }
}
