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
using PhoenixRising.BetterClasses.SkillModifications;
using PhoenixPoint.Geoscape.Events.Eventus;
using Base.Assets;
using PhoenixRising.BetterClasses.StoryRework;
using PhoenixRising.BetterClasses.VariousAdjustments;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Common.Entities.Items;

namespace PhoenixRising.BetterClasses
{
    public class BetterClassesMain
    {
        // New config field.
        internal static Settings Config;
        internal static string LogPath;
        internal static string ModDirectory;
        internal static string ManagedDirectory;
        internal static string TexturesDirectory;
        internal static bool doNotLocalize = true;
        internal static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        internal static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        internal static readonly AssetsManager assetsManager = GameUtl.GameComponent<AssetsManager>();

        public static void HomeMod(Func<string, object, object> api)
        {
            InitBetterClasses(api);

            ApplyDefChanges();

            string methodName = MethodBase.GetCurrentMethod().Name;
            int numRuntimeDefs = Repo.GetRuntimeDefs<BaseDef>(true).Count();
            Logger.Always("----------------------------------------------------------------------------------------------------", false);
            Logger.Always($"{methodName} end, number of RuntimeDefs: {numRuntimeDefs}");
            Logger.Always("----------------------------------------------------------------------------------------------------", false);
            // Modnix logging
            _ = api("log verbose", "HomeMod done, Mod Initialised.");
        }
        public static void GeoscapeOnHide()
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            int numRuntimeDefs = Repo.GetRuntimeDefs<BaseDef>(true).Count();
            Logger.Debug("----------------------------------------------------------------------------------------------------", false);
            Logger.Debug($"{methodName} start, number of RuntimeDefs: {numRuntimeDefs}");
            Logger.Debug("----------------------------------------------------------------------------------------------------", false);

            ApplyDefChanges();

            numRuntimeDefs = Repo.GetRuntimeDefs<BaseDef>(true).Count();
            Logger.Always("----------------------------------------------------------------------------------------------------", false);
            Logger.Always($"{methodName} end, number of RuntimeDefs: {numRuntimeDefs}");
            Logger.Always("----------------------------------------------------------------------------------------------------", false);
        }
        public static void TacticalOnHide()
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            int numRuntimeDefs = Repo.GetRuntimeDefs<BaseDef>(true).Count();
            Logger.Debug("----------------------------------------------------------------------------------------------------", false);
            Logger.Debug($"{methodName} start, number of RuntimeDefs: {numRuntimeDefs}");
            Logger.Debug("----------------------------------------------------------------------------------------------------", false);

            ApplyDefChanges();

            numRuntimeDefs = Repo.GetRuntimeDefs<BaseDef>(true).Count();
            Logger.Always("----------------------------------------------------------------------------------------------------", false);
            Logger.Always($"{methodName} end, number of RuntimeDefs: {numRuntimeDefs}");
            Logger.Always("----------------------------------------------------------------------------------------------------", false);
        }

        public static void InitBetterClasses(Func<string, object, object> api)
        {
            // Read config and assign to config field.
            Config = api("config", null) as Settings ?? new Settings();
            // Path for own logging
            ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Path to preset files
            ManagedDirectory = Path.Combine(ModDirectory, "Assets", "Presets");
            // Path to texture files
            TexturesDirectory = Path.Combine(ModDirectory, "Assets", "Textures");

            // Initialize Logger
            LogPath = Path.Combine(ModDirectory, "BetterClasses.log");
            Logger.Initialize(LogPath, Config.Debug, ModDirectory, nameof(BetterClassesMain));

            // Initialize Helper
            Helper.Initialize();

            // Set localization for project
            doNotLocalize = Config.DoNotLocalizeChangedTexts;

            // Patch all Harmony patches
            HarmonyInstance.Create("BetterClasses.PhoenixRising").PatchAll();

            // Print out ODI titles and texts
            //foreach (GeoscapeEventDef ged in Repo.GetAllDefs<GeoscapeEventDef>().Where(g => g.name.Contains("SDI_")))
            //{
            //    Logger.Always("----------------------------------------------------------------------------", false);
            //    Logger.Always("Event name: " + ged.name, false);
            //    Logger.Always("", false);
            //    Logger.Always("Title: " + ged.GeoscapeEventData.Title.Localize(), false);
            //    Logger.Always("Text:", false);
            //    Logger.Always(ged.GeoscapeEventData.Description.FirstOrDefault().General.Localize(), false);
            //    Logger.Always("", false);
            //}

            // Generate some GUIDs
            //for (int i = 0; i < 100; i++)
            //{
            //    Logger.Always(Guid.NewGuid().ToString(), false);
            //}
            // Modnix logging

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
        }

        public static void ApplyDefChanges()
        {
            // Apply skill modifications
            SkillModsMain.ApplyChanges();

            // Generate the main specialization as configured
            MainSpecModification.GenerateMainSpec();

            // Apply story rework changes (Voland)
            if (Config.ActivateStoryRework)
            {
                StoryReworkMain.ApplyChanges();
            }

            // Apply various adjustments
            VariousAdjustmentsMain.ApplyChanges();
        }
    }
}
