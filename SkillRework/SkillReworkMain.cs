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
        internal static string ManagedDirectory;
        internal static string TexturesDirectory;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();

        public static void MainMod(Func<string, object, object> api)
        {
            // Read config and assign to config field.
            Config = api("config", null) as Settings ?? new Settings();
            // Path for own logging
            ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Path to texture files
            ManagedDirectory = Path.Combine(ModDirectory, "Assets", "Presets");
            // Path to texture files
            TexturesDirectory = Path.Combine(ModDirectory, "Assets", "Textures");

            // Initialize Logger
            LogPath = Path.Combine(ModDirectory, "SkillRework.log");
            Logger.Initialize(LogPath, Config.Debug, ModDirectory, nameof(SkillReworkMain));

            // Initialize Helper
            Helper.Initialize();

            // Generate some GUIDs
            //for (int i = 0; i < 100; i++)
            //{
            //    Logger.Always(Guid.NewGuid().ToString(), false);
            //}

            // Apply skill modifications
            SkillModifications.ApplyChanges();

            // Generate the main specialization as configured
            MainSpecModification.GenerateMainSpec();

            // Patch all Harmony patches
            HarmonyInstance.Create("SkillRework.PhoenixRising").PatchAll();

            try
            {
                // If configured, a new list of all usable abilities (with progression field) will be created.
                // This list is used to map readable names from config to the definitions in the Repo.
                // IMPORTANT: Currently the new created file must be included to assembly and new complied, otherwise new added abilities will not work!
                // TODO:
                // a) Scrap internal JSONs and only read from external files (easy solution, but probably bad performance)
                // b) Find a way to write to internal JSONs
                if (Config.CreateNewJsonFiles)
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
                    Helper.WriteJson(Helper.AbilitiesJsonFileName, outDict, true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            // Modnix logging
            _ = api("log verbose", "Mod Initialised.");
        }
    }
}
