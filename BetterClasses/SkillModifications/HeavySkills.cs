using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects;
using Base.Entities.Effects.ApplicationConditions;
using Base.Entities.Statuses;
using Base.UI;
using Base.Utils.Maths;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class HeavySkills
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            // Return Fire: Fix to work on all classes
            Change_ReturnFire();

            // Hunker Down: -25% incoming damage for 2 AP and 2 WP
            Create_HunkerDown();

            // Skimisher: If you take damage during enemy turn your attacks deal 25% more damage until end of turn
            Create_Skirmisher();

            // Shred Resistance: 50% shred resistance
            Create_ShredResistance();

            // Rage Burst: Increase accuracy and cone angle
            Change_RageBurst();

            // Jetpack Control: 2 AP jump, 12 tiles range
            Create_JetpackControl();

            // Boom Blast: -30% range instead of +50%
            Change_BoomBlast();

            // War Cry: -1 AP and -10% damage, doubled if WP of target < WP of caster (see Harmony patch below)
            //Change_WarCry();
        }

        private static void Change_ReturnFire()
        {
            TacticalAbilityDef returnFire = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains("ReturnFire_AbilityDef"));
            returnFire.ActorTags = new GameTagDef[0]; // Deletes all given tags => no restriction for any class
        }
        private static void Create_HunkerDown()
        {
            string skillName = "HunkerDown_AbilityDef";
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("CloseQuarters_AbilityDef"));
            ApplyStatusAbilityDef hunkerDown = Helper.CreateDefFromClone(
                source,
                "a3d841c5-b3dd-440b-ae4e-629dcabd14df",
                skillName);
            hunkerDown.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "64add472-da6f-4584-b5e9-f204b7d3c735",
                skillName);
            hunkerDown.TargetingDataDef = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("QuickAim_AbilityDef")).TargetingDataDef;
            hunkerDown.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "c0b8b645-b1b7-4f4e-87ea-3f6bacc2dc4f",
                skillName);
            hunkerDown.StatusDef = Helper.CreateDefFromClone(
                source.StatusDef,
                "adc38c08-1878-422f-a37c-a859aa67ceed",
                skillName);
            hunkerDown.Active = true;
            hunkerDown.EndsTurn = true;
            hunkerDown.ActionPointCost = 0.25f;
            hunkerDown.WillPointCost = 2.0f;
            hunkerDown.TraitsRequired = new string[] { "start", "ability", "move" };
            hunkerDown.TraitsToApply = new string[] { "ability" };
            hunkerDown.ShowNotificationOnUse = true;
            hunkerDown.StatusApplicationTrigger = StatusApplicationTrigger.ActivateAbility;
            hunkerDown.CharacterProgressionData.RequiredStrength = 0;
            hunkerDown.CharacterProgressionData.RequiredWill = 0;
            hunkerDown.CharacterProgressionData.RequiredSpeed = 0;
            hunkerDown.ViewElementDef.DisplayName1 = new LocalizedTextBind("HUNKER DOWN", doNotLocalize);
            hunkerDown.ViewElementDef.Description = new LocalizedTextBind("Gain 25% damage resistance until your next turn.", doNotLocalize);
            Sprite hunkerDownIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(ve => ve.name.Equals("E_ViewElement [Chiron_EnterStabilityStance_AbilityDef]")).SmallIcon;
            hunkerDown.ViewElementDef.LargeIcon = hunkerDownIcon;
            hunkerDown.ViewElementDef.SmallIcon = hunkerDownIcon;
            (hunkerDown.StatusDef as DamageMultiplierStatusDef).DurationTurns = 1;
            (hunkerDown.StatusDef as DamageMultiplierStatusDef).Visuals = hunkerDown.ViewElementDef;
            (hunkerDown.StatusDef as DamageMultiplierStatusDef).DamageTypeDefs = new DamageTypeBaseEffectDef[0]; // Empty = all damage types
            (hunkerDown.StatusDef as DamageMultiplierStatusDef).Range = -1.0f; // -1 = no range restriction
            AbilityDef animationSearchDef = Repo.GetAllDefs<AbilityDef>().FirstOrDefault(ad => ad.name.Equals("QuickAim_AbilityDef"));
            foreach (TacActorSimpleAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(aad => aad.name.Contains("Soldier_Utka_AnimActionsDef")))
            {
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(animationSearchDef) && !animActionDef.AbilityDefs.Contains(hunkerDown))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(hunkerDown).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                    Logger.Debug("----------------------------------------------------", false);
                }
            }
        }
        private static void Create_Skirmisher()
        {
            float damageMod = 1.25f;
            string skillName = "Skirmisher_AbilityDef";

            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("Acheron_DynamicResistance_AbilityDef"));
            ApplyStatusAbilityDef skirmisher = Helper.CreateDefFromClone(
                source,
                "d6d9041b-9763-4673-a057-2bbefd96aa67",
                skillName);
            skirmisher.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "657f3e2b-08c0-4234-b16f-3f6d57d049e1",
                skillName);
            skirmisher.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "5ef3fb17-03d0-4e33-b76a-d74cbeefc509",
                skillName);
            skirmisher.StatusDef = Helper.CreateDefFromClone(
                source.StatusDef,
                "2bafd8da-f84a-4fd7-ae41-8ba0f9e7aba6",
                skillName);
            skirmisher.ViewElementDef.DisplayName1 = new LocalizedTextBind("SKIRMISHER", doNotLocalize);
            skirmisher.ViewElementDef.Description = new LocalizedTextBind($"If you take damage during enemy turn your attacks deal {(damageMod * 100) - 100}% more damage until end of turn.", doNotLocalize);
            Sprite skirmisherIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_Gifted.png");
            skirmisher.ViewElementDef.LargeIcon = skirmisherIcon;
            skirmisher.ViewElementDef.SmallIcon = skirmisherIcon;

            StanceStatusDef skirmisherDamageModification = Helper.CreateDefFromClone( // Borrow status from Sneak Attack for damage modification
                Repo.GetAllDefs<StanceStatusDef>().FirstOrDefault(p => p.name.Equals("E_SneakAttackStatus [SneakAttack_AbilityDef]")),
                "728f321f-3a9d-4e63-a160-660c2a2c4664",
                $"E_DamageModificationStatus [{skillName}]");
            skirmisherDamageModification.DurationTurns = 1;
            skirmisherDamageModification.SingleInstance = true;
            skirmisherDamageModification.Visuals = skirmisher.ViewElementDef;
            skirmisherDamageModification.EquipmentsStatModifications = new EquipmentItemTagStatModification[0];
            skirmisherDamageModification.StatModifications = new ItemStatModification[]
            {
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.BonusAttackDamage,
                    Modification = StatModificationType.Multiply,
                    Value = damageMod
                }
            };
            DynamicResistanceStatusDef skirmisherReactionStatus = (DynamicResistanceStatusDef)skirmisher.StatusDef;
            skirmisherReactionStatus.ResistanceStatuses = new DynamicResistanceStatusDef.ResistancePerDamageType[]
            {
                new DynamicResistanceStatusDef.ResistancePerDamageType()
                {
                    DamageTypeBaseEffectDef = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtb => dtb.name.Equals("Projectile_StandardDamageTypeEffectDef")),
                    ResistanceStatusDef = skirmisherDamageModification
                },
                new DynamicResistanceStatusDef.ResistancePerDamageType()
                {
                    DamageTypeBaseEffectDef = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtb => dtb.name.Equals("Bash_StandardDamageTypeEffectDef")),
                    ResistanceStatusDef = skirmisherDamageModification
                },
                new DynamicResistanceStatusDef.ResistancePerDamageType()
                {
                    DamageTypeBaseEffectDef = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtb => dtb.name.Equals("MeleeBash_StandardDamageTypeEffectDef")),
                    ResistanceStatusDef = skirmisherDamageModification
                },
                new DynamicResistanceStatusDef.ResistancePerDamageType()
                {
                    DamageTypeBaseEffectDef = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtb => dtb.name.Equals("Blast_StandardDamageTypeEffectDef")),
                    ResistanceStatusDef = skirmisherDamageModification
                }
            };
        }
        // Harmony patch for Skirmisher in the moment an ability is activated
        [HarmonyPatch(typeof(DynamicResistanceStatus), "OnAbilityActivating")]
        internal static class Skirmisher_DynamicResistanceStatus_OnAbilityActivating_patch
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static bool Prefix(DynamicResistanceStatus __instance)
            {
                TacticalActorBase base_TacticalActorBase = (TacticalActorBase)AccessTools.Property(typeof(TacStatus), "TacticalActorBase").GetValue(__instance, null);
                // Don't execute original method (return false) when current instance is called from Skirmisher ability
                // AND current turn is same turn of ability owner (e.g. self damaging in player turn, no cheese patch)
                return !(__instance.TacStatusDef.name.Contains("Skirmisher_AbilityDef")
                    && base_TacticalActorBase.TacticalLevel.CurrentFaction == base_TacticalActorBase.TacticalFaction);
            }
        }
        // Harmony patch for Skirmisher in the moment damage is appilied
        [HarmonyPatch(typeof(DynamicResistanceStatus), "OnDamageApplied")]
        internal static class Skirmisher_DynamicResistanceStatus_OnDamageApplied_patch
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static bool Prefix(DynamicResistanceStatus __instance)
            {
                TacticalActorBase base_TacticalActorBase = (TacticalActorBase)AccessTools.Property(typeof(TacStatus), "TacticalActorBase").GetValue(__instance, null);
                // Don't execute original method (return false) when current instance is called from Skirmisher ability
                // AND current turn is same turn of ability owner (e.g. self damaging in player turn, no cheese patch)
                return !(__instance.TacStatusDef.name.Contains("Skirmisher_AbilityDef")
                    && base_TacticalActorBase.TacticalLevel.CurrentFaction == base_TacticalActorBase.TacticalFaction);
            }
        }

        private static void Create_ShredResistance()
        {
            string skillName = "ShredResistant_DamageMultiplierAbilityDef";
            DamageMultiplierAbilityDef source = Repo.GetAllDefs<DamageMultiplierAbilityDef>().FirstOrDefault(dma => dma.name.Equals("PoisonResistant_DamageMultiplierAbilityDef"));
            DamageMultiplierAbilityDef shredRes = Helper.CreateDefFromClone(
                source,
                "da32f3c3-74d4-440c-9197-8fcccaf66da8",
                skillName);
            shredRes.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "18a2c7e2-9266-4f8f-acf9-8242c5b529c3",
                skillName);
            shredRes.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "487acaef-7908-436b-b458-b0a670382663",
                skillName);
            shredRes.DamageTypeDef = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtb => dtb.name.Equals("Shred_StandardDamageTypeEffectDef"));
            shredRes.ViewElementDef.DisplayName1 = new LocalizedTextBind("SHRED RESISTANCE", doNotLocalize);
            shredRes.ViewElementDef.Description = new LocalizedTextBind("Shred Resistance", doNotLocalize);
            TacticalAbilityViewElementDef pr_ViewElement = (TacticalAbilityViewElementDef)Repo.GetDef("00431749-6f3f-d7e3-41a1-56e07706bd5a");
            if (pr_ViewElement != null)
            {
                shredRes.ViewElementDef.LargeIcon = pr_ViewElement.LargeIcon;
                shredRes.ViewElementDef.SmallIcon = pr_ViewElement.LargeIcon;
            }
        }
        private static void Change_RageBurst()
        {
            RageBurstInConeAbilityDef rageBurst = Repo.GetAllDefs<RageBurstInConeAbilityDef>().FirstOrDefault(p => p.name.Equals("RageBurst_RageBurstInConeAbilityDef"));
            rageBurst.ProjectileSpreadMultiplier = 0.4f; // acc buff calculation: 1 / value - 100 = +acc%, 1 / 0.4 - 100 = +150%
            rageBurst.ConeSpread = 15.0f;
            rageBurst.ViewElementDef.Description = new LocalizedTextBind("Shoot 5 times across a wide arc with increased accuracy", doNotLocalize);
        }
        private static void Create_JetpackControl()
        {
            string skillName = "JetpackControl_AbilityDef";
            float jetpackControlAPCost = 0.5f;
            float jetpackControlWPCost = 3f;
            float jetpackControlRange = 12f;
            JetJumpAbilityDef source = Repo.GetAllDefs<JetJumpAbilityDef>().FirstOrDefault(jj => jj.name.Equals("JetJump_AbilityDef"));
            JetJumpAbilityDef jetpackControl = Helper.CreateDefFromClone(
                source,
                "ddbb58e8-9ea4-417c-bddb-8ed62837bb10",
                skillName);
            jetpackControl.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "f330ce45-361a-4444-bd69-04b3e6350a0e",
                skillName);
            jetpackControl.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "629a8d02-1dfe-48bb-9ae5-4ef8d789b5eb",
                skillName);
            jetpackControl.TargetingDataDef = Helper.CreateDefFromClone(
                source.TargetingDataDef,
                "c97fda50-4e29-443d-a043-cf852fa0ec12",
                skillName);
            jetpackControl.CharacterProgressionData.RequiredStrength = 0;
            jetpackControl.CharacterProgressionData.RequiredWill = 0;
            jetpackControl.CharacterProgressionData.RequiredSpeed = 0;
            jetpackControl.ViewElementDef.DisplayName1 = new LocalizedTextBind("JETPACK CONTROL", doNotLocalize);
            string description = $"Jet jump to a location within {jetpackControlRange} tiles";
            jetpackControl.ViewElementDef.Description = new LocalizedTextBind(description, doNotLocalize);
            Sprite jetpackControlIcon = Repo.GetAllDefs<ClassProficiencyAbilityDef>().FirstOrDefault(cp => cp.name.Equals("UseAttachedEquipment_AbilityDef")).ViewElementDef.LargeIcon;
            jetpackControl.ViewElementDef.LargeIcon = jetpackControlIcon;
            jetpackControl.ViewElementDef.SmallIcon = jetpackControlIcon;
            jetpackControl.ActionPointCost = jetpackControlAPCost;
            jetpackControl.WillPointCost = jetpackControlWPCost;
            jetpackControl.AbilitiesRequired = new TacticalAbilityDef[] { source };
            jetpackControl.TargetingDataDef.Origin.Range = jetpackControlRange;
            foreach (TacActorSimpleAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(aad => aad.name.Contains("Soldier_Utka_AnimActionsDef")))
            {
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(source) && !animActionDef.AbilityDefs.Contains(jetpackControl))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(jetpackControl).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                    Logger.Debug("----------------------------------------------------", false);
                }
            }
        }
        private static void Change_BoomBlast()
        {
            bool setNewStats = false;
            ApplyStatusAbilityDef boomBlast = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("BigBooms_AbilityDef"));
            boomBlast.ViewElementDef.Description = new LocalizedTextBind(
                $"Until end of turn your explosives gain +50% range. Grenade Launcher AP cost reduce by 1.",
                doNotLocalize);

            // Convert additional statuses to a List for easier access
            List<TacStatusDef> bbAdditionalStatusesToApply = (boomBlast.StatusDef as AddAttackBoostStatusDef).AdditionalStatusesToApply.ToList();

            // Fix AP cost to only affect grenade launcher, incl set the right tag for them (not used and set in vanilla)
            GameTagDef glTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("GrenadeLauncherItem_TagDef"));
            foreach (WeaponDef wd in Repo.GetAllDefs<WeaponDef>())
            {
                if ((wd.name.Equals("PX_GrenadeLauncher_WeaponDef") || wd.name.Equals("AC_Rebuke_WeaponDef"))
                    && !wd.Tags.Contains(glTag))
                {
                    wd.Tags.Add(glTag);
                }
            }
            ChangeAbilitiesCostStatusDef reduceApCostStatus = (ChangeAbilitiesCostStatusDef)bbAdditionalStatusesToApply.FirstOrDefault(a => a.name.Equals("E_ReduceExplosiveAbilitiesCost [BigBooms_AbilityDef]"));
            if (reduceApCostStatus != null && reduceApCostStatus.AbilityCostModification.EquipmentTagDef != glTag)
            {
                reduceApCostStatus.AbilityCostModification.EquipmentTagDef = glTag;
                //bbAdditionalStatusesToApply.Remove(reduceApCostStatus);
            }
            
            // Set new detailed stats if configured
            if (setNewStats)
            {
                float bbDamageMod = 0.33f;
                float bbRangeMod = 1.4f;
                float bbAccuracyMod = 0.5f;
                float bbProjectileMod = 7.0f;
                boomBlast.ViewElementDef.Description = new LocalizedTextBind(
                    $"Until end of turn your explosives get {(bbDamageMod * 100) - 100}% damage, {(bbRangeMod * 100) - 100}% range, {(bbAccuracyMod * 100) - 100}% accuracy. Launcher with multiple explosives per magazine gain +{bbProjectileMod} projectiles per shot.",
                    doNotLocalize);

                StatMultiplierStatusDef bbAccModStatus = Helper.CreateDefFromClone(
                   Repo.GetAllDefs<StatMultiplierStatusDef>().FirstOrDefault(sms => sms.name.Equals("Trembling_StatusDef")),
                   "4a6f7cc4-1bd6-45a5-b572-053963966b07",
                   $"E AccuracyMultiplier [Boom Blast]");
                bbAccModStatus.EffectName = "";
                bbAccModStatus.ShowNotification = false;
                bbAccModStatus.VisibleOnHealthbar = 0;
                bbAccModStatus.VisibleOnStatusScreen = 0;
                bbAccModStatus.Visuals = null;
                bbAccModStatus.StatsMultipliers[0].StatName = "Accuracy";
                bbAccModStatus.StatsMultipliers[0].Multiplier = bbAccuracyMod;
                EquipmentItemTagStatModification[] bbMods = new EquipmentItemTagStatModification[]
                {
                    new EquipmentItemTagStatModification()
                    {
                        ItemTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("ExplosiveWeapon_TagDef")),
                        EquipmentStatModification = new ItemStatModification()
                        {
                            TargetStat = StatModificationTarget.BonusAttackDamage,
                            Modification = StatModificationType.Multiply,
                            Value = bbDamageMod
                        }
                    },
                    new EquipmentItemTagStatModification()
                    {
                        ItemTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("ExplosiveWeapon_TagDef")),
                        EquipmentStatModification = new ItemStatModification()
                        {
                            TargetStat = StatModificationTarget.BonusAttackRange,
                            Modification = StatModificationType.Multiply,
                            Value = bbRangeMod
                        }
                    },
                    new EquipmentItemTagStatModification()
                    {
                        ItemTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("ExplosiveWeapon_TagDef")),
                        EquipmentStatModification = new ItemStatModification()
                        {
                            TargetStat = StatModificationTarget.BonusProjectilesPerShot,
                            Modification = StatModificationType.AddRestrictedToBounds,
                            Value = bbProjectileMod
                        }
                    }
                };
                bbAdditionalStatusesToApply.OfType<StanceStatusDef>().First().EquipmentsStatModifications = bbMods;
                if (!bbAdditionalStatusesToApply.Contains(bbAccModStatus))
                {
                    bbAdditionalStatusesToApply.Add(bbAccModStatus);
                }
            }

            // Convert changed list with additional statuses back to array
            (boomBlast.StatusDef as AddAttackBoostStatusDef).AdditionalStatusesToApply = bbAdditionalStatusesToApply.ToArray();
        }
    }
}
