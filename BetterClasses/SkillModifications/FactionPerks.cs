using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.UI;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
using PhoenixPoint.Tactical.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
            //Rally: 1AP 4WP, 'Until next turn: Allies in 10 tile radius can use disabled limbs and gain panic immunity', icon to LargeIcon from 'E_View [Acheron_CallReinforcements_AbilityDef]'
            Change_Rally();
            //Shadowstep: No changes
            Change_Shadowstep();
            //Cure Spray: Maybe no change, to check if Mutoid_CureSpray_AbilityDef will work
            Change_CureSpray();
            //Pain Chameleon:  Maybe no change, to check if one of the ..._PainChameleon_AbilityDef will work
            Change_PainChameloen();
            //Sonic Blast: 1AP 2WP, to check if the Mutoid_Adapt_Head_Sonic_AbilityDef will work
            Change_SonicBlast();
            //Breathe Mist: Adding progression def, READY
            Change_BreatheMist();
            //Resurrect: 3AP 6WP, to check if the Mutoid_ResurrectAbilityDef will work, change to only allow 1 ressurect at one time (same as MC)
            Change_Resurrect();
            //Pepper Cloud: 3AP 6WP, to check if the Mutoid_PepperCloud_ApplyStatusAbilityDef will work, change range from 5 to 8 tiles
            Change_PepperCloud();
            //Paralyse Limb: 3AP 6WP, to check if the Mutoid_ParalyticSpray_AbilityDef will work
            Change_ParalyseLimb();
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
        private static void Change_Shadowstep()
        {
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' no changes implemented yet!");
        }
        private static void Change_Rally()
        {

            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }
        private static void Change_CureSpray()
        {
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }
        private static void Change_PainChameloen()
        {
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }
        private static void Change_SonicBlast()
        {

            //Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
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
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }
        private static void Change_PepperCloud()
        {
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }
        private static void Change_ParalyseLimb()
        {
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }
        private static void Create_Endurance()
        {
            string skillName = "Endurance_AbilityDef";
            RecoverWillAbilityDef source = Repo.GetAllDefs<RecoverWillAbilityDef>().FirstOrDefault(p => p.name.Equals("RecoverWill_AbilityDef"));
            RecoverWillAbilityDef endurance = Helper.CreateDefFromClone(
                source,
                "4e9712b6-8a46-489d-9553-fdc1380c334a",
                skillName);
            endurance.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "ffc75f46-adf0-4683-b28c-a59e91a99843",
                skillName);
            endurance.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "75155fd6-7cef-40d8-a03d-28bdb3dc0929",
                skillName);

            endurance.WillPointsReturnedPerc = 75;

            endurance.CharacterProgressionData.RequiredSpeed = 0;
            endurance.CharacterProgressionData.RequiredStrength = 0;
            endurance.CharacterProgressionData.RequiredWill = 0;
            endurance.ViewElementDef.DisplayName1 = new LocalizedTextBind("ENDURANCE", doNotLocalize);
            endurance.ViewElementDef.Description = new LocalizedTextBind("Recover restores 75% WP", doNotLocalize);
            Sprite enduranceIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_Stamina.png");
            endurance.ViewElementDef.LargeIcon = enduranceIcon;
        }

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
