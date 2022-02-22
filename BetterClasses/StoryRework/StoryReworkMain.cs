using Base.Defs;
using Base.Eventus.Filters;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Entities.Research;
using PhoenixPoint.Geoscape.Events.Conditions;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Geoscape.Events.Eventus.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.BetterClasses.StoryRework
{
    class StoryReworkMain
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

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
                var savedVoiceover = geoEventFS0.GeoscapeEventData.Description[0].Voiceover;
                GeoscapeEventDef geoEventFS9 = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS9_GeoscapeEventDef"));
                geoEventFS9.GeoscapeEventData.Description[0].Voiceover = savedVoiceover;
                geoEventFS0.GeoscapeEventData.Description[0].Voiceover = null;
                geoEventFS9.GeoscapeEventData.Flavour = "";
                //set event timer for meteor arrival (Mount Egg)
                GeoTimePassedEventFilterDef timePassedFS9 = Repo.GetAllDefs<GeoTimePassedEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS9_TimePassed [GeoTimePassedEventFilterDef]"));
                timePassedFS9.TimePassedRaw = "2d0h";
                timePassedFS9.TimePassedHours = (float)48.0;
                // set event timer for former Augury, now A Sleeping Beauty Awakens
                GeoTimePassedEventFilterDef timePassedFS0 = Repo.GetAllDefs<GeoTimePassedEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS0_TimePassed [GeoTimePassedEventFilterDef]"));
                timePassedFS0.TimePassedRaw = "6d0h";
                timePassedFS0.TimePassedHours = (float)144.0;
                // set background and leader images for A Sleeping Beauty Awakens
                geoEventFS0.GeoscapeEventData.Flavour = "ChineseSinkHole";
                geoEventFS0.GeoscapeEventData.Leader = "SY_Eileen";
                // change leader image from Athena to Eileen for We Are Still Collating (former the Invitation)
                GeoscapeEventDef geoEventFS1 = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS1_GeoscapeEventDef"));
                geoEventFS1.GeoscapeEventData.Leader = "SY_Eileen";
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
    }
}
