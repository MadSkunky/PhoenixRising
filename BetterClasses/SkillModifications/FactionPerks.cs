using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Statuses;
using Base.UI;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
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
            Change_Rally();
            //Phantom Protocol: 0AP 3WP, You gain +25% accuracy and stealth until next turn
            Create_PhantomProtocol();
            //Pain Chameleon:  Maybe no change, to check if one of the ..._PainChameleon_AbilityDef will work
            Change_PainChameloen();
            //Putrid Flesh: Passive, Returns 10% of damage as Viral to the attacker within 10 tiles
            Create_PutridFlesh();
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
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' no changes implemented yet!");
        }
        private static void Change_Shadowstep()
        {
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' no changes implemented yet!");
        }
        private static void Change_Rally()
        {
            string skillName = "BC_Rally_AbilityDef";
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("Rally_AbilityDef"));
            ApplyStatusAbilityDef rally = Helper.CreateDefFromClone(
                source,
                "edea324b-e435-416f-bb93-8e1ea16d2e64",
                skillName);
            rally.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "cf2604ea-f8b8-43b3-97fd-8f2812704370",
                skillName);
            rally.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "30fdd27a-88d9-416d-ae6d-32991c2ba72a",
                skillName);
            TacStatusDef ignorePainStatusDef = Helper.CreateDefFromClone(
                Repo.GetAllDefs<TacStatusDef>().FirstOrDefault(p => p.name.Equals("IgnorePain_StatusDef")),
                "210a4f59-0b7f-4291-953d-7f7ab56c5041",
                "RallyIgnorePain_StatusDef");
            ignorePainStatusDef.DurationTurns = 1;
            ignorePainStatusDef.ShowNotification = true;
            ignorePainStatusDef.VisibleOnHealthbar = TacStatusDef.HealthBarVisibility.AlwaysVisible;
            ignorePainStatusDef.VisibleOnStatusScreen = TacStatusDef.StatusScreenVisibility.VisibleOnStatusesList;
            ignorePainStatusDef.Visuals = rally.ViewElementDef;


            rally.StatusDef = ignorePainStatusDef;
            
            rally.CharacterProgressionData.RequiredSpeed = 0;
            rally.CharacterProgressionData.RequiredStrength = 0;
            rally.CharacterProgressionData.RequiredWill = 0;
            rally.ViewElementDef.DisplayName1 = new LocalizedTextBind("RALLY", doNotLocalize);
            rally.ViewElementDef.Description = new LocalizedTextBind("Until next turn: Allies in 10 tile radius can use disabled limbs and gain panic immunity", doNotLocalize);
            Sprite rallyIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(ve => ve.name.Equals("E_ViewElement [Acheron_CallReinforcements_AbilityDef]")).LargeIcon;
            rally.ViewElementDef.LargeIcon = rallyIcon;
            rally.ViewElementDef.SmallIcon = rallyIcon;
            foreach (TacActorSimpleAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(aad => aad.name.Contains("Soldier_Utka_AnimActionsDef")))
            {
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(source))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(rally).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
                }
            }
            //Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }
        private static void Create_PhantomProtocol()
        {
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

            StatMultiplierStatusDef pPAM = Helper.CreateDefFromClone(
                Repo.GetAllDefs<StatMultiplierStatusDef>().FirstOrDefault(sms => sms.name.Equals("Trembling_StatusDef")),
                "06ca77ea-223b-4ec0-a7e6-734e6b7fefe9",
                "E AccuracyMultiplier [QuickAim_AbilityDef]");

            pPAM.StatsMultipliers = new StatMultiplier[]
            {
                new StatMultiplier()
                {
                    StatName = "Accuracy",
                    Multiplier = 1.25f
                },
                new StatMultiplier()
                {
                    StatName = "Stealth",
                    Multiplier = 1.25f
                },
            };

            phantomProtocol.ActionPointCost = 0;
            phantomProtocol.WillPointCost = 3;

            AddAttackBoostStatusDef phantomProtocolStatus = (AddAttackBoostStatusDef)phantomProtocol.StatusDef;
            phantomProtocolStatus.AdditionalStatusesToApply = phantomProtocolStatus.AdditionalStatusesToApply.Append(pPAM).ToArray();

            phantomProtocol.CharacterProgressionData.RequiredSpeed = 0;
            phantomProtocol.CharacterProgressionData.RequiredStrength = 0;
            phantomProtocol.CharacterProgressionData.RequiredWill = 0;
            phantomProtocol.ViewElementDef.DisplayName1 = new LocalizedTextBind("PHANTOM PROTOCOL", doNotLocalize);
            phantomProtocol.ViewElementDef.Description = new LocalizedTextBind("You gain +25% accuracy and stealth until next turn", doNotLocalize);
        }
        private static void Change_PainChameloen()
        {
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' no changes implemented yet!");
        }
        private static void Create_PutridFlesh()
        {
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
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
            resurrect.WillPointCost = 6;
            resurrect.UsesPerTurn = 1;
        }
        private static void Change_PepperCloud()
        {
            ApplyStatusAbilityDef pepperCloud = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("Mutoid_PepperCloud_ApplyStatusAbilityDef"));
            pepperCloud.TargetingDataDef.Origin.Range = 8;
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
                if (animActionDef.AbilityDefs != null && animActionDef.AbilityDefs.Contains(source))
                {
                    animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(arTargeting).ToArray();
                    Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                    foreach (AbilityDef ad in animActionDef.AbilityDefs)
                    {
                        Logger.Debug("  " + ad.name);
                    }
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
