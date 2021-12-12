using System;
using System.Collections.Generic;
using System.Linq;
using Base.Defs;
using Base.Utils.Maths;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixPoint.Tactical.Levels;
using UnityEngine;

namespace PhoenixRising.SkillRework
{
    class MainSpecModification
    {
        public static void GenerateMainSpec(DefRepository Repo, Settings Config)
        {
            try
            {
                // Fix for Return Fire to work on all classes
                TacticalAbilityDef returnFire = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains("ReturnFire_AbilityDef"));
                returnFire.ActorTags = new GameTagDef[0]; // Deletes all given tags

                LevelProgressionDef levelProgressionDef = Repo.GetAllDefs<LevelProgressionDef>().FirstOrDefault(lpd => lpd.name.Contains("LevelProgressionDef"));
                int secondaryClassLevel = levelProgressionDef.SecondSpecializationLevel;
                int secondaryClassCost = levelProgressionDef.SecondSpecializationSpCost;
                string ability;
                foreach (AbilityTrackDef abilityTrackDef in Repo.GetAllDefs<AbilityTrackDef>())
                {
                    if (Config.ClassSpecializations.Any(c => abilityTrackDef.name.Contains(c.ClassName)))
                    {
                        ClassSpecDef classSpec = Config.ClassSpecializations.Find(c => abilityTrackDef.name.Contains(c.ClassName));
                        string[] configMainSpec = classSpec.MainSpec;
                        if (abilityTrackDef.AbilitiesByLevel.Length != configMainSpec.Length)
                        {
                            Logger.Always("Not enough or too much level skills for 1st row are configured, some may not be set!");
                            Logger.Always("Class preset: " + classSpec.ClassName);
                            Logger.Always("Number of skills configured (should be 7): " + configMainSpec.Length);
                        }
                        for (int i = 0; i < abilityTrackDef.AbilitiesByLevel.Length && i < configMainSpec.Length; i++)
                        {
                            // 0 = main class proficiency and 3 = secondary class selector skipped, main class is in the config but also skipped here to prevent bugs by misconfiguration
                            if (i != 0 && i != 3)
                            {
                                if (Helper.AbilityNameToDefMap.ContainsKey(configMainSpec[i]))
                                {
                                    ability = Helper.AbilityNameToDefMap[configMainSpec[i]];
                                    abilityTrackDef.AbilitiesByLevel[i].Ability = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains(ability));
                                    abilityTrackDef.AbilitiesByLevel[i].Ability.CharacterProgressionData.SkillPointCost = Helper.SPperLevel[i];
                                    abilityTrackDef.AbilitiesByLevel[i].Ability.CharacterProgressionData.MutagenCost = Helper.SPperLevel[i];
                                    Logger.Debug("Class '" + classSpec.ClassName + "' level " + i + 1 + " skill set to: " + abilityTrackDef.AbilitiesByLevel[i].Ability.ViewElementDef.DisplayName1.LocalizeEnglish());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
/*
        // Overwrite preview of new recruits for PX, original crashed with more than 3 abilities
        [HarmonyPatch(typeof(TacticalLevelController), "GetReturnFireAbilities")]
        internal static class GetReturnFireAbilities_Patches
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static bool Prefix(ref List<ReturnFireAbility> __result,
                                       TacticalLevelController __instance,
                                       TacticalActor shooter,
                                       Weapon weapon,
                                       TacticalAbilityTarget target,
                                       ShootAbility shootAbility,
                                       bool getOnlyPossibleTargets = false,
                                       List<TacticalActor> casualties = null)
            {
                try
                {
                    Logger.Debug("------------ Enter GetReturnFireAbilities --------------");
                    Weapon weapon2 = weapon;
                    WeaponDef weaponDef = (weapon2 != null) ? weapon2.WeaponDef : null;
                    if (target.AttackType == AttackType.ReturnFire
                        || target.AttackType == AttackType.Overwatch
                        || target.AttackType == AttackType.Synced
                        || target.AttackType == AttackType.ZoneControl
                        || (weaponDef != null && weaponDef.NoReturnFireFromTargets))
                    {
                        Logger.Debug("AttackType check negative -> leaving GetReturnFireAbilities with NULL result!");
                        Logger.Debug("------- Exit GetReturnFireAbilities sucessful ----------");
                        __result = null;
                        return false;
                    }
                    Logger.Debug("AttackType check successful passed.");
                    List<ReturnFireAbility> result;
                    using (new MultiForceDummyTargetableLock(__instance.Map.GetActors<TacticalActor>(null)))
                    {
                        foreach (TacticalActor tmpActor in __instance.Map.GetActors<TacticalActor>(null))
                        {
                            Logger.Debug("------------------------- Begin Actor data ----------------------------");
                            Logger.Debug("Actor name: " + tmpActor.GetDisplayName());
                            if (tmpActor.GetAbilities<ReturnFireAbility>() != null)
                            {
                                Logger.Debug("                     Actor return fire abilities: " + string.Join(", ", tmpActor.GetAbilities<ReturnFireAbility>()));
                                Logger.Debug("RF ability IgnoreNoValidTargetsFilter is enabled: " + tmpActor.GetAbilities<ReturnFireAbility>().FirstOrDefault().IsEnabled(IgnoredAbilityDisabledStatesFilter.IgnoreNoValidTargetsFilter));
                                Logger.Debug("                       RF ability disabled state: " + tmpActor.GetAbilities<ReturnFireAbility>().FirstOrDefault().GetDisabledState().Key);
                            }
                            else
                            {
                                Logger.Debug("No return fire abilities fourn on actor. ");
                            }
                            Logger.Debug("------------------------- End Actor data ------------------------------");
                        }
                        List<ReturnFireAbility> list = (from actor in __instance.Map.GetActors<TacticalActor>(null)
                                                        where actor.IsAlive && actor.RelationTo(shooter) == FactionRelation.Enemy
                                                        from ability in
                                                            from a in actor.GetAbilities<ReturnFireAbility>()
                                                            orderby a.ReturnFireDef.ReturnFirePriority
                                                            select a
                                                        where ability.IsEnabled(IgnoredAbilityDisabledStatesFilter.IgnoreNoValidTargetsFilter)
                                                        group ability by actor into actorReturns
                                                        let actorAbility = actorReturns.First<ReturnFireAbility>()
                                                        orderby actorAbility.TacticalActor == target.GetTargetActor() descending
                                                        select actorAbility).Where(delegate (ReturnFireAbility returnFireAbility)
                                                        {
                                                            Logger.Debug("Return fire ablity on actor " + returnFireAbility.TacticalActorBase.DisplayName + "found: " + returnFireAbility.AbilityDef.name);
                                                            TacticalActor actor = returnFireAbility.TacticalActor;
                                                            if (weapon != null)
                                                            {
                                                                Logger.Debug("Weapon != null: " + weapon.DisplayName + " -> place target for shooting.");
                                                                shooter.TargetDummy.PlaceForShooting(weapon, target.ShootFromPos, target, false);
                                                            }
                                                            if (!returnFireAbility.IsValidTarget(shooter))
                                                            {
                                                                Logger.Debug("Return fire has no valid target!");
                                                                return false;
                                                            }
                                                            if (returnFireAbility.ReturnFireDef.RiposteWithBashAbility)
                                                            {
                                                                Logger.Debug("Return fire has riposte with bash set, returns true.");
                                                                return true;
                                                            }
                                                            if (getOnlyPossibleTargets
                                                                && target.Actor != actor
                                                                && (target.MultiAbilityTargets == null || !target.MultiAbilityTargets.Any((TacticalAbilityTarget mat) => mat.Actor == actor))
                                                                && (casualties == null || !casualties.Contains(actor)))
                                                            {
                                                                Logger.Debug("Multiple check (getOnlyPossibleTargets & target.Actor != actor ... is true, returns false!");
                                                                return false;
                                                            }
                                                            ShootAbility defaultShootAbility = returnFireAbility.GetDefaultShootAbility();
                                                            TacticalAbilityTarget attackActorTarget = defaultShootAbility.GetAttackActorTarget(shooter, AttackType.ReturnFire);
                                                            if (attackActorTarget == null || !Utl.Equals(attackActorTarget.ShootFromPos, defaultShootAbility.Actor.Pos, 1E-05f))
                                                            {
                                                                Logger.Debug("attackTarget is NULL or (attackActorTarget.ShootFromPos, defaultShootAbility.Actor.Pos) is NOT equal, returns false!");
                                                                return false;
                                                            }
                                                            TacticalActor tacticalActor = null;
                                                            if (returnFireAbility.TacticalActor.TacticalPerception.CheckFriendlyFire(returnFireAbility.Weapon,
                                                                                                                                     attackActorTarget.ShootFromPos,
                                                                                                                                     attackActorTarget,
                                                                                                                                     out tacticalActor,
                                                                                                                                     FactionRelation.Neutral | FactionRelation.Friend))
                                                            {
                                                                Logger.Debug("Check friendly fire triggered, return false!");
                                                                return false;
                                                            }
                                                            if (!returnFireAbility.TacticalActor.TacticalPerception.HasFloorSupportAt(returnFireAbility.TacticalActor.Pos))
                                                            {
                                                                Logger.Debug("Check perception triggered, return false!");
                                                                return false;
                                                            }
                                                            TacticalActorBase tacticalActor2 = returnFireAbility.TacticalActor;
                                                            Vector3 pos = returnFireAbility.TacticalActor.Pos;
                                                            TacticalActorBase shooter2 = shooter;
                                                            bool checkAllPoints = false;
                                                            TacticalAbilityTarget target2 = target;
                                                            return TacticalFactionVision.CheckVisibleLineBetweenActors(tacticalActor2,
                                                                                                                       pos,
                                                                                                                       shooter2,
                                                                                                                       checkAllPoints,
                                                                                                                       new Vector3?((target2 != null) ? target2.ShootFromPos : shooter.Pos),
                                                                                                                       0.5f,
                                                                                                                       null);
                                                        }).ToList<ReturnFireAbility>();
                        if (weapon != null)
                        {
                            Logger.Debug("Weapon check successful passed. Call 'PlaceForShooting' on shooter: " + shooter.DisplayName);
                            shooter.TargetDummy.PlaceForShooting(weapon, shooter.Pos, target, false);
                        }
                        result = list;
                    }
                    Logger.Debug("Return Fire List count: " + result.Count);
                    __result = result;
                    Logger.Debug("------- Exit GetReturnFireAbilities sucessful ----------");
                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    Logger.Debug("------- Exit GetReturnFireAbilities with error! --------");
                    return true;
                }
            }
        }*/
    }
}
