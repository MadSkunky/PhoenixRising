using Base.Core;
using Base.Defs;
using Base.Entities.Effects.ApplicationConditions;
using Base.UI;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.Events;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixRising.BetterClasses.Tactical.Entities.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class TechnicianSkills
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            // Electric Reinforcements: 10 tiles range, +10 armor, 1 AP and 3 WP
            Change_ElectricReinforcements();

            // Stability: Gain 5% extra accuracy per remaining AP up to 20%
            Create_Stability();

            // Amplify Pain: If your next attack deals special damage, double that damage (Bleeding, Paralysis, Viral, Poison, Fire, EMP, Sonic, Shock, Virophage)
            Create_AmplifyPain();
        }

        private static void Change_ElectricReinforcements()
        {
            float armorBonus = 10f;
            ApplyStatusAbilityDef eR = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("ElectricReinforcement_AbilityDef"));
            ItemSlotStatsModifyStatusDef eRStatus = (ItemSlotStatsModifyStatusDef)eR.StatusDef;

            eR.TargetingDataDef.Origin.Range = 10;
            eR.ActionPointCost = 0.25f;
            eR.WillPointCost = 3;
            eR.ViewElementDef.Description = new LocalizedTextBind($"Give yourself and allies within 20 tiles a bonus of {armorBonus} armour for 1 turn. This effect does not stack.", doNotLocalize);
            eRStatus.StatsModifications[0].Value = armorBonus;
            eRStatus.StatsModifications[1].Value = armorBonus;
        }

        private static void Create_Stability()
        {
            float maxAccBoost = 0.2f;
            string skillName = "Stability_AbilityDef";
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("BloodLust_AbilityDef"));
            Sprite icon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_Strategist.png");

            ApplyStatusAbilityDef Stability = Helper.CreateDefFromClone(
                source,
                "697a87ab-a799-4c7a-9332-e0b411a2e82d",
                skillName);
            Stability.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "4f447b56-c8e2-4e25-8a3c-16f599b5cc0c",
                skillName);
            Stability.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "868adb42-e012-4c7a-9d39-fe9d64d95be9",
                skillName);
            Stability.StatusDef = Helper.CreateDefFromClone<ActionpointsRelatedStatusDef>(
                null,
                "997a4627-2982-44d4-944d-1c8cf76acb02",
                $"E_Status [{skillName}]");
            Stability.ViewElementDef.DisplayName1.LocalizationKey = "PR_BC_STABILITY";
            Stability.ViewElementDef.Description.LocalizationKey = "PR_BC_STABILITY_DESC";
            Stability.ViewElementDef.LargeIcon = icon;
            Stability.ViewElementDef.SmallIcon = icon;
            ActionpointsRelatedStatusDef apRelatedStatus = (ActionpointsRelatedStatusDef)Stability.StatusDef;
            apRelatedStatus.EffectName = "StabilityStatus";
            apRelatedStatus.ApplicationConditions = new EffectConditionDef[0];
            apRelatedStatus.DisablesActor = false;
            apRelatedStatus.SingleInstance = false;
            apRelatedStatus.ShowNotification = false;
            apRelatedStatus.VisibleOnPassiveBar = false;
            apRelatedStatus.VisibleOnHealthbar = 0;
            apRelatedStatus.VisibleOnStatusScreen = 0;
            apRelatedStatus.HealthbarPriority = 0;
            apRelatedStatus.StackMultipleStatusesAsSingleIcon = false;
            apRelatedStatus.Visuals = Stability.ViewElementDef;
            apRelatedStatus.ParticleEffectPrefab = null;
            apRelatedStatus.DontRaiseOnApplyOnLoad = false;
            apRelatedStatus.EventOnApply = null;
            apRelatedStatus.EventOnUnapply = null;
            apRelatedStatus.ActionpointsLowBound = 1.0f;
            apRelatedStatus.MaxBoost = maxAccBoost;
            apRelatedStatus.StatModificationTargets = new StatModificationTarget[]
            {
                StatModificationTarget.Accuracy
            };
        }

        private static void Create_AmplifyPain()
        {
            Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
            Logger.Debug("----------------------------------------------------", false);
        }
    }
}
