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

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class InfiltratorSkills
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            // Sneak Attack: Direct fire and melee +60 damage while not spotted
            Change_SneakAttack();

            // Homing Drone:	2 AP 0WP, Shoot your Spider Drone at the target. Next time the target takes damage the drone will explode causing area damage.
            Create_HomingDrone();

            // Cautious: +10% stealth
            Change_Cautious();
        }

        private static void Change_SneakAttack()
        {
            float damageMod = 60f;

            ApplyStatusAbilityDef sA = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("SneakAttack_AbilityDef"));
            AddAttackBoostStatusDef aBStatus = Helper.CreateDefFromClone(
                Repo.GetAllDefs<AddAttackBoostStatusDef>().FirstOrDefault(a => a.name.Equals("E_Status [ArmourBreak_AbilityDef]")),
                "8cc0375e-1f2d-4e4a-85df-d626dab2a92a",
                "E_BC_AddAttackBoostStatus [SneakAttack_AbilityDef]");
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
            aBStatus.DurationTurns = -1;
            aBStatus.Visuals = sA.ViewElementDef;
            aBStatus.NumberOfAttacks = -1;
            aBStatus.WeaponTagFilter = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("GunWeapon_TagDef"));
            DamageKeywordDef SneakAttack_DamageKeyword = Helper.CreateDefFromClone(
                Repo.GetAllDefs<DamageKeywordDef>().FirstOrDefault(dkd => dkd.name.Equals("DistributedShredding_DamageKeywordDataDef")),
                "63353d9f-97c3-46fb-b40b-b9eb9d32f1ce",
                "SneakAttack_DamageKeywordDataDef");
            SneakAttack_DamageKeyword.DamageTypeDef = Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtb => dtb.name.Equals("Blast_StandardDamageTypeEffectDef"));
            SneakAttack_DamageKeyword.DamageTypeDef.HasCameraShake = false;
            SneakAttack_DamageKeyword.AppliesStandardDamage = true;
            SneakAttack_DamageKeyword.SoloEffector = true;
            SneakAttack_DamageKeyword.Visuals = Repo.GetAllDefs<ViewElementDef>().FirstOrDefault(ve => ve.name.Equals("E_Visuals [Blast_DamageKeywordDataDef]"));
            aBStatus.DamageKeywordPairs = new DamageKeywordPair[]
            {
                new DamageKeywordPair()
                {
                    DamageKeywordDef = SneakAttack_DamageKeyword,
                    Value = damageMod
                }
            };
            aBStatus.AdditionalStatusesToApply = new TacStatusDef[0];
            (sA.StatusDef as FactionVisibilityConditionStatusDef).HiddenStateStatusDef = aBStatus;
            (sA.StatusDef as FactionVisibilityConditionStatusDef).LocatedStateStatusDef = aBStatus;
            sA.ViewElementDef.Description = new LocalizedTextBind($"Direct Fire and Melee attacks while not spotted deal {damageMod} additional damage", doNotLocalize);
        }
        // Sneak Attack: Patching ProcessKeywordDataInternal of ShreddingDamageKeywordData to adapt damage distribution on all projectiles, original only for shred
        [HarmonyPatch(typeof(ShreddingDamageKeywordData), "ProcessKeywordDataInternal")]
        internal static class SA_ShreddingDamageKeywordData_ProcessKeywordDataInternal_patch
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static bool Prefix(ShreddingDamageKeywordData __instance, ref bool __result, ref DamageAccumulation.TargetData data)
            {
                try
                {
                    if (__instance.DamageKeywordDef.name.Equals("SneakAttack_DamageKeywordDataDef"))
                    {
                        MethodInfo method_GenerateTargetData = AccessTools.Method(typeof(ShreddingDamageKeywordData), "GenerateTargetData"); ;
                        MethodInfo method_CalculateDamageValue = AccessTools.Method(typeof(ShreddingDamageKeywordData), "CalculateDamageValue"); ;
                        if (data == null)
                        {
                            data = (DamageAccumulation.TargetData)method_GenerateTargetData.Invoke(__instance, null);
                        }
                        data.DamageResult.HealthDamage = (float)method_CalculateDamageValue.Invoke(__instance, null);
                        __result = true;
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return true;
                }
            }
        }


        private static void Create_HomingDrone()
        {
            Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }

        private static void Change_Cautious()
        {
            float damageMod = 0.9f;
            float accuracyMod = 0.2f;
            float stealthMod = 0.1f;
            PassiveModifierAbilityDef cautious = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(asa => asa.name.Equals("Cautious_AbilityDef"));
            cautious.StatModifications = new ItemStatModification[]
            {
                new ItemStatModification()
                {
                   TargetStat = StatModificationTarget.BonusAttackDamage,
                   Modification = StatModificationType.Multiply,
                   Value = damageMod
                },
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
            cautious.ViewElementDef.Description = new LocalizedTextBind($"Gain {accuracyMod * 100}% accuracy and {stealthMod * 100}% stealth but {(damageMod * 100) - 100}% damage", doNotLocalize);
        }
    }
}
