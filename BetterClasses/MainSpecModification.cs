using System;
using System.Linq;
using Base.Core;
using Base.Defs;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Tactical.Entities.Abilities;

namespace PhoenixRising.BetterClasses
{
    class MainSpecModification
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;
        public static void GenerateMainSpec()
        {
            try
            {
                LevelProgressionDef levelProgressionDef = Repo.GetAllDefs<LevelProgressionDef>().FirstOrDefault(lpd => lpd.name.Contains("LevelProgressionDef"));
                int secondaryClassLevel = levelProgressionDef.SecondSpecializationLevel;
                int secondaryClassCost = levelProgressionDef.SecondSpecializationSpCost;
                string ability;
                foreach (AbilityTrackDef abilityTrackDef in Repo.GetAllDefs<AbilityTrackDef>())
                {
                    if (Config.ClassSpecializations.Any(c => abilityTrackDef.name.Contains(c.ClassName)))
                    {
                        ClassSpecDef classSpec = Config.ClassSpecializations.Find(c => abilityTrackDef.name.Contains(c.ClassName));
                        string[] configMainSpec = classSpec.MainSpec;
                        if (abilityTrackDef.AbilitiesByLevel.Length != configMainSpec.Length)
                        {
                            Logger.Always("----------------------------------------------------------------------------------------------------", false);
                            Logger.Always("Not enough or too much level skills for 1st row are configured, this one will NOT be set!");
                            Logger.Always("AbilityTrackDef name: " + abilityTrackDef.name);
                            Logger.Always("AbilityTrackDef number of abilities: " + abilityTrackDef.AbilitiesByLevel.Length);
                            Logger.Always("AbilityTrackDef number of abilities: " + abilityTrackDef.Abilities.Select(a => a.name).Join());
                            Logger.Always("Class preset: " + classSpec.ClassName);
                            Logger.Always("Number of skills configured (should be 7): " + configMainSpec.Length);
                            Logger.Always("----------------------------------------------------------------------------------------------------", false);
                        }
                        else
                        {
                            for (int i = 0; i < abilityTrackDef.AbilitiesByLevel.Length && i < configMainSpec.Length; i++)
                            {
                                // 0 = main class proficiency and 3 = secondary class selector skipped, main class is in the config but also skipped here to prevent bugs by misconfiguration
                                if (i != 0 && i != 3)
                                {
                                    if (Helper.AbilityNameToDefMap.ContainsKey(configMainSpec[i]))
                                    {
                                        ability = Helper.AbilityNameToDefMap[configMainSpec[i]];
                                        abilityTrackDef.AbilitiesByLevel[i].Ability = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains(ability));
                                        abilityTrackDef.AbilitiesByLevel[i].Ability.CharacterProgressionData.SkillPointCost = Helper.SPperLevel[i];
                                        abilityTrackDef.AbilitiesByLevel[i].Ability.CharacterProgressionData.MutagenCost = Helper.SPperLevel[i];
                                        Logger.Debug($"Class '{classSpec.ClassName}' level {i + 1} skill set to: {abilityTrackDef.AbilitiesByLevel[i].Ability.ViewElementDef.DisplayName1.LocalizeEnglish()}");
                                    }
                                }
                            }
                        }
                        Logger.Debug("----------------------------------------------------", false);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
