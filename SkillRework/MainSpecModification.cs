using System;
using System.Collections.Generic;
using System.Linq;
using Base.Defs;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Tactical.Entities.Abilities;

namespace PhoenixRising.SkillRework
{
    class MainSpecModification
    {
        public static bool GenerateMainSpec(DefRepository Repo, Settings Config)
        {
            try
            {
                LevelProgressionDef levelProgressionDef = Repo.GetAllDefs<LevelProgressionDef>().First(lpd => lpd.name.Contains("LevelProgressionDef"));
                int secondaryClassLevel = levelProgressionDef.SecondSpecializationLevel;
                int secondaryClassCost = levelProgressionDef.SecondSpecializationSpCost;
                List<AbilityTrackDef> ClassSpecDefs = new List<AbilityTrackDef>
                {
                    Repo.GetAllDefs<AbilityTrackDef>().First(atd => atd.name.Contains("AssaultSpecializationDef")),
                    Repo.GetAllDefs<AbilityTrackDef>().First(atd => atd.name.Contains("HeavySpecializationDef")),
                    Repo.GetAllDefs<AbilityTrackDef>().First(atd => atd.name.Contains("SniperSpecializationDef")),
                    Repo.GetAllDefs<AbilityTrackDef>().First(atd => atd.name.Contains("BerserkerSpecializationDef")),
                    Repo.GetAllDefs<AbilityTrackDef>().First(atd => atd.name.Contains("PriestSpecializationDef")),
                    Repo.GetAllDefs<AbilityTrackDef>().First(atd => atd.name.Contains("TechnicianSpecializationDef")),
                    Repo.GetAllDefs<AbilityTrackDef>().First(atd => atd.name.Contains("InfiltratorSpecializationDef"))
                };
                string ability;
                foreach (AbilityTrackDef classSpecDef in ClassSpecDefs)
                {
                    KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> ConfigClassSpec = Config.ClassSpecs.First(cs => classSpecDef.name.Contains(cs.Key));
                    string[] configMainSpec = ConfigClassSpec.Value[ClassKey.MainSpecs].Values.ToArray();
                    if (classSpecDef.AbilitiesByLevel.Length != configMainSpec.Length)
                    {
                        Logger.Always("Not enough or too much level skills for 1st row are configured, some may not be set!");
                        Logger.Always("Class preset: " + ConfigClassSpec.Key);
                        Logger.Always("Number of skills configured (should be 7): " + configMainSpec.Length);
                    }
                    for (int i = 0; i < classSpecDef.AbilitiesByLevel.Length && i < configMainSpec.Length; i++)
                    {
                        //Logger.Debug("Config MainSpec " + i + ": " + configMainSpec[i]);
                        if (i != 0 && i != 3) // 3 = secondary class selector and 0 = main class proficiency skipped, main class is in the config but also skipped here to prevent bugs by misconfiguration
                        {
                            if (Helper.AbilityNameToDefMap.ContainsKey(configMainSpec[i]))
                            {
                                ability = Helper.AbilityNameToDefMap[configMainSpec[i]];
                                classSpecDef.AbilitiesByLevel[i].Ability = Repo.GetAllDefs<TacticalAbilityDef>().First(tad => tad.name.Contains(ability));
                                classSpecDef.AbilitiesByLevel[i].Ability.CharacterProgressionData.SkillPointCost = Helper.SPperLevel[i];
                                classSpecDef.AbilitiesByLevel[i].Ability.CharacterProgressionData.MutagenCost = Helper.SPperLevel[i];
                                Logger.Debug("Class '" + ConfigClassSpec.Key + "' level " + i + 1 + " skill set to: " + classSpecDef.AbilitiesByLevel[i].Ability.ViewElementDef.DisplayName1.LocalizeEnglish());
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }
    }
}
