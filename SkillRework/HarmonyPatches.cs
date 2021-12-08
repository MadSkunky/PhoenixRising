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
using System.Collections;

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
                        string ability;
                        int spCost = 0;
                        TacticalAbilityDef tacticalAbilityDef;
                        BaseCharacterStats stats = __result.BonusStats;
                        string[] ppOrder = Config.OrderOfPersonalPerks;
                        // Temporary dictionary to collect the configured perks
                        Dictionary<string, string> tempDict = new Dictionary<string, string>();
                        // Exclusion list for random selection, initialized with main spec skills
                        List<string> exclusionList = new List<string>(Config.ClassSpecializations.FirstOrDefault(cs => cs.ClassName.Equals(className)).MainSpec);
                        
                        for (int i = 0; i < ppOrder.Length; i++)
                        {
                            PersonalPerksDef personalPerksDef = Config.PersonalPerks.FirstOrDefault(pp => pp.PerkKey.Equals(ppOrder[i]));
                            if (!personalPerksDef.IsDefaultValue())
                            {
                                (ability, spCost) = personalPerksDef.GetPerk(Config, className, faction, exclusionList);
                                if (ability != null)
                                {
                                    tacticalAbilityDef = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains(ability));
                                    if (i >= 0 && i < 7 && tacticalAbilityDef != null)
                                    {
                                        // Set SP cost to personal ability. Be careful, SP cost are global per ability, regardless where this ability is set!
                                        tacticalAbilityDef.CharacterProgressionData.SkillPointCost = spCost;
                                        __result.Progression.PersonalAbilities[i] = tacticalAbilityDef;
                                        //exclusionList.Add(ability);
                                    }
                                }
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
                    if (Config.LearnFirstPersonalSkill)
                    {
                        // Personal ability 0 = first skill in the row
                        TacticalAbilityDef persAbility0 = __result.PersonalAbilityTrack.AbilitiesByLevel[0].Ability;
                        if (!__result.Abilities.Contains(persAbility0))
                        {
                            __result.AddAbility(persAbility0);
                        }
                        Logger.Debug("Ability added: " + persAbility0);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
