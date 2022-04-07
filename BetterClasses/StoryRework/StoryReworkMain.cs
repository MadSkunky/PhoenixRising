using AK.Wwise;
using Assets.Code.PhoenixPoint.Geoscape.Entities.Sites.TheMarketplace;
using Base.Defs;
using Base.Entities.Statuses;
using Base.Eventus.Filters;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Research;
using PhoenixPoint.Geoscape.Events;
using PhoenixPoint.Geoscape.Events.Conditions;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Geoscape.Events.Eventus.Filters;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.ActorsInstance;
using PhoenixPoint.Tactical.Entities.Statuses;
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

                //GeoInitialWorldSetup geoInitialWorldSetup = Repo.GetAllDefs<GeoInitialWorldSetup>().FirstOrDefault(g => g.name.Equals("GeoInitialWorldSetup"));

            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

//        // Harmony patch to change the result of CorruptionStatus.CalculateValueIncrement() to be capped by ODI
//        // When ODI is <25%, max corruption is 1/3, between 25 and 50% ODI, max corruption is 2/3, and ODI >50%, corruption can be 100%
//        // Tell Harmony what original method in what class should get patched, the following class after this directive will be used to perform own code by injection
//        [HarmonyPatch(typeof(CorruptionStatus), "CalculateValueIncrement")]
//        
//        // The class that holds the code we want to inject, the name can be anything, but the more accurate the better it is for bug hunting
//        internal static class BC_CorruptionStatus_CalculateValueIncrement_patch
//        {
//            // This directive is only to prevent a VS message that the following method is never called (it will be called, but through Harmony and not our mod code)
//            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
//
//            // Finally the method that is called before (Prefix) or after (Postfix) the original method
//            // In our case we use Postfix that is called after 'CalculateValueIncrement' was executed
//            // The parameters are special variables with their names defined by Harmony:
//            // 'ref int __result' is the return value of the original method 'CalculateValueIncrement' and with the prefix 'ref' we get write access to change it (without it would be readonly)
//            // 'CorruptionStatus __instance' is status object that holds the original method, each character will have its own instance of this status and so we have access to their individual stats
//            private static void Postfix(ref int __result, CorruptionStatus __instance)
//            {
//                // 'try ... catch' to make the code more stable, errors will most likely not result in game crashes or freezes but log an error message in the mods log file
//                try
//                {
//                    // With Harmony patches we cannot directly access base.TacticalActor, Harmony's AccessTools uses Reflection to get it through the backdoor
//                    TacticalActor base_TacticalActor = (TacticalActor)AccessTools.Property(typeof(TacStatus), "TacticalActor").GetValue(__instance, null);
//
//                    // Calculate the percentage of current ODI level, these two variables are globally set by our ODI event patches
//                    int odiPerc = CurrentODI_Level * 100 / ODI_EventIDs.Length;
//
//                    // Get max corruption dependent on max WP of the selected actor
//                    int maxCorruption = 0;
//                    if (odiPerc < 25)
//                    {
//                        maxCorruption = base_TacticalActor.CharacterStats.Willpower.IntMax / 3;
//                    }
//                    else
//                    {
//                        if (odiPerc < 50)
//                        {
//                            maxCorruption = base_TacticalActor.CharacterStats.Willpower.IntMax * 2 / 3;
//                        }
//                        else // > 50%
//                        {
//                            maxCorruption = base_TacticalActor.CharacterStats.Willpower.IntMax;
//                        }
//                    }
//
//                    // Like the original calculation, but adapted with 'maxCorruption'
//                    // Also '__result' for 'return', '__instance' for 'this' and 'base_TacticalActor' for 'base.TacticalActor'
//                    __result = Mathf.Min(__instance.CorruptionStatusDef.ValueIncrement, maxCorruption - base_TacticalActor.CharacterStats.Corruption.IntValue);
//                }
//                catch (Exception e)
//                {
//                    Logger.Error(e);
//                }
//            }
//        }
//
//        // Dictionary to transfer the characters geoscape stamina to tactical level by actor ID
//        public static Dictionary<GeoTacUnitId, int> StaminaMap = new Dictionary<GeoTacUnitId, int>();
//
//        // Harmony patch to save the characters geoscape stamina by acor ID, this mehtod is called in the deployment phase before switching to tactical mode
//        [HarmonyPatch(typeof(CharacterFatigue), "ApplyToTacticalInstance")]
//        internal static class BC_CharacterFatigue_ApplyToTacticalInstance_Patch
//        {
//            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
//            private static void Postfix(CharacterFatigue __instance, TacCharacterData data)
//            {
//                try
//                {
//                    //Logger.Always($"BC_CharacterFatigue_ApplyToTacticalInstance_Patch.POSTFIX called, GeoUnitID {data.Id} with {__instance.Stamina.IntValue} stamina added to dictionary.", false);
//                    if (StaminaMap.ContainsKey(data.Id))
//                    {
//                        StaminaMap[data.Id] = __instance.Stamina.IntValue;
//                    }
//                    else
//                    {
//                        StaminaMap.Add(data.Id, __instance.Stamina.IntValue);
//                    }
//                }
//                catch (Exception e)
//                {
//                    Logger.Error(e);
//                }
//            }
//        }
//
//        // Harmony patch to change the result of CorruptionStatus.GetStatModification() to take Stamina into account
//        // Corruption application get reduced by 100% when Stamina is between 35-40, by 75% between 30-35, by 50% between 25-30.
//        [HarmonyPatch(typeof(CorruptionStatus), "GetStatModification")]
//        internal static class BC_CorruptionStatus_GetStatModification_patch
//        {
//            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
//            // We use again Postfix that is called after 'GetStatModification' was executed
//            // 'ref StatModification __result' is the return value of the original method 'GetStatModification'
//            // 'CorruptionStatus __instance' again like above the status object that holds the original method for each character
//            private static void Postfix(ref StatModification __result, CorruptionStatus __instance)
//            {
//                try
//                {
//                    // With Harmony patches we cannot directly access base.TacticalActor, Harmony's AccessTools uses Reflection to get it through the backdoor
//                    TacticalActor base_TacticalActor = (TacticalActor)AccessTools.Property(typeof(TacStatus), "TacticalActor").GetValue(__instance, null);
//
//                    // Get characters geoscape stamina by his actor ID
//                    int stamina = 40;
//                    if (StaminaMap.ContainsKey(base_TacticalActor.GeoUnitId))
//                    {
//                        stamina = StaminaMap[base_TacticalActor.GeoUnitId];
//                    }
//
//                    // Calculate WP reduction dependent on stamina
//                    float wpReduction = 0; // stamina > 35
//                    if (stamina > 30 && stamina <= 35)
//                    {
//                        wpReduction = base_TacticalActor.CharacterStats.Corruption * 0.25f;
//                    }
//                    else
//                    {
//                        if (stamina > 25 && stamina <= 30)
//                        {
//                            wpReduction = base_TacticalActor.CharacterStats.Corruption * 0.5f;
//                        }
//                        else
//                        {
//                            if (stamina > 20 && stamina <= 25)
//                            {
//                                wpReduction = base_TacticalActor.CharacterStats.Corruption * 0.75f;
//                            }
//                            else // stamina <= 20
//                            {
//                                wpReduction = base_TacticalActor.CharacterStats.Corruption;
//                            }
//                        }
//                    }
//                    
//                    // Like the original calculation, but adapted with 'maxCorruption'
//                    __result = new StatModification(StatModificationType.Add,
//                                                    StatModificationTarget.Willpower.ToString(),
//                                                    -wpReduction,
//                                                    __instance.CorruptionStatusDef,
//                                                    -wpReduction);
//                }
//                catch (Exception e)
//                {
//                    Logger.Error(e);
//                }
//            }
//        }

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
        
