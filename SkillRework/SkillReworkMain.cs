using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Common.Entities.Characters;
using Harmony;

namespace PhoenixRising.SkillRework
{
    public class SkillReworkMain
    {
        // New config field.
        internal static Settings Config;
        internal static string LogPath;
        internal static string ModDirectory;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void MainMod(Func<string, object, object> api)
        {
            // Read config and assign to config field.
            Config = api("config", null) as Settings ?? new Settings();
            // Path for own logging
            ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            LogPath = Path.Combine(ModDirectory, "SkillRework.log");
            // Initialize Logger
            Logger.Initialize(LogPath, Config.Debug, ModDirectory, nameof(SkillReworkMain));

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
            string className, ability;
            string[] abilities;
            foreach (AbilityTrackDef classSpecDef in ClassSpecDefs)
            {
                className = Helper.Classes.First(cn => classSpecDef.name.Contains(cn));
                abilities = Config.ClassSkills[className][Helper.Row[0]];
                if (classSpecDef.AbilitiesByLevel.Length != abilities.Length)
                {
                    Logger.Always("Not enough or too much level skills for 1st row are configured, some may not be set!");
                    Logger.Always("Class preset: " + className);
                    Logger.Always("Number of skills configured (should be 7): " + abilities.Length);
                }
                for (int i = 0; i < classSpecDef.AbilitiesByLevel.Length && i < abilities.Length; i++)
                {
                    if (i != 0 && i != 3) // 3 = secondary class selector and 0 = main class proficiency skipped, main class is in the config but also skipped here to prevent prevent bugs by misconfiguration
                    {
                        if (Helper.abilityNameToDefMap.ContainsKey(abilities[i]))
                        {
                            ability = Helper.abilityNameToDefMap[abilities[i]];
                            classSpecDef.AbilitiesByLevel[i].Ability = Repo.GetAllDefs<TacticalAbilityDef>().First(tad => tad.name.Contains(ability));
                            classSpecDef.AbilitiesByLevel[i].Ability.CharacterProgressionData.SkillPointCost = Helper.SPperLevel[i];
                            classSpecDef.AbilitiesByLevel[i].Ability.CharacterProgressionData.MutagenCost = Helper.SPperLevel[i];
                            Logger.Debug("Class '" + className + "' level " + i + 1 + " skill set to: " + classSpecDef.AbilitiesByLevel[i].Ability.ViewElementDef.DisplayName1.LocalizeEnglish());
                        }
                    }
                }
            }


            // Patch all Harmony patches
            HarmonyInstance.Create("SkillRework.PhoenixRising").PatchAll();

            if (Config.CreateNewJsonForAbilities)
            {
                string key;
                SortedDictionary<string, string> sortedDict = new SortedDictionary<string, string>();
                foreach (TacticalAbilityDef tad in Repo.GetAllDefs<TacticalAbilityDef>())
                {
                    if (tad.ViewElementDef != null && tad.CharacterProgressionData != null)
                    {
                        if (tad.ViewElementDef.DisplayName1 == null ||
                            tad.ViewElementDef.DisplayName1.LocalizeEnglish() == "" ||
                            tad.ViewElementDef.DisplayName1.LocalizeEnglish().Contains("<!-MISSING KEY") ||
                            tad.ViewElementDef.DisplayName1.LocalizeEnglish().Contains("NEEDS TEXT"))
                        {
                            key = "";
                            string[] parts = tad.name.Split('_');
                            for (int i = 0; i < (parts.Length - 1); i++)
                            {
                                key += parts[i];
                            }
                        }
                        else
                        {
                            key = tad.ViewElementDef.DisplayName1.LocalizeEnglish();
                        }
                        while (sortedDict.ContainsKey(key))
                        {
                            key += "+";
                        }
                        string value = tad.name;
                        sortedDict.Add(key, value);
                    }
                }
                Dictionary<string, string> outDict = sortedDict.ToDictionary(kv => kv.Key, kv => kv.Value);
                _ = Helper.WriteJson(Helper.AbilityNameToDefJson, outDict, true);
            }

            // Modnix logging
            _ = api("log verbose", "Mod Initialised.");
        }
    }
}
