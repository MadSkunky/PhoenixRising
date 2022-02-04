using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects.ApplicationConditions;
using Base.UI;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Effects.ApplicationConditions;
using PhoenixPoint.Tactical.Entities.Statuses;
using System;
using System.Linq;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class SkillModsMain
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            try
            {
                // Assault skills ------------------------------------------------------
                AssaultSkills.ApplyChanges();

                // Sniper skills start ------------------------------------------------------
                SniperSkills.ApplyChanges();

                // Heavy skills start --------------------------------------------------------
                HeavySkills.ApplyChanges();

                // Berserker skills start ----------------------------------------------------
                BerserkerSkills.ApplyChanges();

                // Infiltrator skills start --------------------------------------------------
                InfiltratorSkills.ApplyChanges();

                // Technician skills start ---------------------------------------------------
                TechnicianSkills.ApplyChanges();

                // Priest skills start -------------------------------------------------------
                PriestSkills.ApplyChanges();

                // Call Background perk changes -------------------------------------------------------
                BackgroundPerks.ApplyChanges();

                // Faction perks
                FactionPerks.ApplyChanges();

                // Tweaking the weapon proficiency perks incl. descriptions, see below
                Change_ProficiencyPerks();

                // BattleFocus, currently used as placeholder, will go to Vengeance Torso
                Create_BattleFocus();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private static void Change_ProficiencyPerks()
        {
            foreach (PassiveModifierAbilityDef pmad in Repo.GetAllDefs<PassiveModifierAbilityDef>())
            {
                if (pmad.CharacterProgressionData != null && pmad.name.Contains("Talent"))
                {
                    // Assault rifle proficiency fix, was set to shotguns
                    if (pmad.name.Contains("Assault"))
                    {
                        GameTagDef ARtagDef = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gtd => gtd.name.Contains("AssaultRifleItem_TagDef"));
                        pmad.ItemTagStatModifications[0].ItemTag = ARtagDef;
                    }

                    // Change descrition text, not localized (currently), old one mentions fixed buffs that are taken away or set differently by this mod
                    string newText = Helper.NotLocalizedTextMap[pmad.ViewElementDef.name][ViewElement.Description];
                    pmad.ViewElementDef.Description = new LocalizedTextBind(newText, doNotLocalize);

                    Logger.Debug("Proficiency def name: " + pmad.name);
                    Logger.Debug("Viewelement name:     " + pmad.ViewElementDef.name);
                    Logger.Debug("Display1 name:        " + pmad.ViewElementDef.DisplayName1.Localize());
                    Logger.Debug("Description:          " + pmad.ViewElementDef.Description.Localize());

                    // Get modification from config, but first -0.1 to normalise to 0.0 (proficiency perks are all set to +0.1 buff)
                    float newStatModification = -0.1f + Config.BuffsForAdditionalProficiency[Proficiency.Buff];
                    // Loop through all subsequent item stat modifications
                    if (pmad.ItemTagStatModifications.Length > 0)
                    {
                        for (int i = 0; i < pmad.ItemTagStatModifications.Length; i++)
                        {
                            pmad.ItemTagStatModifications[i].EquipmentStatModification.Value += newStatModification;

                            Logger.Debug("  Target item: " + pmad.ItemTagStatModifications[i].ItemTag.name);
                            Logger.Debug("  Target stat: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.TargetStat);
                            Logger.Debug(" Modification: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.Modification);
                            Logger.Debug("        Value: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.Value);
                        }
                    }
                    Logger.Debug("----------------------------------------------------", false);
                }
            }
        }

        // New Battle Focus ability
        public static void Create_BattleFocus()
        {
            string skillName = "BattleFocus_AbilityDef";
            bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

            // Source to clone from
            ApplyStatusAbilityDef masterMarksman = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef"));

            // Create Neccessary RuntimeDefs
            ApplyStatusAbilityDef battleFocusAbility = Helper.CreateDefFromClone(
                masterMarksman,
                "64fc75aa-93be-4d79-b5ac-191c5c7820da",
                skillName);
            AbilityCharacterProgressionDef progression = Helper.CreateDefFromClone(
                masterMarksman.CharacterProgressionData,
                "7ffae720-a656-454e-a95b-b861a673718a",
                skillName);
            TacticalTargetingDataDef targetingData = Helper.CreateDefFromClone(
                masterMarksman.TargetingDataDef,
                "fed0600a-14b3-4ef5-ac0c-31b3bf6f1e6c",
                skillName);
            TacticalAbilityViewElementDef vieElement = Helper.CreateDefFromClone(
                masterMarksman.ViewElementDef,
                "b498b9de-f10b-464c-a9f9-29a293568b04",
                skillName);
            StanceStatusDef stanceStatus = Helper.CreateDefFromClone( // Borrow status from Sneak Attack, Master Marksman status does not fit
                Repo.GetAllDefs<StanceStatusDef>().FirstOrDefault(p => p.name.Equals("E_SneakAttackStatus [SneakAttack_AbilityDef]")),
                "05929419-7d20-47aa-b700-fa6bc6602716",
                "E_Status [" + skillName + "]");
            VisibleActorsInRangeEffectConditionDef visibleActorsInRangeEffectCondition = Helper.CreateDefFromClone(
                (VisibleActorsInRangeEffectConditionDef)masterMarksman.TargetApplicationConditions[0],
                "63a34054-28de-488e-ae4a-af451434f0d4",
                skillName);

            // Set fields
            battleFocusAbility.CharacterProgressionData = progression;
            battleFocusAbility.TargetingDataDef = targetingData;
            battleFocusAbility.ViewElementDef = vieElement;
            battleFocusAbility.StatusDef = stanceStatus;
            battleFocusAbility.TargetApplicationConditions = new EffectConditionDef[] { visibleActorsInRangeEffectCondition };
            progression.RequiredStrength = 0;
            progression.RequiredWill = 0;
            progression.RequiredSpeed = 0;
            targetingData.Origin.Range = 10.0f;
            vieElement.DisplayName1 = new LocalizedTextBind("BATTLE FOCUS", doNotLocalize);
            vieElement.Description = new LocalizedTextBind("If there are enemies within 10 tiles your attacks gain +10% damage", doNotLocalize);
            // TODO: Change to own Icon
            stanceStatus.EffectName = skillName;
            stanceStatus.ShowNotification = true;
            stanceStatus.Visuals = battleFocusAbility.ViewElementDef;
            stanceStatus.StatModifications[0].Value = 1.1f;
            visibleActorsInRangeEffectCondition.TargetingData = battleFocusAbility.TargetingDataDef;
            visibleActorsInRangeEffectCondition.ActorsInRange = true;
        }

    }
}
