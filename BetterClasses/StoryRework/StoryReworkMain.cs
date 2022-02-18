using Base.Defs;
using PhoenixPoint.Common.Core;
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
                //SDI TEST
                //Timer and Trigger verified
                GeoTimePassedEventFilterDef timerToClone = Repo.GetAllDefs<GeoTimePassedEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS0_TimePassed [GeoTimePassedEventFilterDef]"));
                GeoTimePassedEventFilterDef newTimer = Helper.CreateDefFromClone(timerToClone, "0E7EB32D-3A54-4F12-A1B8-046E498DA040", "E_PROG_FS30_TimePassed [GeoTimePassedEventFilterDef]");
                newTimer.TimePassedRaw = "1d0h";
                newTimer.TimePassedHours = (float)24.0;
                OrEventFilterDef TimerTriggerToClone = Repo.GetAllDefs<OrEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS0_MultipleTriggers [OrEventFilterDef]"));
                OrEventFilterDef newTimerTrigger = Helper.CreateDefFromClone(TimerTriggerToClone, "C94E63CF-685B-4471-A3E3-3B0BC30ACD42", "E_PROG_FS30_MultipleTriggers [OrEventFilterDef]");
                newTimerTrigger.OR_Filters[0] = newTimer;
                //Attempts to use clone TimePassed trigger with SDI Event doesn't work, presumably because it takes SDI_Changed trigger
                //Clone FS9 Condition
                GeoLevelConditionDef sourceConditionFS9 = Repo.GetAllDefs<GeoLevelConditionDef>().FirstOrDefault(ged => ged.name.Equals("[PROG_FS0] Condition 1"));
                GeoLevelConditionDef conditionSDIStart = Helper.CreateDefFromClone(sourceConditionFS9, "3CA28779 - D9D6 - 4D1D - B23C - 8DB0CDEE417F", "[PROG_FS30] Condition 1]");
                //GeoscapeEventDef SDIStart = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS9_GeoscapeEventDef"));
                //SDIStart.Name = "GS_TimePassed";
                //Clone FS9 event
                GeoscapeEventDef CloneforSDIStart = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS0_GeoscapeEventDef"));
                GeoscapeEventDef SDIStart = Helper.CreateDefFromClone(CloneforSDIStart, "F9904A5D-B4C4-4046-AE4C-EAD224AF88CE", "PROG_FS30_GeoscapeEventDef");
                SDIStart.Name = "PROG_FS30_GeoscapeEventDef";
                SDIStart.GeoscapeEventData.EventID = "PROG_FS30";
                //SDIStart.Filters[0] = newTimerTrigger;
                //SDIStart.GeoscapeEventData.Conditions[0] = conditionSDIStart;
                SDIStart.GeoscapeEventData.Flavour = "SDI";
                SDIStart.GeoscapeEventData.Title.LocalizationKey = "SDI_01_TITLE";
                SDIStart.GeoscapeEventData.Description[0].General.LocalizationKey = "SDI_01_TEXT_GENERAL_0";
                SDIStart.GeoscapeEventData.Choices[0].Text.LocalizationKey = "SDI_01_CHOICE_0_TEXT";
                // copy Augury chant from PROG_FS0 to PROG_FS9 and remove from PROG_FS0, because Augury doesn't happen and FS0 event will be used for a Sleeping Beauty Awakens
                GeoscapeEventDef geoEventFS0 = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS0_GeoscapeEventDef"));
                var savedVoiceover = geoEventFS0.GeoscapeEventData.Description[0].Voiceover;
                GeoscapeEventDef geoEventFS9 = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_FS9_GeoscapeEventDef"));
                geoEventFS9.GeoscapeEventData.Description[0].Voiceover = savedVoiceover;
                geoEventFS0.GeoscapeEventData.Description[0].Voiceover = null;
                geoEventFS9.GeoscapeEventData.Flavour = "";
                //set event timer for meteor arrival (Mount Egg)
                GeoTimePassedEventFilterDef timePassedFS9 = Repo.GetAllDefs<GeoTimePassedEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS9_TimePassed [GeoTimePassedEventFilterDef]"));
                timePassedFS9.TimePassedRaw = "4d0h";
                timePassedFS9.TimePassedHours = (float)96.0;
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



                //SDIStart.GeoscapeEventData.Choices[0].Outcome.UnlockFeatures = null;
                //SDIStart.GeoscapeEventData.Choices[0].Outcome.Cinematic = null; 
                // set event timer for the former The Gift mission reveal, now The Hatching
                // currently this is unchanged from Vanilla, but here is the code to make the change if desired
                // GeoTimePassedEventFilterDef timePassedFS1 = RepoGeoscapeEvent.GetAllDefs<GeoTimePassedEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_FS1_TimePassed"));
                // timePassedFS1.TimePassedRaw = "8d0h";
                // timePassedFS1.TimePassedHours = 192;
                // Voland messing with Corruption
                // Get corruption going from the start of the game... eh with Meteor.
                GeoscapeEventDef geoEventCH0WIN = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_CH0_WIN_GeoscapeEventDef"));
                var corruption = geoEventCH0WIN.GeoscapeEventData.Choices[0].Outcome.VariablesChange[1];
                geoEventFS9.GeoscapeEventData.Choices[0].Outcome.VariablesChange.Add(corruption);
                geoEventCH0WIN.GeoscapeEventData.Choices[0].Outcome.VariablesChange.Remove(corruption);
                // Make Acheron research available to Alien Faction without requiring completion of Unexpected Emergency, instead make it appear with Sirens and Chirons (ALN Lair Research)
                ResearchDef ALN_SirenResearch1 = Repo.GetAllDefs<ResearchDef>().FirstOrDefault(ged => ged.name.Equals("ALN_Siren1_ResearchDef"));
                var requirementForAlienAcheronResearch = ALN_SirenResearch1.RevealRequirements.Container[0];
                ResearchDef ALN_AcheronResearch1 = Repo.GetAllDefs<ResearchDef>().FirstOrDefault(ged => ged.name.Equals("ALN_Acheron1_ResearchDef"));
                ALN_AcheronResearch1.RevealRequirements.Container[0] = requirementForAlienAcheronResearch;
                // Make CH0 Mission appear when Player completes Acheron Autopsy
                GeoResearchEventFilterDef PP_ResearchConditionCH0_Miss = Repo.GetAllDefs<GeoResearchEventFilterDef>().FirstOrDefault(ged => ged.name.Equals("E_PROG_CH0_ResearchCompleted [GeoResearchEventFilterDef]"));
                PP_ResearchConditionCH0_Miss.ResearchID = "PX_Alien_Acheron_ResearchDef";
                // Make CH1 Mission appear when Player win CH0 Mission
                GeoscapeEventDef CH1_Event = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_CH1_GeoscapeEventDef"));
                var revealSiteCH1_Miss = CH1_Event.GeoscapeEventData.Choices[0].Outcome.RevealSites[0];
                var setEventCH1_Miss = CH1_Event.GeoscapeEventData.Choices[0].Outcome.SetEvents[0];
                var trackEventCH1_Miss = CH1_Event.GeoscapeEventData.Choices[0].Outcome.TrackEncounters[0];
                GeoscapeEventDef CH0_Won = Repo.GetAllDefs<GeoscapeEventDef>().FirstOrDefault(ged => ged.name.Equals("PROG_CH0_WIN_GeoscapeEventDef"));
                CH0_Won.GeoscapeEventData.Choices[0].Outcome.RevealSites.Add(revealSiteCH1_Miss);
                CH0_Won.GeoscapeEventData.Choices[0].Outcome.SetEvents.Add(setEventCH1_Miss);
                CH0_Won.GeoscapeEventData.Choices[0].Outcome.TrackEncounters.Add(trackEventCH1_Miss);

            }
    }
}
