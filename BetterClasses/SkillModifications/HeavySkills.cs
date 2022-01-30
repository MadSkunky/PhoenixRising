using Base.Core;
using Base.Defs;
using Base.Entities.Effects;
using Base.Entities.Statuses;
using Base.UI;
using Base.Utils.Maths;
using Harmony;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Tactical;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class HeavySkills
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void ApplyChanges(bool doNotLocalize = true)
        {
            // Return Fire: Fix to work on all classes
            TacticalAbilityDef returnFire = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains("ReturnFire_AbilityDef"));
            returnFire.ActorTags = new GameTagDef[0]; // Deletes all given tags => no restriction for any class

            // War Cry: -1 AP and -10% damage, doubled if WP of target < WP of caster (see Harmony patch below)
            ApplyStatusAbilityDef warCry = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("WarCry_AbilityDef"));
            warCry.ViewElementDef.Description = new LocalizedTextBind(
                "Enemies in 10 tiles gain -1AP and -10% damage. If their current will points are less than yours effect is doubled.",
                doNotLocalize);
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

            // Rage Burst: Increase accuracy and cone angle

            // Dynamic Resistance: Copy from Acheron

            // Hunker Down: -25% incoming damage for 2 AP and 2 WP

            // Jetpack Control: 2 AP jump, 12 tiles range

            // Boom Blast: -30% range instead of +50%
        }

        // War Cry Harmony patches
        // Keep track of target actor when WP of target are lower in the moment of casting War Cry
        internal static List<string> WarCryLowerWpList = new List<string>();
        // Harmony patch at the moment the source actor casts War Cry to track if WP of target actor is lower
        [HarmonyPatch(typeof(ApplyStatusAbility), "Activate")]
        internal static class WC_Activate_patch
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static void Postfix(ApplyStatusAbility __instance, object parameter)
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
                                float sourceActorWP = sourceActorFromBase.CharacterStats.WillPoints + 6;
                                float targetActorWP = targetActor.Status.GetStat(StatModificationTarget.WillPoints.ToString(), null);
                                Logger.Always("War Cry ability <Activate> method called from ...");
                                Logger.Always("  Source actor      : " + sourceActorFromBase.name);
                                Logger.Always("  Source actor WP   : " + sourceActorWP);
                                Logger.Always("  Target actor      : " + targetActor.name);
                                Logger.Always("  Target actor WP   : " + targetActorWP);
                                Logger.Always("----------------------------------------------------");
                                Logger.Always("  Target actor AP and damage statuses: " + targetActor.name);
                                foreach (StatModification statModAp1 in targetActor.Status.GetStat(StatModificationTarget.ActionPoints.ToString()).Modifications)
                                {
                                    Logger.Always("    " + statModAp1);
                                }
                                foreach (StatModification statModBad1 in targetActor.Status.GetStat(StatModificationTarget.BonusAttackDamage.ToString()).Modifications)
                                {
                                    Logger.Always("    " + statModBad1);
                                }
                                if (Utl.LesserThan(targetActorWP, sourceActorWP))
                                {
                                    WarCryLowerWpList.Add(targetActor.name);
                                    Logger.Always("  Target actor '" + targetActor.name + "' added to <WarCryLowerWpList>");
                                    //targetActor.Status.UnapplyStatus()
                                }
                                Logger.Always("----------------------------------------------------", false);
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
                        Logger.Always("War Cry status <OnUnapply> called from ...");
                        Logger.Always("  Target actor      : " + targetActor.name);
                        Logger.Always("    " + targetActor.Status.GetStat(StatModificationTarget.WillPoints.ToString()));
                        foreach (StatModification statModAp1 in targetActor.Status.GetStat(StatModificationTarget.ActionPoints.ToString()).Modifications)
                        {
                            Logger.Always("    " + statModAp1);
                        }
                        foreach (StatModification statModBad1 in targetActor.Status.GetStat(StatModificationTarget.BonusAttackDamage.ToString()).Modifications)
                        {
                            Logger.Always("    " + statModBad1);
                        }
                        Logger.Always("----------------------------------------------------");
                        if (WarCryLowerWpList.Contains(targetActor.name))
                        {
                            Logger.Always("  Target actor was added to <WarCryLowerWpList>, apply the penalty effect a 2nd time and remove him from the list.");
                            EffectTarget actorEffectTarget = TacUtil.GetActorEffectTarget(targetActor, null);
                            _ = Effect.Apply(repo, __instance.DelayedEffectStatusDef.EffectDef, actorEffectTarget, source ?? __instance);
                            _ = WarCryLowerWpList.Remove(targetActor.name);
        
                            foreach (StatModification statModAp2 in targetActor.Status.GetStat(StatModificationTarget.ActionPoints.ToString()).Modifications)
                            {
                                Logger.Always("    " + statModAp2);
                            }
                            foreach (StatModification statModBad2 in targetActor.Status.GetStat(StatModificationTarget.BonusAttackDamage.ToString()).Modifications)
                            {
                                Logger.Always("    " + statModBad2);
                            }
                        }
                        Logger.Always("----------------------------------------------------", false);
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
