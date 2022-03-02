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

            // Equilibrium: After using melee or bash your overwatch with handguns cost 0 AP until the end of turn
            Create_Equilibrium();

            // Rage: 0AP 4WP, Recover 2AP. Next turn your AP is halved. Limited to 1 use per turn.
            Create_Rage();
        }

        private static void Change_Dash()
        {
            //float dashRange = 13f;
            //int dashUsesPerTurn = 1;
            //string dashDescription = $"Move up to {(int)dashRange} tiles. Limited to {dashUsesPerTurn} use per turn";
            //Sprite dashIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(tave => tave.name.Equals("E_View [BodySlam_AbilityDef]")).LargeIcon;
            //
            //RepositionAbilityDef dash = Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef"));
            //dash.TargetingDataDef.Origin.Range = dashRange;
            //dash.ViewElementDef.Description = new LocalizedTextBind(dashDescription, doNotLocalize);
            //dash.ViewElementDef.LargeIcon = dashIcon;
            //dash.ViewElementDef.SmallIcon = dashIcon;
            //dash.UsesPerTurn = dashUsesPerTurn;
            //dash.AmountOfMovementToUseAsRange = -1.0f;
        }

        private static void Change_IgnorePain()
        {
            //// Remove Ignore Pain from mind control application conditions
            //MindControlStatusDef mcStatus = Repo.GetAllDefs<MindControlStatusDef>().FirstOrDefault(mcs => mcs.name.Equals("MindControl_StatusDef"));
            //List<EffectConditionDef> mcApplicationConditions = mcStatus.ApplicationConditions.ToList();
            //if (mcApplicationConditions.Remove(Repo.GetAllDefs<EffectConditionDef>().FirstOrDefault(ec => ec.name.Contains("IgnorePain"))))
            //{
            //    mcStatus.ApplicationConditions = mcApplicationConditions.ToArray();
            //}
            //// Change description
            //ApplyStatusAbilityDef ignorePain = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("IgnorePain_AbilityDef"));
            //ignorePain.ViewElementDef.Description = new LocalizedTextBind("Disabled body parts remain functional and cannot Panic.", doNotLocalize);
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

        private static void Create_Equilibrium()
        {
            string skillName = "Equilibrium_StrikeAbilityDef";
            
            ShootAbilityDef attackSource = Repo.GetAllDefs<ShootAbilityDef>().FirstOrDefault(sa => sa.name.Equals("Strike_ShootAbilityDef"));
            ShootAbilityDef followUpSource = Repo.GetAllDefs<ShootAbilityDef>().FirstOrDefault(sa => sa.name.Equals("DeadlyDuo_FollowUp_ShootAbilityDef"));

            ShootAbilityDef Equilibrium_StrikeAbility = Helper.CreateDefFromClone(
                attackSource,
                "",
                skillName);
            Equilibrium_StrikeAbility.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<AbilityCharacterProgressionDef>().FirstOrDefault(acp => acp.name.Equals("E_CharacterProgressionData [DeadlyDuo_ShootAbilityDef]")),
                "",
                skillName);
            Equilibrium_StrikeAbility.ViewElementDef = Helper.CreateDefFromClone(
                attackSource.ViewElementDef,
                "",
                skillName);
            Equilibrium_StrikeAbility.AddFollowupAbilityStatusDef = Helper.CreateDefFromClone(
                Repo.GetAllDefs<AddAbilityStatusDef>().FirstOrDefault(aas => aas.name.Equals("E_AddFollowupAbilityStatus [DeadlyDuo_ShootAbilityDef]")),
                "",
                skillName);
            Equilibrium_StrikeAbility.AddFollowupAbilityStatusDef.AbilityDef = Helper.CreateDefFromClone(
                followUpSource,
                "",
                "Equilibrium_FollowUp_ShootAbilityDef");

        }

        private static void Create_Rage()
        {

        }
    }
}