//        // Current and last ODI level
//        public static int CurrentODI_Level = 0;
//        // All SDI (ODI) event IDs, levels as array, index 0 - 19
//        public static readonly string[] ODI_EventIDs = new string[]
//        {
//            "SDI_01",
//            "SDI_02",
//            "SDI_03",
//            "SDI_04",
//            "SDI_05",
//            "SDI_06",
//            "SDI_07",
//            "SDI_08",
//            "SDI_09",
//            "SDI_10",
//            "SDI_11",
//            "SDI_12",
//            "SDI_13",
//            "SDI_14",
//            "SDI_15",
//            "SDI_16",
//            "SDI_17",
//            "SDI_18",
//            "SDI_19",
//            "SDI_20"
//        };
//        // Harmony patch to gather some game stats from the alien faction (pandorans) when geo level starts (campaign start, game loaded, after tactical missions)
//        [HarmonyPatch(typeof(GeoAlienFaction), "OnAfterFactionsLevelStart")]
//        internal static class BC_GeoAlienFaction_OnAfterFactionsLevelStart_patch
//        {
//            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
//            private static void Postfix(GeoAlienFaction __instance, int ____evolutionProgress)
//            {
//                Calculate_ODI_Level(__instance, ____evolutionProgress);
//            }
//        }
//        // Harmony patch to gather some game stats from the alien faction (pandorans) each day in game
//        [HarmonyPatch(typeof(GeoAlienFaction), "UpdateFactionDaily")]
//        internal static class BC_GeoAlienFaction_UpdateFactionDaily_patch
//        {
//            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
//            private static void Postfix(GeoAlienFaction __instance, int ____evolutionProgress)
//            {
//                Calculate_ODI_Level(__instance, ____evolutionProgress);
//            }
//        }
//        internal static void Calculate_ODI_Level(GeoAlienFaction geoAlienFaction, int evolutionProgress)
//        {
//            try
//            {
//                // Index of last element of the ODI event ID array is Length - 1
//                int ODI_EventIDs_LastIndex = ODI_EventIDs.Length - 1;
//                // Set a maximum number to determine the upper limit from when the maximum ODI level is reached
//                int maxODI_Progress = 470 * ODI_EventIDs_LastIndex;
//                // Calculate the current ODI level = index for the ODI event ID array
//                // Mathf.Min = cap the lavel at max index, after that the index will not longer get increased wiht higher progress
//                CurrentODI_Level = Mathf.Min(ODI_EventIDs_LastIndex, evolutionProgress * ODI_EventIDs_LastIndex / maxODI_Progress);
//                // Get the GeoLevelController to get access to the event system and the variable
//                GeoLevelController geoLevelController = geoAlienFaction.GeoLevel;
//                // If current calculated level is different to last saved one then new ODI level is reached, show the new ODI event
//                if (CurrentODI_Level != geoLevelController.EventSystem.GetVariable("BC_SDI", -1))
//                {
//                    // Get the Event ID from array dependent on calculated level index
//                    string eventID = ODI_EventIDs[CurrentODI_Level];
//                    GeoscapeEventContext geoscapeEventContext = new GeoscapeEventContext(geoAlienFaction, geoLevelController.ViewerFaction);
//                    geoLevelController.EventSystem.TriggerGeoscapeEvent(ODI_EventIDs[CurrentODI_Level], geoscapeEventContext);
//                    geoLevelController.EventSystem.SetVariable("BC_SDI", CurrentODI_Level);
//                }
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//            }
//        }
    }
}
