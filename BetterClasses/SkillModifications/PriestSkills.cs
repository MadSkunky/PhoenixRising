using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects.ApplicationConditions;
using Base.Utils.Maths;
using Harmony;
using I2.Loc;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Effects;
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
            // Biochemist: Paralysis, Poison and Viral damage increased 25%
            Create_BC_Biochemist();

            // Lay Waste: 1 AP, 3 WP, If your current Willpower score is higher than target's deal 30 damage for each point of WP difference
            Create_LayWaste();
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
                SkillModsMain.sharedSoloDamageKeywords.SoloViralKeyword,
                SkillModsMain.sharedSoloDamageKeywords.SoloAcidKeyword,
                SkillModsMain.sharedSoloDamageKeywords.SoloPoisonousKeyword,
                SkillModsMain.sharedSoloDamageKeywords.SoloParalysingKeyword
            };
            statusDef.BonusDamagePerc = damageMod;
        }

        private static void Create_LayWaste()
        {
            float apCost = 0.25f;
            float wpCost = 3.0f;
            
            string skillName = "LayWaste_AbilityDef";
            Sprite icon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(tav => tav.name.Equals("E_ViewElement [Mutoid_PoisonExplosion_ApplyStatusAbilityDef]")).LargeIcon;

            ApplyEffectAbilityDef source = Repo.GetAllDefs<ApplyEffectAbilityDef>().FirstOrDefault(aea => aea.name.Equals("MindCrush_AbilityDef"));

            ApplyEffectAbilityDef LayWaste = Helper.CreateDefFromClone(
                source,
                "887e8c79-21d1-460f-b528-b6377ab6baf5",
                skillName);
            LayWaste.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "dc252b8d-2fca-46b8-bd31-68e125e7d0ef",
                skillName);
            LayWaste.TargetingDataDef = Helper.CreateDefFromClone(
                source.TargetingDataDef,
                "787d2f98-69d4-4e87-90a6-ad8d68395ce0",
                skillName);
            LayWaste.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "d8fc6992-fd47-474a-a071-150162b8aebb",
                skillName);
            LayWaste.EffectDef = Helper.CreateDefFromClone(
                source.EffectDef,
                "1160e6e7-8e03-4823-a986-58f0130f21a6",
                skillName);
            //(LayWaste.StatusDef as TacEffectStatusDef).EffectDef = Helper.CreateDefFromClone(
            //    Repo.GetAllDefs<DamageEffectDef>().FirstOrDefault(de => de.name.Equals("E_Effect [MindCrush_AbilityDef]")),
            //    "ea33c61e-2eb2-4b56-94c4-55aa4265ed05",
            //    skillName);

            LayWaste.TargetingDataDef.Origin.LineOfSight = LineOfSightType.InSight;
            LayWaste.TargetingDataDef.Origin.FactionVisibility = LineOfSightType.InSight;
            LayWaste.TargetingDataDef.Origin.Range = float.PositiveInfinity;
            LayWaste.TargetingDataDef.Target.TargetEnemies = true;
            LayWaste.TargetingDataDef.Target.TargetResult = TargetResult.Actor;

            LayWaste.ViewElementDef.DisplayName1.LocalizationKey = "PR_BC_LAY_WASTE";
            LayWaste.ViewElementDef.Description.LocalizationKey = "PR_BC_LAY_WASTE_DESC";
            LayWaste.ViewElementDef.LargeIcon = icon;
            LayWaste.ViewElementDef.SmallIcon = icon;

            LayWaste.ActionPointCost = apCost;
            LayWaste.WillPointCost = wpCost;
            LayWaste.ApplyToAllTargets = false;
            LayWaste.MultipleTargetSimulation = false;

            //TacEffectStatusDef effectStatus = LayWaste.StatusDef as TacEffectStatusDef;
            //effectStatus.DurationTurns = 0;
            //effectStatus.SingleInstance = true;
            //effectStatus.VisibleOnPassiveBar = false;
            //effectStatus.VisibleOnHealthbar = 0;
            //effectStatus.VisibleOnStatusScreen = 0;
            //effectStatus.StackMultipleStatusesAsSingleIcon = false;
            //effectStatus.Visuals = null;
            //effectStatus.ParticleEffectPrefab = null;
            //effectStatus.ApplyOnStatusApplication = true;
            //effectStatus.ApplyOnTurnStart = false;

            DamageEffectDef damageEffect = LayWaste.EffectDef as DamageEffectDef;
            damageEffect.MinimumDamage = 60;
            damageEffect.MaximumDamage = 60;

            TacticalAbilityDef animSource = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(ta => ta.name.Equals("InducePanic_AbilityDef"));
            foreach (TacActorSimpleAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(aad => aad.name.Contains("Soldier_Utka_AnimActionsDef")))
            {
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(animSource) && !animActionDef.AbilityDefs.Contains(LayWaste))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(LayWaste).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                    Logger.Debug("----------------------------------------------------", false);
                }
            }

            //Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
            //Logger.Debug("----------------------------------------------------", false);

            //foreach (TacEffectStatusDef temp in Repo.GetAllDefs<TacEffectStatusDef>())
            //{
            //    Logger.Always(temp.name);
            //}
        }
    }
}
