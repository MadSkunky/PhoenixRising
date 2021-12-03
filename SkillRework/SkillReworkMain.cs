using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Tactical.Entities.Abilities;
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

            // Apply skill modifications
            SkillModifications.ApplyChanges(Repo, Shared);

            // Generate the main specialization as configured
            if( MainSpecModification.GenerateMainSpec(Repo, Config) )
            {
                // Patch all Harmony patches only if main spec was sucessful
                HarmonyInstance.Create("SkillRework.PhoenixRising").PatchAll();
            }

            // If configured a new list of all usable abilities (with progression field) will be created.
            // This list is used to map readable names from config to the definitions in the Repo
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
                _ = Helper.WriteJson(Helper.AbilitiesJsonFileName, outDict, true);
            }

            // Modnix logging
            _ = api("log verbose", "Mod Initialised.");
        }
    }
}
