using Base.Core;
using Base.Defs;
using Base.Entities.Effects.ApplicationConditions;
using Base.Entities.Statuses;
using Base.UI;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Effects.ApplicationConditions;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    internal class InfiltratorSkills
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            // Spider Drone Pack: 3 AP 3 WP, rest vanilla
            Change_DeployDronePack();

            // Sneak Attack: Direct fire and melee +60 damage while not spotted
            Change_SneakAttack();

            // Homing Drone:	2 AP 0WP, Shoot your Spider Drone at the target. Next time the target takes damage the drone will explode causing area damage. SCRAPPED FOR NOW
            //Create_HomingDrone();

            // Cautious: +10% stealth
            Change_Cautious();
        }

        private static void Change_DeployDronePack()
        {
            float apCost = 0.75f;
            float wpCost = 3.0f;

            ShootAbilityDef DeployDronePack = Repo.GetAllDefs<ShootAbilityDef>().FirstOrDefault(s => s.name.Equals("DeployDronePack_ShootAbilityDef"));
            DeployDronePack.ActionPointCost = apCost;
            DeployDronePack.WillPointCost = wpCost;
        }

        private static void Change_SneakAttack()
        {
            float hiddenDamageMod = 1.5f;
            //float hiddenCrossbowDamageMod = 2f;
            float locatedDamageMod = 1.25f;
            //float locatedCrossbowDamageMod = 1.5f;

            ApplyStatusAbilityDef SneakAttack = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("SneakAttack_AbilityDef"));
            FactionVisibilityConditionStatusDef factionVisibilityConditionStatus = (FactionVisibilityConditionStatusDef)SneakAttack.StatusDef;
            StanceStatusDef hiddenStateStatus = (StanceStatusDef)factionVisibilityConditionStatus.HiddenStateStatusDef;
            hiddenStateStatus.Visuals = Helper.CreateDefFromClone(
                SneakAttack.ViewElementDef,
                "8981e175-124a-48eb-8644-69f4ae77d454",
                $"E_HiddenStatusViewElement [{SneakAttack.name}]");
            StanceStatusDef locatedStateStatus = Helper.CreateDefFromClone(
                hiddenStateStatus,
                "8cc0375e-1f2d-4e4a-85df-d626dab2a92a",
                $"E_DetectedSneakAttackStatus [{SneakAttack.name}]");
            locatedStateStatus.Visuals = Helper.CreateDefFromClone(
                SneakAttack.ViewElementDef,
                "63353d9f-97c3-46fb-b40b-b9eb9d32f1ce",
                $"E_DetectedStatusViewElement [{SneakAttack.name}]");

            // Setting fields
            //string description = string.Concat(
            //    $"Attacks while hidden deal +{(hiddenDamageMod - 1) * 100}% and while located +{(locatedDamageMod - 1) * 100}% damage.\n",
            //    $"Crossbows gain +{(hiddenCrossbowDamageMod - 1) * 100}% and +{(locatedCrossbowDamageMod - 1) * 100}% respectively.");
            SneakAttack.ViewElementDef.Description = new LocalizedTextBind($"Attacks while hidden deal +{(hiddenDamageMod - 1) * 100}% and while located +{(locatedDamageMod - 1) * 100}% damage.", doNotLocalize);

            hiddenStateStatus.Visuals.Description = new LocalizedTextBind($"HIDDEN: Deal +{(hiddenDamageMod - 1) * 100}% damage.", doNotLocalize);
            hiddenStateStatus.Visuals.Color = Color.green;
            //hiddenStateStatus.EquipmentsStatModifications = new EquipmentItemTagStatModification[]
            //{
            //    new EquipmentItemTagStatModification()
            //    {
            //        ItemTag = Shared.SharedGameTags.SilentTags.FirstOrDefault(st => st.name.Contains("Crossbow")),
            //        EquipmentStatModification = new ItemStatModification()
            //        {
            //            TargetStat = StatModificationTarget.BonusAttackDamage,
            //            Modification = StatModificationType.Multiply,
            //            Value = hiddenCrossbowDamageMod - (hiddenDamageMod - 1f)
            //        }
            //    }
            //};
            hiddenStateStatus.StatModifications[0].Value = hiddenDamageMod;

            locatedStateStatus.Visuals.Description = new LocalizedTextBind($"LOCATED: Deal +{(locatedDamageMod - 1) * 100}% damage.", doNotLocalize);
            locatedStateStatus.Visuals.Color = Color.yellow;
            //locatedStateStatus.EquipmentsStatModifications = new EquipmentItemTagStatModification[]
            //{
            //    new EquipmentItemTagStatModification()
            //    {
            //        ItemTag = Shared.SharedGameTags.SilentTags.FirstOrDefault(st => st.name.Contains("Crossbow")),
            //        EquipmentStatModification = new ItemStatModification()
            //        {
            //            TargetStat = StatModificationTarget.BonusAttackDamage,
            //            Modification = StatModificationType.Multiply,
            //            Value = locatedCrossbowDamageMod - (locatedDamageMod - 1f)
            //        }
            //    }
            //};
            locatedStateStatus.StatModifications[0].Value = locatedDamageMod;

            factionVisibilityConditionStatus.LocatedStateStatusDef = locatedStateStatus;

            //AddAttackBoostStatusDef aBStatus = Helper.CreateDefFromClone(
            //    Repo.GetAllDefs<AddAttackBoostStatusDef>().FirstOrDefault(a => a.name.Equals("E_Status [ArmourBreak_AbilityDef]")),
            //    "",
            //    "E_BC_AddAttackBoostStatus [SneakAttack_AbilityDef]");
            //aBStatus.ApplicationConditions = new EffectConditionDef[]
            //{
            //    new ActorHasTagEffectConditionDef()
            //    {
            //        GameTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("GunWeapon_TagDef"))
            //    },
            //    new ActorHasTagEffectConditionDef()
            //    {
            //        GameTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("MeleeWeapon_TagDef"))
            //    }
            //};
            //aBStatus.DurationTurns = -1;
            //aBStatus.Visuals = SneakAttack.ViewElementDef;
            //aBStatus.NumberOfAttacks = -1;
            //aBStatus.WeaponTagFilter = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("GunWeapon_TagDef"));
            //DamageKeywordDef SneakAttack_DamageKeyword = Helper.CreateDefFromClone(
            //    Repo.GetAllDefs<DamageKeywordDef>().FirstOrDefault(dkd => dkd.name.Equals("DistributedShredding_DamageKeywordDataDef")),
            //    "",
            //    "SneakAttack_DamageKeywordDataDef");
            //SneakAttack_DamageKeyword.DamageTypeDef = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtb => dtb.name.Equals("Blast_StandardDamageTypeEffectDef"));
            //SneakAttack_DamageKeyword.DamageTypeDef.HasCameraShake = false;
            //SneakAttack_DamageKeyword.AppliesStandardDamage = true;
            //SneakAttack_DamageKeyword.SoloEffector = true;
            //SneakAttack_DamageKeyword.Visuals = Repo.GetAllDefs<ViewElementDef>().FirstOrDefault(ve => ve.name.Equals("E_Visuals [Blast_DamageKeywordDataDef]"));
            //aBStatus.DamageKeywordPairs = new DamageKeywordPair[]
            //{
            //    new DamageKeywordPair()
            //    {
            //        DamageKeywordDef = SneakAttack_DamageKeyword,
            //        Value = damageMod
            //    }
            //};
            //aBStatus.AdditionalStatusesToApply = new TacStatusDef[0];
            //(SneakAttack.StatusDef as FactionVisibilityConditionStatusDef).HiddenStateStatusDef = aBStatus;
            //(SneakAttack.StatusDef as FactionVisibilityConditionStatusDef).LocatedStateStatusDef = aBStatus;
            //SneakAttack.ViewElementDef.Description = new LocalizedTextBind($"Direct Fire and Melee attacks while not spotted deal {damageMod} additional damage", doNotLocalize);
        }
        // Sneak Attack: Patching ProcessKeywordDataInternal of ShreddingDamageKeywordData to adapt damage distribution on all projectiles, original only for shred
        //[HarmonyPatch(typeof(ShreddingDamageKeywordData), "ProcessKeywordDataInternal")]
        //internal static class SA_ShreddingDamageKeywordData_ProcessKeywordDataInternal_patch
        //{
        //    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        //    private static bool Prefix(ShreddingDamageKeywordData __instance, ref bool __result, ref DamageAccumulation.TargetData data)
        //    {
        //        try
        //        {
        //            if (__instance.DamageKeywordDef.name.Equals("SneakAttack_DamageKeywordDataDef"))
        //            {
        //                MethodInfo method_GenerateTargetData = AccessTools.Method(typeof(ShreddingDamageKeywordData), "GenerateTargetData"); ;
        //                MethodInfo method_CalculateDamageValue = AccessTools.Method(typeof(ShreddingDamageKeywordData), "CalculateDamageValue"); ;
        //                if (data == null)
        //                {
        //                    data = (DamageAccumulation.TargetData)method_GenerateTargetData.Invoke(__instance, null);
        //                }
        //                data.DamageResult.HealthDamage = (float)method_CalculateDamageValue.Invoke(__instance, null);
        //                __result = true;
        //                return false;
        //            }
        //            return true;
        //        }
        //        catch (Exception e)
        //        {
        //            Logger.Error(e);
        //            return true;
        //        }
        //    }
        //}


        //private static void Create_HomingDrone()
        //{
        //    Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        //    Logger.Debug("----------------------------------------------------", false);
        //}

        private static void Change_Cautious()
        {
            //float damageMod = 0.9f;
            float accuracyMod = 0.2f;
            float stealthMod = 0.1f;
            PassiveModifierAbilityDef cautious = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(asa => asa.name.Equals("Cautious_AbilityDef"));
            cautious.StatModifications = new ItemStatModification[]
            {
                //new ItemStatModification()
                //{
                //   TargetStat = StatModificationTarget.BonusAttackDamage,
                //   Modification = StatModificationType.Multiply,
                //   Value = damageMod
                //},
                new ItemStatModification()
                {
                   TargetStat = StatModificationTarget.Accuracy,
                   Modification = StatModificationType.Add,
                   Value = accuracyMod
                },
                new ItemStatModification()
                {
                   TargetStat = StatModificationTarget.Stealth,
                   Modification = StatModificationType.Add,
                   Value = stealthMod
                }
            };
            cautious.ViewElementDef.Description = new LocalizedTextBind($"Gain {accuracyMod * 100}% accuracy and {stealthMod * 100}% stealth.", doNotLocalize);
        }
    }
}
