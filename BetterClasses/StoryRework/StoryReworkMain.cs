using AK.Wwise;
using Assets.Code.PhoenixPoint.Geoscape.Entities.Sites.TheMarketplace;
using Base.Defs;
using Base.Eventus.Filters;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Research;
using PhoenixPoint.Geoscape.Events;
using PhoenixPoint.Geoscape.Events.Conditions;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Geoscape.Events.Eventus.Filters;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Levels.Factions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PhoenixRising.BetterClasses.StoryRework
{
    class StoryReworkMain
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        private static Event AugeryChant = null;

        public static void ApplyChanges()
        {
            try
            {
                // Volands magic mission changes coding down from here
                //SDI TEST DOESN'T WORK
                //Timer and Trigger verified

                //GeoTimePassedEventFilterDef timerToClone = Repo.GetAllDefs<GeoTimePassedEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS0_TimePassed [GeoTimePassedEventFilterDef]"));
                //GeoTimePassedEventFilterDef newTimer = Helper.CreateDefFromClone(timerToClone, "0E7EB32D-3A54-4F12-A1B8-046E498DA040", "E_PROG_FS30_TimePassed [GeoTimePassedEventFilterDef]");
                //newTimer.TimePassedRaw = "1d0h";
                //newTimer.TimePassedHours = (float)24.0;
                //OrEventFilterDef TimerTriggerToClone = Repo.GetAllDefs<OrEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS0_MultipleTriggers [OrEventFilterDef]"));
                //OrEventFilterDef newTimerTrigger = Helper.CreateDefFromClone(TimerTriggerToClone, "C94E63CF-685B-4471-A3E3-3B0BC30ACD42", "E_PROG_FS30_MultipleTriggers [OrEventFilterDef]");
                //newTimerTrigger.OR_Filters[0] = newTimer;

                // Try using Sink_TimePassedEvent GS_TimePassed_Sink_EventDef

                //GeoscapeEventDef SDI01E = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("GS_TimePassed_Sink_EventDef"));
                //GeoscapeEventDef geoEventFS0 = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS0_GeoscapeEventDef"));
                //SDI01E.Filters[0] = newTimerTrigger;
                //SDI01E.GeoscapeEventData = geoEventFS0.GeoscapeEventData;
                //SDI01E.GeoscapeEventData.Mute = false;
                //SDI01E.GeoscapeEventData.EventID = "SDI01E";
                //SDI01E.GeoscapeEventData.Flavour = "SDI";
                //SDI01E.GeoscapeEventData.Title.LocalizationKey = "SDI_01_TITLE";
                //SDI01E.GeoscapeEventData.Description[0].General.LocalizationKey = "SDI_01_TEXT_GENERAL_0";
                //SDI01E.GeoscapeEventData.Choices[0].Text.LocalizationKey = "SDI_01_CHOICE_0_TEXT";
                //SDI01E.GeoscapeEventData.Description[0].Voiceover = null;

                //SDIStart.GeoscapeEventData.Title.LocalizationKey = "SDI_01_TITLE";
                //SDIStart.GeoscapeEventData.Description[0].General.LocalizationKey = "SDI_01_TEXT_GENERAL_0";
                //SDIStart.GeoscapeEventData.Choices[0].Text.LocalizationKey = "SDI_01_CHOICE_0_TEXT";

                //Attempts to use clone TimePassed trigger with SDI Event doesn't work, presumably because it takes SDI_Changed trigger
                //Clone FS9 Condition
                //GeoLevelConditionDef sourceConditionFS9 = Repo.GetAllDefs<GeoLevelConditionDef>().FirstOrDefault(ged => ged.name.Equals("[PROG_FS0] Condition 1"));
                //GeoLevelConditionDef conditionSDIStart = Helper.CreateDefFromClone(sourceConditionFS9, "3CA28779-D9D6-4D1D-B23C-8DB0CDEE417F", "[PROG_FS30] Condition 1]");
                //GeoscapeEventDef SDIStart = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS9_GeoscapeEventDef"));
                //SDIStart.Name = "GS_TimePassed";

                //Clone FS9 event
                //GeoscapeEventDef CloneforSDIStart = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS0_GeoscapeEventDef"));
                //GeoscapeEventDef SDIStart = Helper.CreateDefFromClone(CloneforSDIStart, "F9904A5D-B4C4-4046-AE4C-EAD224AF88CE", "PROG_FS30_GeoscapeEventDef");
                //SDIStart.Name = "PROG_FS30_GeoscapeEventDef";
                //SDIStart.GeoscapeEventData.EventID = "PROG_FS30";
                //SDIStart.Filters[0] = newTimerTrigger;
                //SDIStart.GeoscapeEventData.Conditions[0] = conditionSDIStart;
                //SDIStart.GeoscapeEventData.Flavour = "SDI";
                //SDIStart.GeoscapeEventData.Title.LocalizationKey = "SDI_01_TITLE";
                //SDIStart.GeoscapeEventData.Description[0].General.LocalizationKey = "SDI_01_TEXT_GENERAL_0";
                //SDIStart.GeoscapeEventData.Choices[0].Text.LocalizationKey = "SDI_01_CHOICE_0_TEXT";

                //Festering Skies changes
                // copy Augury chant from PROG_FS0 to PROG_FS9 and remove from PROG_FS0, because Augury doesn't happen and FS0 event will be used for a Sleeping Beauty Awakens
                GeoscapeEventDef geoEventFS0 = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS0_GeoscapeEventDef"));
                if (AugeryChant == null && geoEventFS0.GeoscapeEventData.Description[0].Voiceover != null)
                {
                    AugeryChant = geoEventFS0.GeoscapeEventData.Description[0].Voiceover;
                }
                GeoscapeEventDef geoEventFS9 = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS9_GeoscapeEventDef"));
                geoEventFS9.GeoscapeEventData.Description[0].Voiceover = AugeryChant;
                geoEventFS0.GeoscapeEventData.Description[0].Voiceover = null;
                geoEventFS9.GeoscapeEventData.Flavour = "";
                geoEventFS9.GeoscapeEventData.Choices[0].Outcome.OutcomeText.General.LocalizationKey = "PROG_FS9_OUTCOME";
                //set event timer for meteor arrival (Mount Egg)
                GeoTimePassedEventFilterDef timePassedFS9 = Repo.GetAllDefs<GeoTimePassedEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS9_TimePassed [GeoTimePassedEventFilterDef]"));
                timePassedFS9.TimePassedRaw = "2d0h";
                timePassedFS9.TimePassedHours = (float)48.0;
                // set event timer for former Augury, now A Sleeping Beauty Awakens
                GeoTimePassedEventFilterDef timePassedFS0 = Repo.GetAllDefs<GeoTimePassedEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS0_TimePassed [GeoTimePassedEventFilterDef]"));
                timePassedFS0.TimePassedRaw = "6d0h";
                timePassedFS0.TimePassedHours = (float)144.0;
                // set background and leader images for A Sleeping Beauty Awakens and break the panel in 2
                geoEventFS0.GeoscapeEventData.Flavour = "";
                geoEventFS0.GeoscapeEventData.Leader = "SY_Eileen";
                geoEventFS0.GeoscapeEventData.Choices[0].Outcome.OutcomeText.General.LocalizationKey = "PROG_FS0_TEXT_OUTCOME_0";
                // change leader image from Athena to Eileen for We Are Still Collating (former the Invitation)
                GeoscapeEventDef geoEventFS1 = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS1_GeoscapeEventDef"));
                geoEventFS1.GeoscapeEventData.Leader = "SY_Eileen";
                geoEventFS1.GeoscapeEventData.Choices[0].Outcome.OutcomeText.General.LocalizationKey = "PROG_FS1_OUTCOME";
                // Destroy Haven after mission
                GeoscapeEventDef geoEventFS1WIN = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS1_WIN_GeoscapeEventDef"));
                geoEventFS1WIN.GeoscapeEventData.Choices[0].Outcome.HavenPopulationChange = -20000;

                // set event timer for the former The Gift mission reveal, now The Hatching
                // currently this is unchanged from Vanilla, but here is the code to make the change if desired
                // GeoTimePassedEventFilterDef timePassedFS1 = RepoGeoscapeEvent.GetAllDefs<GeoTimePassedEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS1_TimePassed"));
                // timePassedFS1.TimePassedRaw = "8d0h";
                // timePassedFS1.TimePassedHours = 192;

            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        //// Harmony patch to change the reveal of alien bases when in scanner range, so increases the reveal chance instead of revealing it right away
        //[HarmonyPatch(typeof(GeoAlienFaction), "TryRevealAlienBase")]
        //internal static class BC_GeoAlienFaction_OnLevelStart_patch
        //{
        //    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        //    private static bool Prefix(ref bool __result, GeoSite site, GeoFaction revealToFaction, GeoLevelController ____level)
        //    {
        //        if (!site.GetVisible(revealToFaction))
        //        {
        //            GeoAlienBase component = site.GetComponent<GeoAlienBase>();
        //            if (revealToFaction is GeoPhoenixFaction && ((GeoPhoenixFaction)revealToFaction).IsSiteInBaseScannerRange(site, true))
        //            {
        //                component.IncrementBaseAttacksRevealCounter();
        //                // original code:
        //                //site.RevealSite(____level.PhoenixFaction);
        //                //__result = true;
        //                //return false;
        //            }
        //            if (component.CheckForBaseReveal())
        //            {
        //                site.RevealSite(____level.PhoenixFaction);
        //                __result = true;
        //                return false;
        //            }
        //            component.IncrementBaseAttacksRevealCounter();
        //        }
        //        __result = false;
        //        return false; // Return without calling the original method
        //    }
        //}
        
        //// Harmony patch to change the result of AllMissionsCompleted.get() to always true
        //[HarmonyPatch(typeof(GeoMarketplace), "get_AllMissionsCompleted")]
        //internal static class BC_GeoMarketplace_get_AllMissionsCompleted_patch
        //{
        //    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        //    private static bool Prefix(ref bool __result, GeoLevelController ____level, TheMarketplaceSettingsDef ____settings)
        //    {
        //        __result = ____level.EventSystem.GetVariable(____settings.NumberOfDLC5MissionsCompletedVariable) > 0;
        //        return false; // Return without calling the original method
        //    }
        //}
        //
        //// Current and last ODI level
        //public static int CurrentODI_Level = 0;
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
        //[HarmonyPatch(typeof(GeoAlienFaction), "OnAfterFactionsLevelStart")]
        //internal static class BC_GeoAlienFaction_OnAfterFactionsLevelStart_patch
        //{
        //    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        //    private static void Postfix(GeoAlienFaction __instance, int ____evolutionProgress)
        //    {
        //        Calculate_ODI_Level(__instance, ____evolutionProgress);
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
        //internal static void Calculate_ODI_Level(GeoAlienFaction geoAlienFaction, int evolutionProgress)
        //{
        //    try
        //    {
        //        // Index of last element of the ODI event ID array is Length - 1
        //        int ODI_EventIDs_LastIndex = ODI_EventIDs.Length - 1;
        //        // Set a maximum number to determine the upper limit from when the maximum ODI level is reached
        //        int maxODI_Progress = 470 * ODI_EventIDs_LastIndex;
        //        // Calculate the current ODI level = index for the ODI event ID array
        //        // Mathf.Min = cap the lavel at max index, after that the index will not longer get increased wiht higher progress
        //        CurrentODI_Level = Mathf.Min(ODI_EventIDs_LastIndex, evolutionProgress * ODI_EventIDs_LastIndex / maxODI_Progress);
        //        // Get the GeoLevelController to get access to the event system and the variable
        //        GeoLevelController geoLevelController = geoAlienFaction.GeoLevel;
        //        // If current calculated level is different to last saved one then new ODI level is reached, show the new ODI event
        //        if (CurrentODI_Level != geoLevelController.EventSystem.GetVariable("BC_SDI", -1))
        //        {
        //            // Get the Event ID from array dependent on calculated level index
        //            string eventID = ODI_EventIDs[CurrentODI_Level];
        //            GeoscapeEventContext geoscapeEventContext = new GeoscapeEventContext(geoAlienFaction, geoLevelController.ViewerFaction);
        //            geoLevelController.EventSystem.TriggerGeoscapeEvent(ODI_EventIDs[CurrentODI_Level], geoscapeEventContext);
        //            geoLevelController.EventSystem.SetVariable("BC_SDI", CurrentODI_Level);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error(e);
        //    }
        //}
    }
}
