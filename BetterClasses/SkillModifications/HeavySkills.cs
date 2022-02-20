using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects;
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

            // War Cry: -1 AP and -10% damage, doubled if WP of target < WP of caster (see Harmony patch below)
            Change_WarCry();

            // Hunker Down: -25% incoming damage for 2 AP and 2 WP
            Create_HunkerDown();

            // Dynamic Resistance: Copy from Acheron
            Create_DynamicResistance();

            // Rage Burst: Increase accuracy and cone angle
            Change_RageBurst();

            // Jetpack Control: 2 AP jump, 12 tiles range
            Create_JetpackControl();

            // Boom Blast: -30% range instead of +50%
            Change_BoomBlast();
        }

        private static void Change_ReturnFire()
        {
            TacticalAbilityDef returnFire = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains("ReturnFire_AbilityDef"));
            returnFire.ActorTags = new GameTagDef[0]; // Deletes all given tags => no restriction for any class
        }
        private static void Change_WarCry()
        {
            ApplyStatusAbilityDef warCry = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("WarCry_AbilityDef"));
            warCry.ViewElementDef.Description = new LocalizedTextBind(
                "Enemies in 10 tiles gain -1AP and -10% damage. If their current will points are less than yours effect is doubled.",
                doNotLocalize);
            WC_Activate_patch.WC_WPCost = warCry.WillPointCost;
            StatModification warCryApMultipier = new StatModification
            {
                Modification = StatModificationType.MultiplyRestrictedToBounds,
                StatName = StatModificationTarget.ActionPoints.ToString(),
                Value = 0.75f
            };
            StatModification warCryDamageMultiplier = new StatModification
            {
                Modification = StatModificationType.MultiplyRestrictedToBounds,
                StatName = StatModificationTarget.BonusAttackDamage.ToString(),
                Value = 0.9f
            };
            ((warCry.StatusDef as DelayedEffectStatusDef).EffectDef as StatsModifyEffectDef).StatModifications = new List<StatModification> { warCryApMultipier, warCryDamageMultiplier };
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
            hunkerDown.ActionPointCost = 0.5f;
            hunkerDown.WillPointCost = 0;
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
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(animationSearchDef))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(hunkerDown).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                }
            }
        }
        public static void Create_DynamicResistance()
        {
            string skillName = "BC_DynamicResistance_AbilityDef";
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("Acheron_DynamicResistance_AbilityDef"));
            ApplyStatusAbilityDef dynamicResistance = Helper.CreateDefFromClone(
                source,
                "d6d9041b-9763-4673-a057-2bbefd96aa67",
                skillName);
            dynamicResistance.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "657f3e2b-08c0-4234-b16f-3f6d57d049e1",
                skillName);
            dynamicResistance.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "5ef3fb17-03d0-4e33-b76a-d74cbeefc509",
                skillName);
            dynamicResistance.ViewElementDef.DisplayName1 = new LocalizedTextBind("DYNAMIC RESISTANCE", doNotLocalize);
            dynamicResistance.ViewElementDef.Description = new LocalizedTextBind("Gain 50% resistance to damage type suffered this turn", doNotLocalize);
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
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(source))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(jetpackControl).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                }
            }
        }
        private static void Change_BoomBlast()
        {
            float bbRangeModValue = 0.8f;
            ApplyStatusAbilityDef boomBlast = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("BigBooms_AbilityDef"));
            boomBlast.ViewElementDef.Description = new LocalizedTextBind(
                $"The Action Point cost of Grenades, and other explosive weapons, is reduced by 1 and their range is modified by {(bbRangeModValue * 100) - 100}% until the end of the turn.",
                doNotLocalize);
            EquipmentItemTagStatModification bbRangeMod = new EquipmentItemTagStatModification()
            {
                ItemTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("ExplosiveWeapon_TagDef")),
                EquipmentStatModification = new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.BonusAttackRange,
                    Modification = StatModificationType.Multiply,
                    Value = bbRangeModValue
                }
            };
            (boomBlast.StatusDef as AddAttackBoostStatusDef).AdditionalStatusesToApply.OfType<StanceStatusDef>().First().EquipmentsStatModifications = new EquipmentItemTagStatModification[]
            {
                bbRangeMod
            };
        }

        // War Cry Harmony patches
        // Keep track of target actor when WP of target are lower in the moment of casting War Cry
        internal static List<string> WarCryLowerWpList = new List<string>();
        // Harmony patch at the moment the source actor casts War Cry to track if WP of target actor is lower
        [HarmonyPatch(typeof(ApplyStatusAbility), "Activate")]
        internal static class WC_Activate_patch
        {

            public static float WC_WPCost;
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static void Postfix(ApplyStatusAbility __instance)
            {
                try
                {
                    if (__instance.ApplyStatusAbilityDef.name.Equals("WarCry_AbilityDef"))
                    {
                        TacticalActor sourceActorFromBase = (TacticalActor)AccessTools.Property(typeof(TacticalAbility), "TacticalActor").GetValue(__instance, null);
                        MethodInfo methodInfo_GetTargetActors = AccessTools.Method(typeof(TacticalAbility), "GetTargetActors", new Type[] { typeof(TacticalTargetData) });
                        using (IEnumerator<TacticalAbilityTarget> enumerator = ((IEnumerable<TacticalAbilityTarget>)methodInfo_GetTargetActors.Invoke(
                            __instance,
                            new object[] { __instance.OriginTargetData })).GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                TacticalActorBase targetActor = enumerator.Current.GetTargetActor();
                                float sourceActorWP = sourceActorFromBase.CharacterStats.WillPoints + WC_WPCost; // Add WP cost for War Cry to let the WP compararison behave as would it be before the cast.
                                float targetActorWP = targetActor.Status.GetStat(StatModificationTarget.WillPoints.ToString(), null);
                                Logger.Debug("War Cry ability <Activate> method called from ...");
                                Logger.Debug("  Source actor      : " + sourceActorFromBase.name);
                                Logger.Debug("  Source actor WP   : " + sourceActorWP);
                                Logger.Debug("  Target actor      : " + targetActor.name);
                                Logger.Debug("  Target actor WP   : " + targetActorWP);
                                Logger.Debug("----------------------------------------------------");
                                Logger.Debug("  Target actor AP and damage statuses: " + targetActor.name);
                                foreach (StatModification statModAp1 in targetActor.Status.GetStat(StatModificationTarget.ActionPoints.ToString()).Modifications)
                                {
                                    Logger.Debug("    " + statModAp1);
                                }
                                foreach (StatModification statModBad1 in targetActor.Status.GetStat(StatModificationTarget.BonusAttackDamage.ToString()).Modifications)
                                {
                                    Logger.Debug("    " + statModBad1);
                                }
                                if (Utl.LesserThan(targetActorWP, sourceActorWP))
                                {
                                    WarCryLowerWpList.Add(targetActor.name);
                                    Logger.Debug("  Target actor '" + targetActor.name + "' added to <WarCryLowerWpList>");
                                    //targetActor.Status.UnapplyStatus()
                                }
                                Logger.Debug("----------------------------------------------------", false);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        // Harmony patch after the War Cry status applied the StatModification effect on a target 
        [HarmonyPatch(typeof(DelayedEffectStatus), "OnUnapply")]
        internal static class WC_OnUnapply_patch
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static void Postfix(DelayedEffectStatus __instance)
            {
                try
                {
                    if (__instance.DelayedEffectStatusDef.name.Equals("E_Status [WarCry_AbilityDef]"))
                    {
                        TacticalActorBase targetActor = (TacticalActorBase)AccessTools.Property(typeof(TacStatus), "TacticalActorBase").GetValue(__instance);
                        DefRepository repo = (DefRepository)AccessTools.Property(typeof(TacStatus), "Repo").GetValue(__instance);
                        object source = AccessTools.Property(typeof(TacStatus), "Source").GetValue(__instance);
                        Logger.Debug("War Cry status <OnUnapply> called from ...");
                        Logger.Debug("  Target actor      : " + targetActor.name);
                        Logger.Debug("    " + targetActor.Status.GetStat(StatModificationTarget.WillPoints.ToString()));
                        foreach (StatModification statModAp1 in targetActor.Status.GetStat(StatModificationTarget.ActionPoints.ToString()).Modifications)
                        {
                            Logger.Debug("    " + statModAp1);
                        }
                        foreach (StatModification statModBad1 in targetActor.Status.GetStat(StatModificationTarget.BonusAttackDamage.ToString()).Modifications)
                        {
                            Logger.Debug("    " + statModBad1);
                        }
                        Logger.Debug("----------------------------------------------------");
                        if (WarCryLowerWpList.Contains(targetActor.name))
                        {
                            Logger.Debug("  Target actor was added to <WarCryLowerWpList>, apply the penalty effect a 2nd time and remove him from the list.");
                            EffectTarget actorEffectTarget = TacUtil.GetActorEffectTarget(targetActor, null);
                            _ = Effect.Apply(repo, __instance.DelayedEffectStatusDef.EffectDef, actorEffectTarget, source ?? __instance);
                            _ = WarCryLowerWpList.Remove(targetActor.name);
        
                            foreach (StatModification statModAp2 in targetActor.Status.GetStat(StatModificationTarget.ActionPoints.ToString()).Modifications)
                            {
                                Logger.Debug("    " + statModAp2);
                            }
                            foreach (StatModification statModBad2 in targetActor.Status.GetStat(StatModificationTarget.BonusAttackDamage.ToString()).Modifications)
                            {
                                Logger.Debug("    " + statModBad2);
                            }
                        }
                        Logger.Debug("----------------------------------------------------", false);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
