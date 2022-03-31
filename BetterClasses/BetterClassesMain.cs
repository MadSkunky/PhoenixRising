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
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Geoscape.Entities;
using UnityEngine;
using Base.UI.MessageBox;
using Base.Utils.GameConsole;
using PhoenixPoint.Geoscape.Events;

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
        internal static string LocalizationDirectory;
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
            // Path to localization CSVs
            LocalizationDirectory = Path.Combine(ModDirectory, "Assets", "Localization");

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

        // Current and last saved ODI level
        //public static int CurrentODI_Level;
        //public static int LastSavedODI_Level;
        //// All SDI (ODI) event IDs, levels as array, index 0 - 19
        //public static readonly string[] ODI_EventIDs = new string[]
        //{
        //    "SDI_01",
        //    "SDI_02",
        //    "SDI_03",
        //    "SDI_04",
        //    "SDI_05",
        //    "SDI_06",
        //    "SDI_07",
        //    "SDI_08",
        //    "SDI_09",
        //    "SDI_10",
        //    "SDI_11",
        //    "SDI_12",
        //    "SDI_13",
        //    "SDI_14",
        //    "SDI_15",
        //    "SDI_16",
        //    "SDI_17",
        //    "SDI_18",
        //    "SDI_19",
        //    "SDI_20"
        //};
        //// Harmony patch to gather some game stats from the alien faction (pandorans) when geo level starts (campaign start, game loaded, after tactical missions)
        //[HarmonyPatch(typeof(GeoAlienFaction), "OnLevelStart")]
        //internal static class BC_GeoAlienFaction_OnLevelStart_patch
        //{
        //    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        //    private static void Postfix(GeoAlienFaction __instance, int ____evolutionProgress)
        //    {
        //        Calculate_ODI_Level(__instance, ____evolutionProgress, true);
        //    }
        //}
        //// Harmony patch to gather some game stats from the alien faction (pandorans) each day in game
        //[HarmonyPatch(typeof(GeoAlienFaction), "UpdateFactionDaily")]
        //internal static class BC_GeoAlienFaction_UpdateFactionDaily_patch
        //{
        //    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        //    private static void Postfix(GeoAlienFaction __instance, int ____evolutionProgress)
        //    {
        //        Calculate_ODI_Level(__instance, ____evolutionProgress);
        //    }
        //}
        //internal static void Calculate_ODI_Level(GeoAlienFaction geoAlienFaction, int evolutionProgress, bool onLevelStart = false)
        //{
        //    try
        //    {
        //        // Lost population to accelerate the ODI progress, value increasing from 0 to 1, 0 means no loss = start of campaign, 1 = no population left
        //        // A player that keeps the havens alive can prolong the ODI progress
        //        float lostPopulation = (float)(geoAlienFaction.GeoLevel.StartingPopulation - geoAlienFaction.GeoLevel.CurrentPopulation) / geoAlienFaction.GeoLevel.StartingPopulation;
        //        // Calculate the current ODI progress value with the given variables, here basically evolutionProgress and accelerated by lost population
        //        int currentODI_Progress = (int)(evolutionProgress * (1 + lostPopulation));
        //        // Set a maximum number to determine the upper limit from when the maximum ODI level is reached
        //        // Here just an example assuming 3000 is a value somehwere after pandorans evolve to Citadel
        //        int maxODI_Progress = 3000;
        //        // Index of last element of the ODI event ID array is Length - 1
        //        int ODI_EventIDs_LastIndex = ODI_EventIDs.Length - 1;
        //        // Cap at above defined max progress, after that the Index will not longer get increased wiht higher progress
        //        CurrentODI_Level = Mathf.Min(ODI_EventIDs_LastIndex, Mathf.RoundToInt((float)currentODI_Progress * ODI_EventIDs_LastIndex / maxODI_Progress));
        //        // If this is called by OnLevelStart() and CurrentODI_Level != LastSavedODI_Level set the last saved to the current calculated value
        //        //  to prevent triggering events on each geo level start (campaign start, game loaded, after tactical missions)
        //        // Needs some tests so we don't miss events after tactical that can increase the evolution progress
        //        if (onLevelStart && CurrentODI_Level != LastSavedODI_Level)
        //        {
        //            LastSavedODI_Level = CurrentODI_Level;
        //        }
        //        // Get the Event ID from array dependent on calculated level index
        //        string eventID = ODI_EventIDs[CurrentODI_Level];
        //        if (CurrentODI_Level != LastSavedODI_Level)
        //        {
        //            GeoLevelController geoLevelController = geoAlienFaction.GeoLevel;
        //            GeoscapeEventContext geoscapeEventContext = new GeoscapeEventContext(geoLevelController.AlienFaction, geoLevelController.ViewerFaction);
        //            geoLevelController.EventSystem.TriggerGeoscapeEvent(eventID, geoscapeEventContext);
        //            LastSavedODI_Level = CurrentODI_Level;
        //        }
        //
        //        //// Trigger SDI event the hacky way by executing a console command
        //        //IConsole console = UnityEngine.Object.FindObjectOfType<GameConsoleWindow>();
        //        //if (evolutionProgress > 100 && evolutionProgress < 200)
        //        //{
        //        //    console.ExecuteCommandLine($"geo_event_trigger {eventID}");
        //        //}
        //
        //        // Logging some gathered in game values
        //        DateTime currentGameTime = geoAlienFaction.GeoLevel.GameController.CurrentGameTime;
        //        int percPopulation = Mathf.RoundToInt((float)geoAlienFaction.GeoLevel.CurrentPopulation * 100 / geoAlienFaction.GeoLevel.StartingPopulation);
        //        string message = string.Concat($" Gathered ingame values by GeoAlienFaction.OnLevelStart() and .UpdateFactionDaily():\n",
        //                                       $"Game time: {currentGameTime}\n",
        //                                       $"Alien evolution progress: {evolutionProgress}\n",
        //                                       $"Current population: {percPopulation}%\n",
        //                                       $"------------------------------------------------------------------------------------------------------");
        //        Logger.Debug(message);
        //
        //        // Show a message box with gathered values
        //        //geoAlienFaction.GeoLevel.View.RequestGamePause();
        //        //GameUtl.GetMessageBox().ShowSimplePrompt(message, MessageBoxIcon.Information, MessageBoxButtons.OK, null);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error(e);
        //    }
        //}
    }
}
