using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
//using System.Text.Json;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Common.Entities.Characters;
using Newtonsoft.Json;

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

            if (Config.CreateNewJsonForAbilities)
            {
                SaveAbilitiesAsJson();
            }

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
            //string className, ability;
            //string[] abilities;
            //foreach (AbilityTrackDef classSpecDef in ClassSpecDefs)
            //{
            //    className = Settings.Classes.First(cn => classSpecDef.name.Contains(cn));
            //    abilities = Config.MainSkillsForClass[className];
            //    //string[] classAbilities = GetClassAbilities(classSpecDef.name);
            //    for (int i = 0; i < classSpecDef.AbilitiesByLevel.Length; i++)
            //    {
            //        if (abilities[i] != null) // null = secondary class selector, [0] = main class proficiency will be set by config helper
            //        {
            //            ability = Settings.abilityDefToName.First(kvp => kvp.Value.Contains(abilities[i])).Key;
            //            classSpecDef.AbilitiesByLevel[i].Ability = Repo.GetAllDefs<TacticalAbilityDef>().First(tad => tad.name.Contains(ability));
            //            classSpecDef.AbilitiesByLevel[i].Ability.CharacterProgressionData.SkillPointCost = Settings.SPperLevel[i];
            //            classSpecDef.AbilitiesByLevel[i].Ability.CharacterProgressionData.MutagenCost = Settings.SPperLevel[i];
            //        }
            //    }
            //}


            // Patch all Harmony patches
            //HarmonyInstance.Create("SkillRework.PhoenixRising").PatchAll();

            // Modnix logging
            api("log verbose", "Mod Initialised.");
        }

        public static void SaveAbilitiesAsJson ()
        {
            string key;
            SortedDictionary<string, string> tempDict = new SortedDictionary<string, string>();
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
                    while (tempDict.ContainsKey(key))
                    {
                        key += "+";
                    }
                    string value = tad.name;
                    tempDict.Add(key, value);
                }
            }
            Dictionary<string, string> outDict = tempDict.ToDictionary(kv => kv.Key, kv => kv.Value);
            string jsonString = JsonConvert.SerializeObject(outDict, Formatting.Indented);
            File.WriteAllText(Path.Combine(ModDirectory, "AbilityDefToNameDict.json"), jsonString);
        }
    }
}
