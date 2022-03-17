using Base.Core;
using Base.Defs;
using Base.Entities.Effects.ApplicationConditions;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Tactical;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
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
    class PriestSkills
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            // Psychic Ward: Fix psychic damage immunity or at least resistance, otherwise at least change description
            Change_PsychicWard();
            
            // Biochemist: Paralysis, Poison and Viral damage increased 25%
            Create_BC_Biochemist();

            // Lay Waste: 1 AP, 3 WP, If your current Willpower score is higher than target's deal 30 damage for each point of WP difference
            Create_LayWaste();
        }

        private static void Change_PsychicWard()
        {
            DamageMultiplierStatusDef pW = Repo.GetAllDefs<DamageMultiplierStatusDef>().FirstOrDefault(asa => asa.name.Equals("PsychicWard_StatusDef"));
            pW.Multiplier = 0.001f;
        }

        private static void Create_BC_Biochemist()
        {
            float damageMod = 0.25f;
            string skillName = "BC_Biochemist_AbilityDef";

            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(dma => dma.name.Equals("BodypartDamageMultiplier_AbilityDef"));
            Sprite icon = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(ta => ta.name.Equals("BioChemist_AbilityDef")).ViewElementDef.LargeIcon;

            ApplyStatusAbilityDef Biochemist = Helper.CreateDefFromClone(
                source,
                "87d0f9a4-0d26-4c2a-badb-cef90ae746a5",
                skillName);
            Biochemist.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "1845e667-c394-4248-bf21-10fa661aeb6f",
                skillName);
            Biochemist.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "683abf9c-46ea-4406-a943-ae3864fd1ce4",
                skillName);
            Biochemist.StatusDef = Helper.CreateDefFromClone<AddDependentDamageKeywordsStatusDef>(
                null,
                "1bfd9c34-b5c5-4c6e-abaf-aefccaea2a3c",
                $"E_Status [{skillName}]");

            Biochemist.ViewElementDef.DisplayName1.LocalizationKey = "PR_BC_BIOCHEMIST";
            Biochemist.ViewElementDef.Description.LocalizationKey = "PR_BC_BIOCHEMIST_DESC";
            Biochemist.ViewElementDef.LargeIcon = icon;
            Biochemist.ViewElementDef.SmallIcon = icon;

            AddDependentDamageKeywordsStatusDef statusDef = (AddDependentDamageKeywordsStatusDef)Biochemist.StatusDef;
            statusDef.EffectName = "BC_Biochemist";
            statusDef.ApplicationConditions = new EffectConditionDef[0];
            statusDef.DurationTurns = -1;
            statusDef.DisablesActor = false;
            statusDef.SingleInstance = true;
            statusDef.ShowNotification = true;
            statusDef.VisibleOnPassiveBar = false;
            statusDef.VisibleOnHealthbar = TacStatusDef.HealthBarVisibility.AlwaysVisible;
            statusDef.VisibleOnStatusScreen = TacStatusDef.StatusScreenVisibility.VisibleOnStatusesList | TacStatusDef.StatusScreenVisibility.VisibleOnBodyPartStatusList;
            statusDef.HealthbarPriority = 0;
            statusDef.StackMultipleStatusesAsSingleIcon = false;
            statusDef.Visuals = Biochemist.ViewElementDef;
            statusDef.ParticleEffectPrefab = null;
            statusDef.DontRaiseOnApplyOnLoad = false;
            statusDef.EventOnApply = null;
            statusDef.EventOnUnapply = null;
            statusDef.DamageKeywordDefs = new DamageKeywordDef[]
            {
                Shared.SharedDamageKeywords.ViralKeyword,
                Shared.SharedDamageKeywords.AcidKeyword,
                Shared.SharedDamageKeywords.PoisonousKeyword,
                Shared.SharedDamageKeywords.ParalysingKeyword
            };
            statusDef.DamageMultiplierType = DamageMultiplierType.Outgoing;
            statusDef.BonusDamagePerc = damageMod;
        }

        private static void Create_LayWaste()
        {
            Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
            Logger.Debug("----------------------------------------------------", false);
        }
    }
}
