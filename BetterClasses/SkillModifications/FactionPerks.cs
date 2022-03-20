using AK.Wwise;
using Base;
using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects;
using Base.Entities.Effects.ApplicationConditions;
using Base.Entities.Statuses;
using Base.Levels;
using Base.UI;
using Base.Utils.Maths;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Tactical;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Effects;
using PhoenixPoint.Tactical.Entities.Effects.ApplicationConditions;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static PhoenixPoint.Tactical.Entities.Statuses.StatMultiplierStatusDef;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class FactionPerks
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            //OW Focus: Change icon to 'UI_AbilitiesIcon_EquipmentAbility_OverwatchFocus-2.png'
            Change_OWFocus();
            //Takedown: Your bash and melee attacks gain +100 Shock value.
            Create_Takedown();
            //Shadowstep: No changes
            Change_Shadowstep();
            //Rally: 1AP 4WP, 'Until next turn: Allies in 10 tile radius can use disabled limbs and gain panic immunity', icon to LargeIcon from 'E_View [Acheron_CallReinforcements_AbilityDef]'
            //Change_Rally();
            //Phantom Protocol: 0AP 3WP, You gain +25% accuracy and stealth until next turn
            Create_PhantomProtocol();
            //Pain Chameleon:  Maybe no change, to check if one of the ..._PainChameleon_AbilityDef will work
            Change_PainChameloen();
            //Sower of Change: Passive, Returns 10% of damage as Viral to the attacker within 10 tiles
            Create_SowerOfChange();
            //Breathe Mist: Adding progression def
            Change_BreatheMist();
            //Resurrect: 3AP 6WP, to check if the Mutoid_ResurrectAbilityDef will work, change to only allow 1 ressurect at one time (same as MC)
            Change_Resurrect();
            //Pepper Cloud: 1AP 2WP, to check if the Mutoid_PepperCloud_ApplyStatusAbilityDef will work, change range from 5 to 8 tiles
            Change_PepperCloud();
            //AR Targeting: 2AP 2WP, Target ally gains +20% accuracy
            Create_AR_Targeting();
            //Endurance: Create new with 'Recover Restores 75% WP (instead of 50%)', check cloning from 'RecoverWill_AbilityDef', icon to LargeIcon from 'Reckless_AbilityDef'
            Create_Endurance();
        }

        private static void Change_OWFocus()
        {
            OverwatchFocusAbilityDef overwatchFocus = Repo.GetAllDefs<OverwatchFocusAbilityDef>().FirstOrDefault(of => of.name.Equals("OverwatchFocus_AbilityDef"));
            Sprite owSprite = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_EquipmentAbility_OverwatchFocus-2.png");
            overwatchFocus.ViewElementDef.LargeIcon = owSprite;
            overwatchFocus.ViewElementDef.SmallIcon = owSprite;
        }
        private static void Create_Takedown()
        {
            string skillName = "BC_Takedown_AbilityDef";
            float bashDamage = 60f;
            float bashShock = 160f;
            //float meleeShockAddition = 100.0f;
            LocalizedTextBind displayName = new LocalizedTextBind("TAKEDOWN", doNotLocalize);
            LocalizedTextBind description = new LocalizedTextBind($"Deal {(int)bashDamage} damage and {(int)bashShock} shock damage to an adjacent target. Replaces Bash.", doNotLocalize);
            Sprite icon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(tave => tave.name.Equals("E_ViewElement [Brawler_AbilityDef]")).LargeIcon;

            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("WeakSpot_AbilityDef"));
            ApplyStatusAbilityDef takedown = Helper.CreateDefFromClone(
                source,
                "d2711bfc-b4cb-46dd-bb9f-599a88c1ebff",
                skillName);
            takedown.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "f7ce1c44-1447-41a3-8112-666c82451e25",
                skillName);
            takedown.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "0324925f-e318-40b6-ac8c-b68033823cd9",
                skillName);
            // Set usual fields for new created base ability (Takedown)
            takedown.CharacterProgressionData.RequiredStrength = 0;
            takedown.CharacterProgressionData.RequiredWill = 0;
            takedown.CharacterProgressionData.RequiredSpeed = 0;
            takedown.ViewElementDef.DisplayName1 = displayName;
            takedown.ViewElementDef.Description = description;
            takedown.ViewElementDef.LargeIcon = icon;
            takedown.ViewElementDef.SmallIcon = icon;

            // Create a new Bash ability by cloning from standard Bash with fixed damage and shock values
            BashAbilityDef bashToRemoveAbility = Repo.GetAllDefs<BashAbilityDef>().FirstOrDefault(gt => gt.name.Equals("Bash_WithWhateverYouCan_AbilityDef"));
            BashAbilityDef bashAbility = Helper.CreateDefFromClone(
                bashToRemoveAbility,
                "b2e1ecee-ad51-445f-afc4-6d2f629a8422",
                "Takedown_Bash_AbilityDef");
            bashAbility.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
            {
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.DamageKeyword,
                    Value = bashDamage
                },
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.ShockKeyword,
                    Value = bashShock
                }
            };
            bashAbility.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "e9617a5a-32ae-46a2-b9ca-538956470c0f",
                skillName);
            bashAbility.ViewElementDef.ShowInStatusScreen = false;
            bashAbility.ViewElementDef.DisplayName1 = displayName;
            bashAbility.ViewElementDef.Description = new LocalizedTextBind($"Deal {(int)bashDamage} damage and {(int)bashShock} shock damage to an adjacent target.", doNotLocalize);
            bashAbility.ViewElementDef.LargeIcon = icon;
            bashAbility.ViewElementDef.SmallIcon = icon;
            bashAbility.BashWith = BashAbilityDef.BashingWith.SelectedEquipmentOrBareHands;

            // Create a status to apply the bash ability to the actor
            AddAbilityStatusDef addNewBashAbiltyStatus = Helper.CreateDefFromClone( // Borrow status from Deplay Beacon (final mission)
                Repo.GetAllDefs<AddAbilityStatusDef>().FirstOrDefault(a => a.name.Equals("E_AddAbilityStatus [DeployBeacon_StatusDef]")),
                "f084d230-9ad4-4315-a49d-d5e73c954254",
                $"E_ApplyNewBashAbilityEffect [{skillName}]");
            addNewBashAbiltyStatus.DurationTurns = -1;
            addNewBashAbiltyStatus.SingleInstance = true;
            addNewBashAbiltyStatus.ExpireOnEndOfTurn = false;
            addNewBashAbiltyStatus.AbilityDef = bashAbility;

            // Create an effect that removes the standard Bash from the actors abilities
            RemoveAbilityEffectDef removeRegularBashAbilityEffect = Helper.CreateDefFromClone(
                Repo.GetAllDefs<RemoveAbilityEffectDef>().FirstOrDefault(rae => rae.name.Equals("RemoveAuraAbilities_EffectDef")),
                "b4bba4bf-f568-42b5-8baf-0169b7aa218a",
                $"E_RemoveRegularBashAbilityEffect [{skillName}]");
            removeRegularBashAbilityEffect.AbilityDefs = new AbilityDef[] { bashToRemoveAbility };

            // Create a status that applies the remove ability effect to the actor
            TacEffectStatusDef applyRemoveAbilityEffectStatus = Helper.CreateDefFromClone(
                Repo.GetAllDefs<TacEffectStatusDef>().FirstOrDefault(tes => tes.name.Equals("Mist_spawning_StatusDef")),
                "1a9ba75a-8075-4e07-8a13-b23798eda4a0",
                $"E_ApplyRemoveAbilityEffect [{skillName}]");
            applyRemoveAbilityEffectStatus.EffectName = "";
            applyRemoveAbilityEffectStatus.DurationTurns = -1;
            applyRemoveAbilityEffectStatus.ExpireOnEndOfTurn = false;
            applyRemoveAbilityEffectStatus.Visuals = null;
            applyRemoveAbilityEffectStatus.EffectDef = removeRegularBashAbilityEffect;
            applyRemoveAbilityEffectStatus.StatusAsEffectSource = false;
            applyRemoveAbilityEffectStatus.ApplyOnStatusApplication = true;
            applyRemoveAbilityEffectStatus.ApplyOnTurnStart = true;

            // Create a multi status to hold all statuses that Takedown applies to the actor
            MultiStatusDef multiStatus = Helper.CreateDefFromClone( // Borrow multi status from Rapid Clearance
                Repo.GetAllDefs<MultiStatusDef>().FirstOrDefault(m => m.name.Equals("E_MultiStatus [RapidClearance_AbilityDef]")),
                "f4bc1190-c87c-4162-bf86-aa797c82d5d2",
                skillName);
            multiStatus.Statuses = new StatusDef[] { addNewBashAbiltyStatus, applyRemoveAbilityEffectStatus };

            takedown.StatusDef = multiStatus;

            //TacActorAimingAbilityAnimActionDef noWeaponBashAnim = Repo.GetAllDefs<TacActorAimingAbilityAnimActionDef>().FirstOrDefault(aa => aa.name.Equals("E_NoWeaponBash [Soldier_Utka_AnimActionsDef]"));
            //if (!noWeaponBashAnim.AbilityDefs.Contains(bashAbility))
            //{
            //    noWeaponBashAnim.AbilityDefs = noWeaponBashAnim.AbilityDefs.Append(bashAbility).ToArray();
            //}

            // Adding new bash ability to proper animations
            foreach (TacActorAimingAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorAimingAbilityAnimActionDef>().Where(aad => aad.name.Contains("Soldier_Utka_AnimActionsDef")))
            {
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(bashToRemoveAbility) && !animActionDef.AbilityDefs.Contains(bashAbility))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(bashAbility).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                    Logger.Debug("----------------------------------------------------", false);
                }
            }
        }

        private static void Change_Shadowstep()
        {
            Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' no changes implemented yet!");
            Logger.Debug("----------------------------------------------------", false);
        }
        //private static void Change_Rally()
        //{
        //    string skillName = "BC_Rally_AbilityDef";
        //    ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("Rally_AbilityDef"));
        //    ApplyStatusAbilityDef rally = Helper.CreateDefFromClone(
        //        source,
        //        "edea324b-e435-416f-bb93-8e1ea16d2e64",
        //        skillName);
        //    rally.CharacterProgressionData = Helper.CreateDefFromClone(
        //        source.CharacterProgressionData,
        //        "cf2604ea-f8b8-43b3-97fd-8f2812704370",
        //        skillName);
        //    rally.ViewElementDef = Helper.CreateDefFromClone(
        //        source.ViewElementDef,
        //        "30fdd27a-88d9-416d-ae6d-32991c2ba72a",
        //        skillName);
        //    TacStatusDef ignorePainStatusDef = Helper.CreateDefFromClone(
        //        Repo.GetAllDefs<TacStatusDef>().FirstOrDefault(p => p.name.Equals("IgnorePain_StatusDef")),
        //        "210a4f59-0b7f-4291-953d-7f7ab56c5041",
        //        "RallyIgnorePain_StatusDef");
        //    ignorePainStatusDef.DurationTurns = 1;
        //    ignorePainStatusDef.ShowNotification = true;
        //    ignorePainStatusDef.VisibleOnHealthbar = TacStatusDef.HealthBarVisibility.AlwaysVisible;
        //    ignorePainStatusDef.VisibleOnStatusScreen = TacStatusDef.StatusScreenVisibility.VisibleOnStatusesList;
        //    ignorePainStatusDef.Visuals = rally.ViewElementDef;
        //
        //
        //    rally.StatusDef = ignorePainStatusDef;
        //    
        //    rally.CharacterProgressionData.RequiredSpeed = 0;
        //    rally.CharacterProgressionData.RequiredStrength = 0;
        //    rally.CharacterProgressionData.RequiredWill = 0;
        //    rally.ViewElementDef.DisplayName1 = new LocalizedTextBind("RALLY", doNotLocalize);
        //    rally.ViewElementDef.Description = new LocalizedTextBind("Until next turn: Allies in 10 tile radius can use disabled limbs and gain panic immunity", doNotLocalize);
        //    Sprite rallyIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(ve => ve.name.Equals("E_ViewElement [Acheron_CallReinforcements_AbilityDef]")).LargeIcon;
        //    rally.ViewElementDef.LargeIcon = rallyIcon;
        //    rally.ViewElementDef.SmallIcon = rallyIcon;
        //    foreach (TacActorSimpleAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(aad => aad.name.Contains("Soldier_Utka_AnimActionsDef")))
        //    {
        //        if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(source))
        //        {
        //            animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(rally).ToArray();
        //            Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
        //            foreach (AbilityDef ad in animActionDef.AbilityDefs)
        //            {
        //                Logger.Debug("  " + ad.name);
        //            }
        //        }
        //    }
        //    //Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        //}
        private static void Create_PhantomProtocol()
        {
            float mod = 0.25f;
            string skillName = "BC_PhantomProtocol_AbilityDef";
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("QuickAim_AbilityDef"));
            ApplyStatusAbilityDef phantomProtocol = Helper.CreateDefFromClone(
                source,
                "5f3e257c-aff7-4296-9992-f6728bfa8af8",
                skillName);
            phantomProtocol.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "08545868-2bed-47a4-8628-371bbce5f718",
                skillName);
            phantomProtocol.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "c312e7f4-3339-4ee8-9717-d1f9c8bd2b32",
                skillName);
            phantomProtocol.StatusDef = Helper.CreateDefFromClone(
                Repo.GetAllDefs<StanceStatusDef>().FirstOrDefault(sms => sms.name.Equals("E_VanishedStatus [Vanish_AbilityDef]")),
                "06ca77ea-223b-4ec0-a7e6-734e6b7fefe9",
                "E_AccAnd StealthMultiplier [BC_PhantomProtocol_AbilityDef]");

            phantomProtocol.ViewElementDef.DisplayName1 = new LocalizedTextBind("PHANTOM PROTOCOL", doNotLocalize);
            phantomProtocol.ViewElementDef.Description = new LocalizedTextBind("You gain +25% accuracy and stealth until next turn", doNotLocalize);
            Sprite icon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_Thief.png");
            phantomProtocol.ViewElementDef.LargeIcon = icon;
            phantomProtocol.ViewElementDef.SmallIcon = icon;
            phantomProtocol.ActionPointCost = 0;
            phantomProtocol.WillPointCost = 3;

            StanceStatusDef ppModStatus = (StanceStatusDef)phantomProtocol.StatusDef;
            ppModStatus.Visuals = phantomProtocol.ViewElementDef;
            ppModStatus.EventOnApply = null;
            ppModStatus.EventOnUnapply = null;
            ppModStatus.StatModifications = new ItemStatModification[]
            {
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Accuracy,
                    Modification = StatModificationType.Add,
                    Value = mod
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Stealth,
                    Modification = StatModificationType.Add,
                    Value = mod
                }
            };
            ppModStatus.EquipmentsStatModifications = new EquipmentItemTagStatModification[0];
            ppModStatus.StanceShader = null;

            foreach (TacActorSimpleAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(aad => aad.name.Contains("Soldier_Utka_AnimActionsDef")))
            {
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(source) && !animActionDef.AbilityDefs.Contains(phantomProtocol))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(phantomProtocol).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                    Logger.Debug("----------------------------------------------------", false);
                }
            }
        }
        private static void Change_PainChameloen()
        {
            Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' no changes implemented yet!");
            Logger.Debug("----------------------------------------------------", false);
        }
        private static void Create_SowerOfChange()
        {
            string skillName = "SowerOfChange_AbilityDef";
            LocalizedTextBind name = new LocalizedTextBind("SOWER OF CHANGE", doNotLocalize);
            LocalizedTextBind description = new LocalizedTextBind("Returns 10% of damage as Viral to the attacker within 10 tiles", doNotLocalize);
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(oad => oad.name.Equals("Acheron_ContactCorruption_ApplyStatusAbilityDef"));
            ApplyStatusAbilityDef SowerOfChange = Helper.CreateDefFromClone(
                source,
                "40d9f907-a5a4-4f9a-bc12-e1a3f5459b3e",
                skillName);
            SowerOfChange.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "008272c9-2431-4681-a0a1-3bf61f3462bb",
                skillName);
            SowerOfChange.TargetingDataDef = Helper.CreateDefFromClone(
                source.TargetingDataDef,
                "1217a22e-0857-4094-a548-d224db6776a2",
                skillName);
            SowerOfChange.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "0441e1a3-47b5-4c31-9c33-5eb323f7e6a8",
                skillName);
            SowerOfChange.StatusDef = Helper.CreateDefFromClone(
                source.StatusDef,
                "1f5f7143-c6c3-440a-a7f5-0020f037d5cb",
                $"E_Status [{skillName}]");

            SowerOfChange.ViewElementDef.DisplayName1.LocalizationKey = "PR_BC_SOWER_OF_CHANGE";
            SowerOfChange.ViewElementDef.Description.LocalizationKey = "PR_BC_SOWER_OF_CHANGE_DESC";
            //SowerOfChange.AnimType = -1;

            AddStatusDamageKeywordDataDef RawVirausDamageKeyword = Helper.CreateDefFromClone(
                Shared.SharedDamageKeywords.ViralKeyword,
                "c03aa65b-9ca2-4665-9370-67fa81144cf3",
                $"RawViral_DamageKeywordDataDef");
            RawVirausDamageKeyword.ApplyOnlyOnHealthDamage = false;

            DamagePayloadEffectDef DamageEffect = Helper.CreateDefFromClone(
                Repo.GetAllDefs<DamagePayloadEffectDef>().FirstOrDefault(dpe => dpe.name.Equals("E_Element0 [SwarmerPoisonExplosion_Die_AbilityDef]")),
                "d9870608-797c-428a-8b56-17c1bdadbe27",
                $"E_DamagePayloadEffectDef {skillName}");
            DamageEffect.DamagePayload = Repo.GetAllDefs<ApplyDamageEffectAbilityDef>().FirstOrDefault(ade => ade.name.Equals("Mutoid_ViralExplode_AbilityDef")).DamagePayload;
            DamageEffect.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
            {
                //new DamageKeywordPair()
                //{
                //    DamageKeywordDef = Shared.SharedDamageKeywords.BlastKeyword,
                //    Value = 5
                //},
                //new DamageKeywordPair()
                //{
                //    DamageKeywordDef = Shared.SharedDamageKeywords.PiercingKeyword,
                //    Value = 100
                //},
                //new DamageKeywordPair()
                //{
                //    DamageKeywordDef = Shared.SharedDamageKeywords.ViralKeyword,
                //    Value = 1
                //}
                new DamageKeywordPair()
                {
                    DamageKeywordDef = RawVirausDamageKeyword,
                    Value = 1
                }
            };
            DamageEffect.DamagePayload.DamageType = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dt => dt.name.Equals("Virus_DamageOverTimeDamageTypeEffectDef"));
            DamageEffect.DamagePayload.DamageValue = 2333.0f;
            DamageEffect.DamagePayload.ArmourPiercing = 123.0f;
            DamageEffect.DamagePayload.Speed = 200.0f;
            DamageEffect.DamagePayload.BodyPartMultiplier = 0.0f;
            DamageEffect.DamagePayload.ObjectMultiplier = 0.0f;
            DamageEffect.DamagePayload.DamageDeliveryType = DamageDeliveryType.DirectLine;
            DamageEffect.DamagePayload.AoeRadius = 0.4f;
            DamageEffect.DamagePayload.ObjectToSpawnOnExplosion = null;
            DamageEffect.EffectPositionOffset = new Vector3(0, 0.2f, 0); // prevent to explode in the ground
            
            OnActorDamageReceivedStatusDef SocStatus = (OnActorDamageReceivedStatusDef)SowerOfChange.StatusDef;
            SocStatus.ApplicationConditions = new EffectConditionDef[0];
            SocStatus.DamageDeliveryTypeFilter = new List<DamageDeliveryType>();
            SocStatus.TargetApplicationConditions = new EffectConditionDef[]
            {
                Repo.GetAllDefs<EffectConditionDef>().FirstOrDefault(ec1 => ec1.name.Equals("NotOfPhoenixFaction_ApplicationCondition"))
            };

            SocStatus.EffectForAttacker = DamageEffect;
        }
        // Sower of Chage: Patching OnActorDamageReceivedStatus.OnActorDamageReceived() to handle the trigger effect preventing errors and to much slow motion
        [HarmonyPatch(typeof(OnActorDamageReceivedStatus), "OnActorDamageReceived")]
        internal static class SowerOfChange_OnActorDamageReceived_Patch
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static bool Prefix(OnActorDamageReceivedStatus __instance, DamageResult damageResult)
            {
                try
                {
                    object ___Source = AccessTools.Property(typeof(Status), "Source").GetValue(__instance, null);
                    TacticalActor ___TacticalActor = (TacticalActor)AccessTools.Property(typeof(TacStatus), "TacticalActor").GetValue(__instance, null);
                    TacticalAbility SowerOfChange = ___TacticalActor.GetAbilities<TacticalAbility>().FirstOrDefault(s => s.AbilityDef.name.Equals("SowerOfChange_AbilityDef"));
                    if (SowerOfChange != null)
                    {
                        Logger.Debug("----------------------------------------------------------------------------------------------------", false);
                        Logger.Debug($"OnActorDamageReceivedStatus.OnActorDamageReceived() called from '{SowerOfChange.AbilityDef.name}' ...");
                        Logger.Debug($"Actor: {___TacticalActor.DisplayName}");
                        Logger.Debug($"Recieved HealthDamage: {damageResult.HealthDamage}");
                        if (!(damageResult.Source is IDamageDealer damageDealer))
                        {
                            Logger.Debug($"damageResult.Source, type {damageResult.Source.GetType().Name}, is no IDamageDealer, exit without apply effect!");
                            Logger.Debug("----------------------------------------------------------------------------------------------------", false);
                            return false;
                        }
                        if (!__instance.OnActorDamageReceivedStatusDef.DamageDeliveryTypeFilter.IsEmpty()
                            && !__instance.OnActorDamageReceivedStatusDef.DamageDeliveryTypeFilter.Contains(damageDealer.GetDamagePayload().DamageDeliveryType))
                        {
                            Logger.Debug($"DamageDeliveryType {damageDealer.GetDamagePayload().DamageDeliveryType} does not fit preset, exit without apply effect!");
                            Logger.Debug("----------------------------------------------------------------------------------------------------", false);
                            return false;
                        }
                        TacticalActorBase tacticalActorBase = damageDealer.GetTacticalActorBase();
                        Logger.Debug($"TacticalActorBase of target: {tacticalActorBase.DisplayName}");
                        EffectConditionDef[] targetApplicationConditions = __instance.OnActorDamageReceivedStatusDef.TargetApplicationConditions;
                        for (int i = 0; i < targetApplicationConditions.Length; i++)
                        {
                            if (!targetApplicationConditions[i].ConditionMet(tacticalActorBase))
                            {
                                Logger.Debug($"OnActorDamageReceivedStatusDef.TargetApplicationConditions not met, exit without apply effect!");
                                Logger.Debug("----------------------------------------------------------------------------------------------------", false);
                                return false;
                            }
                        }
                        EffectTarget actorEffectTarget = TacUtil.GetActorEffectTarget(tacticalActorBase, null);
                        //GameObject effectTargetObject = actorEffectTarget.GameObject;
                        DamagePayloadEffectDef effectDef = (DamagePayloadEffectDef)__instance.OnActorDamageReceivedStatusDef.EffectForAttacker;
                        float viralDamage = 1;
                        //float blastDamage = 0;
                        float timingScale = 0.8f;
                        //blastDamage = effectDef.DamagePayload.DamageKeywords.Find(dk => dk.DamageKeywordDef == Shared.SharedDamageKeywords.BlastKeyword).Value;
                        viralDamage = damageResult.HealthDamage >= 10 ? damageResult.HealthDamage / 10 : 1.0f;
                        AddStatusDamageKeywordDataDef RawVirausDamageKeyword = Repo.GetAllDefs<AddStatusDamageKeywordDataDef>().FirstOrDefault(asd => asd.name.Equals("RawViral_DamageKeywordDataDef"));
                        effectDef.DamagePayload.DamageKeywords.Find(dk => dk.DamageKeywordDef == RawVirausDamageKeyword).Value = viralDamage;
                        //effectDef.DamagePayload.DamageKeywords.Find(dk => dk.DamageKeywordDef == Shared.SharedDamageKeywords.ViralKeyword).Value = viralDamage;
                        ___TacticalActor.Timing.Scale = timingScale;
                        tacticalActorBase.Timing.Scale = timingScale;
                        Logger.Debug($"'{___TacticalActor}' applies {viralDamage} viral damage on '{actorEffectTarget}', position '{actorEffectTarget.Position + effectDef.EffectPositionOffset}'");
                        //Logger.Always($"'{___TacticalActor}' applies {blastDamage} blast and {viralDamage} viral damage on '{effectTargetObject}', position '{actorEffectTarget.Position + effectDef.EffectPositionOffset}'");
                        Effect.Apply(__instance.Repo, effectDef, actorEffectTarget, ___TacticalActor);
                        ___TacticalActor.Timing.Scale = timingScale;
                        tacticalActorBase.Timing.Scale = timingScale;
                        Logger.Debug($"Effect applied on {tacticalActorBase}");
                        Logger.Debug("----------------------------------------------------------------------------------------------------", false);
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return false;
                }
            }
        }
        private static void Change_BreatheMist()
        {
            // Breathe Mist adding progression def
            ApplyEffectAbilityDef mistBreather = Repo.GetAllDefs<ApplyEffectAbilityDef>().FirstOrDefault(a => a.name.Equals("MistBreather_AbilityDef"));
            AbilityCharacterProgressionDef mbProgressionDef = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "9eaf8809-01d9-4582-89e0-78c8596f5e7d",
                "MistBreather_AbilityDef");
            mbProgressionDef.RequiredStrength = 0;
            mbProgressionDef.RequiredWill = 0;
            mbProgressionDef.RequiredSpeed = 0;
            mistBreather.CharacterProgressionData = mbProgressionDef;
        }
        private static void Change_Resurrect()
        {
            ResurrectAbilityDef resurrect = Repo.GetAllDefs<ResurrectAbilityDef>().FirstOrDefault(a => a.name.Equals("Mutoid_ResurrectAbilityDef"));
            resurrect.ActionPointCost = 0.75f;
            resurrect.WillPointCost = 10;
            resurrect.UsesPerTurn = 1;
        }
        private static void Change_PepperCloud()
        {
            float pcRange = 8.0f;
            ApplyStatusAbilityDef pepperCloud = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("Mutoid_PepperCloud_ApplyStatusAbilityDef"));
            pepperCloud.TargetingDataDef.Origin.Range = pcRange;
            pepperCloud.ViewElementDef.Description = new LocalizedTextBind($"Reduces Accuracy by 50% of all organic enemies within {pcRange} tiles for 1 turn.", doNotLocalize);
        }
        private static void Create_AR_Targeting()
        {
            string skillName = "BC_ARTargeting_AbilityDef";
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("DeterminedAdvance_AbilityDef"));
            ApplyStatusAbilityDef arTargeting = Helper.CreateDefFromClone(
                source,
                "ad95d7cb-b172-4e0d-acc5-e7e514fcb824",
                skillName);
            arTargeting.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "143a743b-e42e-4f65-9f83-f76bf42c733b",
                skillName);
            arTargeting.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "7019bd7f-d30d-4ce8-9c3d-0b6161bd4ee0",
                skillName);
            StanceStatusDef artStatus = Helper.CreateDefFromClone(
                Repo.GetAllDefs<StanceStatusDef>().FirstOrDefault(p => p.name.Equals("StomperLegs_StabilityStance_StatusDef")),
                "56b4ea0e-d0cc-4fc9-b6cf-26e45b2dc81c",
                "ARTargeting_Stance_StatusDef");
            artStatus.DurationTurns = 0;
            artStatus.SingleInstance = true;
            artStatus.Visuals = arTargeting.ViewElementDef;

            arTargeting.StatusDef = artStatus;

            arTargeting.WillPointCost = 2;
            arTargeting.UsesPerTurn = 8;

            arTargeting.CharacterProgressionData.RequiredSpeed = 0;
            arTargeting.CharacterProgressionData.RequiredStrength = 0;
            arTargeting.CharacterProgressionData.RequiredWill = 0;
            arTargeting.ViewElementDef.DisplayName1 = new LocalizedTextBind("AR TARGETING", doNotLocalize);
            arTargeting.ViewElementDef.Description = new LocalizedTextBind("Target ally gains +20% accuracy", doNotLocalize);
            Sprite artIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(ve => ve.name.Equals("E_ViewElement [EagleEye_AbilityDef]")).LargeIcon;
            arTargeting.ViewElementDef.LargeIcon = artIcon;
            arTargeting.ViewElementDef.SmallIcon = artIcon;

            foreach (TacActorSimpleAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(aad => aad.name.Contains("Soldier_Utka_AnimActionsDef")))
            {
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(source) && !animActionDef.AbilityDefs.Contains(arTargeting))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(arTargeting).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                    Logger.Debug("----------------------------------------------------", false);
                }
            }
        }
        private static void Create_Endurance()
        {
            // Harmony patch RecoverWillAbility.GetWillpowerRecover
            // Adding an ability that get checked in the patched method (see below)
            string skillName = "Endurance_AbilityDef";
            PassiveModifierAbilityDef source = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("Talent"));
            PassiveModifierAbilityDef endurance = Helper.CreateDefFromClone(
                source,
                "4e9712b6-8a46-489d-9553-fdc1380c334a",
                skillName);
            endurance.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "ffc75f46-adf0-4683-b28c-a59e91a99843",
                skillName);
            endurance.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "75155fd6-7cef-40d8-a03d-28bdb3dc0929",
                skillName);
            // reset all possible passive modifications, we need none, this ability is only to have something to chose and as flag for the Endurance Harmony patch
            endurance.StatModifications = new ItemStatModification[0];
            endurance.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
            endurance.DamageKeywordPairs = new DamageKeywordPair[0];
            // Set necessary fields
            endurance.CharacterProgressionData.RequiredSpeed = 0;
            endurance.CharacterProgressionData.RequiredStrength = 0;
            endurance.CharacterProgressionData.RequiredWill = 0;
            endurance.ViewElementDef.DisplayName1 = new LocalizedTextBind("ENDURANCE", doNotLocalize);
            endurance.ViewElementDef.Description = new LocalizedTextBind("Recover restores 75% WP", doNotLocalize);
            Sprite enduranceIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(ve => ve.name.Equals("E_ViewElement [Reckless_AbilityDef]")).LargeIcon;
            endurance.ViewElementDef.LargeIcon = enduranceIcon;
            endurance.ViewElementDef.SmallIcon = enduranceIcon;
        }

        // Endurance: Patching GetWillpowerRecover from active actor when he uses Recover to check if Endurance ability is active and return 75% WP to recover
        [HarmonyPatch(typeof(RecoverWillAbility), "GetWillpowerRecover")]
        internal static class RecoverWillAbility_GetWillpowerRecover
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static void Postfix(ref float __result, RecoverWillAbility __instance)
            {
                TacticalActor ___TacticalActor = (TacticalActor)AccessTools.Property(typeof(TacticalAbility), "TacticalActor").GetValue(__instance, null);
                TacticalAbility endurance = ___TacticalActor.GetAbilities<TacticalAbility>().FirstOrDefault(s => s.AbilityDef.name.Equals("Endurance_AbilityDef"));
                if (endurance != null)
                {
                    __result = Mathf.Ceil(___TacticalActor.CharacterStats.WillPoints.Max * 75 / 100f);
                }
                else
                {
                    __result = Mathf.Ceil(___TacticalActor.CharacterStats.WillPoints.Max * __instance.RecoverWillAbilityDef.WillPointsReturnedPerc / 100f);
                }
            }
        }
    }
}
