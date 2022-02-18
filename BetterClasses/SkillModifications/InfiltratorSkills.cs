using Base.Core;
using Base.Defs;
using Base.Entities.Effects.ApplicationConditions;
using Base.Entities.Statuses;
using Base.UI;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Effects.ApplicationConditions;
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

            // Master Archer: Shoot your Crossbow 3 times at one target, reveal your position
            Create_MasterArcher();

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
            aBStatus.ApplicationConditions = new EffectConditionDef[]
            {
                new ActorHasTagEffectConditionDef()
                {
                    GameTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("GunWeapon_TagDef"))
                },
                new ActorHasTagEffectConditionDef()
                {
                    GameTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("MeleeWeapon_TagDef"))
                }
            };
            aBStatus.Visuals = sA.ViewElementDef;
            aBStatus.DamageKeywordPairs = new DamageKeywordPair[]
            {
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.DamageKeyword,
                    Value = damageMod
                }
            };
            (sA.StatusDef as FactionVisibilityConditionStatusDef).HiddenStateStatusDef = aBStatus;
            (sA.StatusDef as FactionVisibilityConditionStatusDef).LocatedStateStatusDef = aBStatus;
            sA.ViewElementDef.Description = new LocalizedTextBind($"Direct Fire and Melee attacks while not spotted deal {damageMod} additional damage", doNotLocalize);
        }

        private static void Create_MasterArcher()
        {
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }

        private static void Change_Cautious()
        {
            PassiveModifierAbilityDef cautious = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(asa => asa.name.Equals("Cautious_AbilityDef"));
            cautious.StatModifications = new ItemStatModification[]
            {
                new ItemStatModification()
                {
                   TargetStat = StatModificationTarget.BonusAttackDamage,
                   Modification = StatModificationType.Multiply,
                   Value = 0.9f
                },
                new ItemStatModification()
                {
                   TargetStat = StatModificationTarget.Accuracy,
                   Modification = StatModificationType.Add,
                   Value = 0.1f
                },
                new ItemStatModification()
                {
                   TargetStat = StatModificationTarget.Stealth,
                   Modification = StatModificationType.Add,
                   Value = 0.1f
                }
            };
        }
    }
}
