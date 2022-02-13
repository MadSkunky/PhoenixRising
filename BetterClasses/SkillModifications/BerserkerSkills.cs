using Base.Core;
using Base.Defs;
using Base.Entities.Effects.ApplicationConditions;
using Base.Entities.Statuses;
using Base.UI;
using com.ootii.Collections;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Tactical.Entities.Abilities;
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
            // cancelled, delayed
            //RepositionAbilityDef dash = Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef"));
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' no changes implemented yet!");
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
            ignorePain.ViewElementDef.Description = new LocalizedTextBind("Disabled body parts remain functional and cannot panic.", doNotLocalize);
        }

        private static void Change_AdrenalineRush()
        {
            ApplyStatusAbilityDef adrenalineRush = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("AdrenalineRush_AbilityDef"));
            adrenalineRush.StatusDef = Repo.GetAllDefs<ChangeAbilitiesCostStatusDef>().FirstOrDefault(sd => sd.name.Equals("E_SetAbilitiesTo1AP [AdrenalineRush_AbilityDef]"));
            adrenalineRush.ViewElementDef.Description = new LocalizedTextBind("Until end of turn one-handed weapon and all non-weapon skills cost 1AP, except recover.", doNotLocalize);
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
            Logger.Always("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
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

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static void Postfix(ref bool __result, TacticalAbility ability)
            {
                try
                {
                    if (ability != null && ability.TacticalAbilityDef.SkillTags.Contains(attackAbility_Tag))
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
                    if (ability != null && arExcludeList.Contains(ability.TacticalAbilityDef.name))
                    {
                        __result = false;
                        return;
                    }
                    __result = true;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
