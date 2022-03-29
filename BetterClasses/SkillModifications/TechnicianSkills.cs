﻿using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects.ApplicationConditions;
using Base.Entities.Statuses;
using Base.UI;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.Events;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
using PhoenixPoint.Tactical.Entities.Equipments;
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
            string skillName = "AmplifyPain_AbilityDef";
            float healMultiplier = 2f;
            float additionalDamageMultiplier = 0.5f;
            float wpCost = 4f;
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("QuickAim_AbilityDef"));
            Sprite icon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_CharacterAbility_AmplifyPain01.png");

            ApplyStatusAbilityDef AmplifyPain = Helper.CreateDefFromClone(
                source,
                "463a6458-e45c-4310-abb6-c7cb904cb918",
                skillName);
            AmplifyPain.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "5938a019-b798-4076-98a8-a58d100411f3",
                skillName);
            AmplifyPain.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "a4812b9a-385d-4083-a494-3e169d08c39e",
                skillName);
            AmplifyPain.StatusDef = Helper.CreateDefFromClone(
                source.StatusDef,
                "d3ce1f20-3503-4dfb-a836-6bdc78064d4e",
                skillName);
            StanceStatusDef HealMod = Helper.CreateDefFromClone(
                Repo.GetAllDefs<StanceStatusDef>().FirstOrDefault(sd => sd.name.Equals("E_Status [ImprovedMedkit_FactionEffectDef]")),
                "66cf66aa-c0f5-4711-b8de-52a5a478b389",
                $"E_HealMultiplier [{skillName}]");
            AddDependentDamageKeywordsStatusDef DamageMod = Helper.CreateDefFromClone<AddDependentDamageKeywordsStatusDef>(
                null,
                "247f3abb-420f-42ac-8af3-d3ca12fb5787",
                $"E_DamageMultiplier [{skillName}]");
            
            AmplifyPain.ViewElementDef.DisplayName1.LocalizationKey = "PR_BC_AMPLIFY_PAIN";
            AmplifyPain.ViewElementDef.Description.LocalizationKey = "PR_BC_AMPLIFY_PAIN_DESC";
            AmplifyPain.ViewElementDef.LargeIcon = icon;
            AmplifyPain.ViewElementDef.SmallIcon = icon;
            AmplifyPain.UsesPerTurn = 1;
            AmplifyPain.WillPointCost = wpCost;
            AmplifyPain.DisablingStatuses = new StatusDef[] { AmplifyPain.StatusDef };

            (AmplifyPain.StatusDef as AddAttackBoostStatusDef).DurationTurns = 0;
            (AmplifyPain.StatusDef as AddAttackBoostStatusDef).ExpireOnEndOfTurn = true;
            (AmplifyPain.StatusDef as AddAttackBoostStatusDef).Visuals = AmplifyPain.ViewElementDef;
            (AmplifyPain.StatusDef as AddAttackBoostStatusDef).NumberOfAttacks = -1; // lasts as long as the status = end of turn
            (AmplifyPain.StatusDef as AddAttackBoostStatusDef).AdditionalStatusesToApply = new TacStatusDef[] { HealMod, DamageMod };

            HealMod.DurationTurns = 0;
            HealMod.EquipmentsStatModifications = new EquipmentItemTagStatModification[]
            {
                new EquipmentItemTagStatModification()
                {
                    ItemTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt1 => gt1.name.Equals("MedkitItem_TagDef")),
                    EquipmentStatModification = new ItemStatModification()
                    {
                        TargetStat = StatModificationTarget.BonusHealValue,
                        Modification = StatModificationType.Multiply,
                        Value = healMultiplier
                    }
                },
                new EquipmentItemTagStatModification()
                {
                    ItemTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt2 => gt2.name.Equals("RoboticArmItem_TagDef")),
                    EquipmentStatModification = new ItemStatModification()
                    {
                        TargetStat = StatModificationTarget.BonusHealValue,
                        Modification = StatModificationType.Multiply,
                        Value = healMultiplier
                    }
                }
            };

            DamageMod.EffectName = "AmplifyPain";
            DamageMod.ApplicationConditions = new EffectConditionDef[0];
            DamageMod.DurationTurns = 0;
            DamageMod.DisablesActor = false;
            DamageMod.SingleInstance = true;
            DamageMod.ShowNotification = true;
            DamageMod.VisibleOnPassiveBar = false;
            DamageMod.VisibleOnHealthbar = 0;
            DamageMod.VisibleOnStatusScreen = 0;
            DamageMod.HealthbarPriority = 0;
            DamageMod.StackMultipleStatusesAsSingleIcon = false;
            DamageMod.Visuals = AmplifyPain.ViewElementDef;
            DamageMod.ParticleEffectPrefab = null;
            DamageMod.DontRaiseOnApplyOnLoad = false;
            DamageMod.EventOnApply = null;
            DamageMod.EventOnUnapply = null;
            DamageMod.DamageKeywordDefs = new DamageKeywordDef[]
            {
                SkillModsMain.sharedSoloDamageKeywords.SoloViralKeyword,
                SkillModsMain.sharedSoloDamageKeywords.SoloAcidKeyword,
                SkillModsMain.sharedSoloDamageKeywords.SoloPoisonousKeyword,
                SkillModsMain.sharedSoloDamageKeywords.SoloParalysingKeyword,
                //SkillModsMain.sharedSoloDamageKeywords.SoloShockKeyword,
                //SkillModsMain.sharedSoloDamageKeywords.SoloSonicKeyword
            };
            DamageMod.BonusDamagePerc = additionalDamageMultiplier;

            foreach (TacActorSimpleAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(aad => aad.name.Contains("Soldier_Utka_AnimActionsDef")))
            {
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(source) && !animActionDef.AbilityDefs.Contains(AmplifyPain))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(AmplifyPain).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                    Logger.Debug("----------------------------------------------------", false);
                }
            }
        }
    }
}
