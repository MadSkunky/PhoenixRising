using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects.ApplicationConditions;
using Base.UI;
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
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly AssetsManager assetsManager = GameUtl.GameComponent<AssetsManager>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();

        public static void ApplyChanges()
        {
            try
            {
                // Get config setting for localized texts.
                bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

                // BattleFocus, currently used as placeholder, will go to Vengeance Torso
                Create_BattleFocus(Repo, Config);

                // Assault skills ------------------------------------------------------
                AssaultSkills.ApplyChanges(doNotLocalize);

                // Sniper skills start ------------------------------------------------------

                // Extreme Focus: Set to 1 AP regardless of weapon type
                ChangeAbilitiesCostStatusDef extremeFocusAPcostMod = Repo.GetAllDefs<ChangeAbilitiesCostStatusDef>().FirstOrDefault(c => c.name.Contains("ExtremeFocus_AbilityDef"));
                extremeFocusAPcostMod.AbilityCostModification.ActionPointModType = TacticalAbilityModificationType.Set;
                extremeFocusAPcostMod.AbilityCostModification.ActionPointMod = 0.25f;
                extremeFocusAPcostMod.Visuals.Description = new LocalizedTextBind("Overwatch cost is set to 1 Action Point cost for all weapons", doNotLocalize);

                // Armor Break: Set to 15 shred and -25% damage

                // Gunslinger: 3 pistol shots in one action (Rage Burst)

                // Kill Zone: An additional overwatch shot

                // Sniper skills end ---------------------------------------------------------

                // Heavy skills start --------------------------------------------------------

                // Return Fire: Fix to work on all classes
                TacticalAbilityDef returnFire = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains("ReturnFire_AbilityDef"));
                returnFire.ActorTags = new GameTagDef[0]; // Deletes all given tags => no restriction for any class

                // War Cry: -1 AP and -10% damage, doubled if WP of target < WP of caster

                // Rage Burst: Increase accuracy and cone angle

                // Dynamic Resistance: Copy from Acheron

                // Hunker Down: -25% incoming damage for 2 AP and 2 WP

                // Jetpack Control: 2 AP jump, 12 tiles range

                // Boom Blast: -30% range instead of +50%

                // Heavy skills end ----------------------------------------------------------

                // Berserker skills start ----------------------------------------------------

                // Dash changes -- cancelled, delayed
                //RepositionAbilityDef dash = Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef"));

                // Adrenaline Rush: 1 AP for one handed weapons and skills, no WP restriction

                // Melee Specialist: +10% damage instead of +25%

                // Personal Space: Until next turn, attack first enemy entering melee range

                // Berserker skills end ------------------------------------------------------

                // Infiltrator skills start --------------------------------------------------

                // Sneak Attack: Direct fire and melee +60 damage while not potted

                // Master Archer: Shoot your Crossbow 3 times at one target, reveal your position

                // Cautious: +10% stealth

                // Infiltrator skills end ----------------------------------------------------

                // Technician skills start ---------------------------------------------------

                // Electric Reinforcements: 10 tiles range, +10 armor, 1 AP and 3 WP

                // Stability: Gain 5% extra accuracy per remaining AP up to 20%

                // Amplify Pain: If your next attack deals special damage, double that damage (Bleeding, Paralysis, Viral, Poison, Fire, EMP, Sonic, Shock, Virophage)

                // Technician skills end -----------------------------------------------------

                // Priest skills start -------------------------------------------------------

                // Biochemist: Paralysis, Poison and Viral damage increased 25%

                // Enrage (Mutog): Target Mutog becomes "Enraged"

                // Priest skills end ---------------------------------------------------------

                // Faction perks
                // Mist Breather adding progression def
                ApplyEffectAbilityDef mistBreather = Repo.GetAllDefs<ApplyEffectAbilityDef>().FirstOrDefault(a => a.name.Equals("Exalted_MistBreather_AbilityDef"));
                AbilityCharacterProgressionDef mbProgressionDef = Helper.CreateDefFromClone(
                    Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                    "9eaf8809-01d9-4582-89e0-78c8596f5e7d",
                    "MistBreather_AbilityDef");
                mbProgressionDef.RequiredStrength = 0;
                mbProgressionDef.RequiredWill = 0;
                mbProgressionDef.RequiredSpeed = 0;
                mistBreather.CharacterProgressionData = mbProgressionDef;

                // Tweaking the weapon proficiency perks incl. descriptions
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
                        string newText = BetterClasses.Helper.NotLocalizedTextMap[pmad.ViewElementDef.name][ViewElement.Description];
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
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        // New Battle Focus ability
        public static void Create_BattleFocus(DefRepository Repo, Settings Config)
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
