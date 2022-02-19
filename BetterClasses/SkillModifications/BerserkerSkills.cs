using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects.ApplicationConditions;
using Base.Entities.Statuses;
using Base.UI;
using com.ootii.Collections;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Effects.ApplicationConditions;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class BerserkerSkills
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            // Dash: Move up to 13 tiles. Limited to 1 use per turn
            Change_Dash();

            // Ignore Pain: Remove MC immunity
            Change_IgnorePain();

            // Adrenaline Rush: 1 AP for one handed weapons and skills, no WP restriction
            Change_AdrenalineRush();

            // Melee Specialist: +10% damage instead of +25%
            Change_MeleeSpecialist();

            // Personal Space: Until next turn, attack first enemy entering melee range
            Create_PersonalSpace();
        }

        private static void Change_Dash()
        {
            float dashRange = 13;
            int dashUsesPerTurn = 1;
            string dashDescription = $"Move up to {(int)dashRange} tiles. Limited to {dashUsesPerTurn} use per turn";
            Sprite dashIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(tave => tave.name.Equals("E_View [BodySlam_AbilityDef]")).LargeIcon;

            RepositionAbilityDef dash = Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef"));
            dash.TargetingDataDef.Origin.Range = dashRange;
            dash.ViewElementDef.Description = new LocalizedTextBind(dashDescription, doNotLocalize);
            dash.ViewElementDef.LargeIcon = dashIcon;
            dash.ViewElementDef.SmallIcon = dashIcon;
            dash.UsesPerTurn = dashUsesPerTurn;
            dash.AmountOfMovementToUseAsRange = -1.0f;
        }

        private static void Change_IgnorePain()
        {
            // Remove Ignore Pain from mind control application conditions
            MindControlStatusDef mcStatus = Repo.GetAllDefs<MindControlStatusDef>().FirstOrDefault(mcs => mcs.name.Equals("MindControl_StatusDef"));
            List<EffectConditionDef> mcApplicationConditions = mcStatus.ApplicationConditions.ToList();
            if (mcApplicationConditions.Remove(Repo.GetAllDefs<EffectConditionDef>().FirstOrDefault(ec => ec.name.Contains("IgnorePain"))))
            {
                mcStatus.ApplicationConditions = mcApplicationConditions.ToArray();
            }
            // Change description
            ApplyStatusAbilityDef ignorePain = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("IgnorePain_AbilityDef"));
            ignorePain.ViewElementDef.Description = new LocalizedTextBind("Disabled body parts remain functional and cannot Panic.", doNotLocalize);
        }

        private static void Change_AdrenalineRush()
        {
            ApplyStatusAbilityDef adrenalineRush = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("AdrenalineRush_AbilityDef"));
            adrenalineRush.StatusDef = Repo.GetAllDefs<ChangeAbilitiesCostStatusDef>().FirstOrDefault(sd => sd.name.Equals("E_SetAbilitiesTo1AP [AdrenalineRush_AbilityDef]"));
            adrenalineRush.ViewElementDef.Description = new LocalizedTextBind("Until end of turn one-handed weapon and all non-weapon skills cost 1AP, except Recover.", doNotLocalize);
        }
        // Adrenaline Rush: Patching AbilityQualifies of ARs TacticalAbilityCostModification to determine which ability should get modified
        [HarmonyPatch(typeof(TacticalAbilityCostModification), "AbilityQualifies")]
        internal static class AR_AbilityQualifies_patch
        {
            internal static List<string> arExcludeList = new List<string>()
            {
                "RecoverWill_AbilityDef",
                "Overwatch_AbilityDef"
            };
            internal static SkillTagDef attackAbility_Tag = Repo.GetAllDefs<SkillTagDef>().FirstOrDefault(st => st.name.Equals("AttackAbility_SkillTagDef"));
            internal static ApplyStatusAbilityDef adrenalineRush = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("AdrenalineRush_AbilityDef"));
            internal static ChangeAbilitiesCostStatusDef arStatus = Repo.GetAllDefs<ChangeAbilitiesCostStatusDef>().FirstOrDefault(sd => sd.name.Equals("E_SetAbilitiesTo1AP [AdrenalineRush_AbilityDef]"));

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static void Postfix(ref bool __result, TacticalAbility ability)
            {
                try
                {
                    if (ability.TacticalActor.Status.HasStatus(arStatus))
                    {
                        if (ability.TacticalAbilityDef.SkillTags.Contains(attackAbility_Tag))
                        {
                            Equipment source = ability.GetSource<Equipment>();
                            if (source != null && source.HandsToUse == 1)
                            {
                                __result = true;
                                return;
                            }
                            else
                            {
                                __result = false;
                                return;
                            }
                        }
                        if (arExcludeList.Contains(ability.TacticalAbilityDef.name))
                        {
                            __result = false;
                            return;
                        }
                        __result = true;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        private static void Change_MeleeSpecialist()
        {
            float modValue = 1.25f;
            PassiveModifierAbilityDef meleeSpecialist = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("ExpertMelee_AbilityDef"));
            for (int i = 0; i < meleeSpecialist.ItemTagStatModifications.Length; i++)
            {
                if (meleeSpecialist.ItemTagStatModifications[i].ItemTag == Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("MeleeWeapon_TagDef")))
                {
                    meleeSpecialist.ItemTagStatModifications[i].EquipmentStatModification.Value = modValue;
                }
            }
            meleeSpecialist.ViewElementDef.DisplayName1 = new LocalizedTextBind("MELEE SPECIALIST", doNotLocalize);
            meleeSpecialist.ViewElementDef.Description = new LocalizedTextBind($"Your melee attacks deal {(modValue * 100) - 100}% more damage", doNotLocalize);
        }

        private static void Create_PersonalSpace()
        {
            //Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
            float apCost = 0.5f;
            float wpCost = 4.0f;
            
            string skillName = "PersonalSpace_AbilityDef";
            
            // Get standard melee attack ability
            BashAbilityDef psStrikeAbility = Repo.GetAllDefs<BashAbilityDef>().FirstOrDefault(ba => ba.name.Equals("Strike_ShootAbilityDef"));

            // Create main ability that applies the trigger status an the actor
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("CloseQuarters_AbilityDef"));
            ApplyStatusAbilityDef personalSpace = Helper.CreateDefFromClone(
                source,
                "d4d4ce0f-39b2-4630-97ce-96ab00f4b4ec",
                skillName);
            personalSpace.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "a9994647-673e-43a8-b3f2-22fc2933cd70",
                skillName);
            personalSpace.TargetingDataDef = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("QuickAim_AbilityDef")).TargetingDataDef;
            personalSpace.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "96297e2e-978f-46ac-b008-78c612b6c6df",
                skillName);

            // Create status that triggers when an enemy enters adjacent tiles, cloned from "CanBeRecruitedIntoPhoenix_1x1_StatusDef"
            TriggerAbilityZoneOfControlStatusDef triggerSource = Repo.GetAllDefs<TriggerAbilityZoneOfControlStatusDef>().FirstOrDefault(taz => taz.name.Equals("CanBeRecruitedIntoPhoenix_1x1_StatusDef"));
            TriggerAbilityZoneOfControlStatusDef psTriggerStatus = Helper.CreateDefFromClone(
                triggerSource,
                "75a0ef17-a35c-4275-bcc0-93d80753949a",
                $"E_TriggerStatus [{skillName}]");

            // Create EffectConditionDef's to check if melee attack ability is present and for the trigger condition that enemy is in range
            ActorHasAbilityEffectConditionDef actorHasAbility = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ActorHasAbilityEffectConditionDef>().FirstOrDefault(aha => aha.name.Equals("HasRecoverWillAbility_ApplicationCondition")),
                "611859a0-673f-4ca1-8604-584a3903423b",
                $"E_ActorHasAbility [{skillName}]");
            actorHasAbility.AbilityDef = psStrikeAbility;
            
            VisibleActorsInRangeEffectConditionDef visibleActorsInRange = Helper.CreateDefFromClone(
                Repo.GetAllDefs<VisibleActorsInRangeEffectConditionDef>().FirstOrDefault(aha => aha.name.Equals("E_VisibleActorsInRange [MasterMarksman_AbilityDef]")),
                "a505d354-b47c-43e3-b74e-d904e795c1c1",
                skillName);
            visibleActorsInRange.TargetingData.Origin.LineOfSight = LineOfSightType.InSight;
            visibleActorsInRange.TargetingData.Origin.FactionVisibility = LineOfSightType.InSight;
            visibleActorsInRange.TargetingData.Origin.Range = 1.45f;
            visibleActorsInRange.TargetingData.Origin.HorizontalRangeOnly = true;
            visibleActorsInRange.Relation = FactionRelation.Enemy;
            visibleActorsInRange.ShownMode = KnownState.Revealed;
            visibleActorsInRange.ActorsInRange = true;

            // Set fields
            psTriggerStatus.ApplicationConditions = new EffectConditionDef[0];
            psTriggerStatus.TriggerConditions = new EffectConditionDef[] { actorHasAbility, visibleActorsInRange };
            psTriggerStatus.Visuals = personalSpace.ViewElementDef;
            psTriggerStatus.VisibleOnHealthbar = TacStatusDef.HealthBarVisibility.AlwaysVisible;
            psTriggerStatus.Range = -1; // -1 = ability range will be used, melee attack ability should trigger when enemy is adjacent
            psTriggerStatus.ExecuteAbility = psStrikeAbility;
            psTriggerStatus.ExecuteAbilityWithTrait = null;
            psTriggerStatus.TargetSelf = false;
            psTriggerStatus.ApplyTimingScale = true;

            personalSpace.CharacterProgressionData.RequiredStrength = 0;
            personalSpace.CharacterProgressionData.RequiredWill = 0;
            personalSpace.CharacterProgressionData.RequiredSpeed = 0;
            personalSpace.ViewElementDef.DisplayName1 = new LocalizedTextBind("PERSONAL SPACE", doNotLocalize);
            personalSpace.ViewElementDef.Description = new LocalizedTextBind("Until your next turn, attack the first enemy entering melee range. Ends Turn.", doNotLocalize);
            Sprite personalSpaceIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(ve => ve.name.Equals("E_View [MeleeReturnFire_WithBashAbility_AbilityDef]")).SmallIcon;
            personalSpace.ViewElementDef.LargeIcon = personalSpaceIcon;
            personalSpace.ViewElementDef.SmallIcon = personalSpaceIcon;
            personalSpace.Active = true;
            personalSpace.EndsTurn = true;
            personalSpace.ActionPointCost = apCost;
            personalSpace.WillPointCost = wpCost;
            personalSpace.TraitsRequired = new string[] { "start", "ability", "move" };
            personalSpace.TraitsToApply = new string[] { "ability" };
            personalSpace.ShowNotificationOnUse = true;
            //personalSpace.TargetApplicationConditions = new EffectConditionDef[] { actorHasAbility };
            personalSpace.StatusApplicationTrigger = StatusApplicationTrigger.ActivateAbility;
            personalSpace.StatusDef = psTriggerStatus;
            AbilityDef animationSearchDef = Repo.GetAllDefs<AbilityDef>().FirstOrDefault(ad => ad.name.Equals("QuickAim_AbilityDef"));
            foreach (TacActorSimpleAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(aad => aad.name.Contains("Soldier_Utka_AnimActionsDef")))
            {
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(animationSearchDef))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(personalSpace).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                }
            }
        }
    }
}
