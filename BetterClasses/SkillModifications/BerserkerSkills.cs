using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects;
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
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Entities.Weapons;
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
            float wpCost = 3.0f;
            string skillName = "Equilibrium_StrikeAbilityDef";
            string hgQaSkillName = "Equilibrium_HandgunQuickAim_AbilityDef";

            // Get some basics from repo
            ShootAbilityDef standardMeleeAbility = Repo.GetAllDefs<ShootAbilityDef>().FirstOrDefault(sa => sa.name.Equals("Strike_ShootAbilityDef"));
            ApplyStatusAbilityDef martialArtist = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("MartialArtist_AbilityDef"));
            GameTagDef handgunWeaponTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("HandgunItem_TagDef"));
            GameTagDef meleeWeaponTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("MeleeWeapon_TagDef"));
            //SkillTagDef attackSkillTag = Repo.GetAllDefs<SkillTagDef>().FirstOrDefault(st => st.name.Equals("AttackAbility_SkillTagDef"));
            //DamageTypeBaseEffectDef meleeDamage = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtbe => dtbe.name.Equals("MeleeBash_StandardDamageTypeEffectDef"));
            //DamageTypeBaseEffectDef slashDamage = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtbe => dtbe.name.Equals("Slash_StandardDamageTypeEffectDef"));
            //DamageTypeBaseEffectDef bashDamage = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtbe => dtbe.name.Equals("Bash_StandardDamageTypeEffectDef"));
            //ActorLastDamageTypeEffectConditionDef actorLastDamage = Repo.GetAllDefs<ActorLastDamageTypeEffectConditionDef>().FirstOrDefault(ald => ald.name.Equals("ActorLastDamageType_Fire_ApplicationCondition"));

            Sprite icon = martialArtist.ViewElementDef.LargeIcon;
            LocalizedTextBind name = new LocalizedTextBind("EQUILIBRIUM", doNotLocalize);
            LocalizedTextBind descripion = new LocalizedTextBind("After using melee attack your next shot with handguns cost 0 AP.", doNotLocalize);

            // Create necessary Defs
            ShootAbilityDef Equilibrium = Helper.CreateDefFromClone(
                standardMeleeAbility,
                "1217a22e-0857-4094-a548-d224db6776a2",
                skillName);
            Equilibrium.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "cf65b3b6-ec33-48ab-a08f-71a3cb44567a",
                skillName);
            Equilibrium.TargetingDataDef = Helper.CreateDefFromClone(
                standardMeleeAbility.TargetingDataDef,
                "790233f5-5aa5-4769-931c-c2f740271836",
                skillName);
            Equilibrium.ViewElementDef = Helper.CreateDefFromClone(
                standardMeleeAbility.ViewElementDef,
                "008272c9-2431-4681-a0a1-3bf61f3462bb",
                skillName);
            Equilibrium.AddFollowupAbilityStatusDef = Helper.CreateDefFromClone(
                Repo.GetAllDefs<AddAbilityStatusDef>().FirstOrDefault(aas => aas.name.Equals("E_AddAbilityStatus [DeployBeacon_StatusDef]")),
                "40d9f907-a5a4-4f9a-bc12-e1a3f5459b3e",
                skillName);
            // AP reduction for handguns
            ApplyStatusAbilityDef HandGunQA = Helper.CreateDefFromClone(
                martialArtist,
                "4c6e3ad0-787a-4185-9011-f568f382abba",
                hgQaSkillName);
            HandGunQA.CharacterProgressionData = Helper.CreateDefFromClone(
                martialArtist.CharacterProgressionData,
                "0aefa178-33db-4d96-8d95-b548cec1a848",
                hgQaSkillName);
            HandGunQA.TargetingDataDef = Helper.CreateDefFromClone(
                martialArtist.TargetingDataDef,
                "c6fdce21-fd70-4c8c-a92a-b623715c8762",
                hgQaSkillName);
            HandGunQA.ViewElementDef = Helper.CreateDefFromClone(
                martialArtist.ViewElementDef,
                "d20f2149-a24b-4419-8a7f-b86bb7837a4d",
                hgQaSkillName);
            HandGunQA.StatusDef = Helper.CreateDefFromClone(
                martialArtist.StatusDef,
                "f0bdbe30-2947-49f6-a1e7-276c6245861b",
                hgQaSkillName);

            // Set fields
            Equilibrium.ViewElementDef.DisplayName1 = name;
            Equilibrium.ViewElementDef.Description = descripion;
            Equilibrium.ViewElementDef.LargeIcon = icon;
            Equilibrium.ViewElementDef.SmallIcon = icon;
            Equilibrium.ViewElementDef.DisplayPriority = 5;
            Equilibrium.ViewElementDef.ShowInStatusScreen = true;
            Equilibrium.WillPointCost = wpCost;
            Equilibrium.EquipmentTags = new GameTagDef[] { meleeWeaponTag };
            Equilibrium.IsDefault = false;
            Equilibrium.AddFollowupAbilityStatusDef.AbilityDef = HandGunQA;
            TacticalAbilityCostModification costModification = new TacticalAbilityCostModification()
            {
                TargetAbilityTagDef = null,
                AbilityCullFilter = null,
                SkillTagCullFilter = null,
                EquipmentTagDef = handgunWeaponTag,
                RequiresProficientEquipment = true,
                ActionPointModType = TacticalAbilityModificationType.Add,
                ActionPointMod = -0.25f
            };
            (HandGunQA.StatusDef as ChangeAbilitiesCostStatusDef).AbilityCostModification = costModification;
        }

        private static void Create_Rage()
        {

        }
    }
}
