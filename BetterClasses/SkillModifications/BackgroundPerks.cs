using Base.Core;
using Base.Defs;
using Base.Entities.Statuses;
using Base.UI;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Entities.Weapons;
using System.Linq;
using UnityEngine;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class BackgroundPerks
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void ApplyChanges(bool doNotLocalize = true)
        {
            // Pacifist, WP +4, but - 4 STR
            Create_Pacifist(doNotLocalize);
            // Healer, WP + 2.Healing + 30 %, <- vanilla, needs no change
            //Create_Healer(doNotLocalize);
            // Farsighted, WP + 2, Perception + 4
            Create_Farsighted(doNotLocalize);
            // Firefighter, Fire Resistance, STR + 2
            Create_Firefighter(doNotLocalize);
            // Strongman, STR +4, Perception - 10
            Create_Strongman(doNotLocalize);
            // Gym Rat, STR + 2, Carry Weight +25 %
            Create_GymRat(doNotLocalize);
            // Night Owl, Nightvision, +2 SPD, -2WP
            Create_NightOwl(doNotLocalize);
            // Athlete, "Jump", +1 SPD
            Create_Athlete(doNotLocalize);
            // Thief, Stealth + 15 %, SPD + 1
            Create_Thief(doNotLocalize);
            // Special Forces, Accuracy + 10 %
            // Guerilla, Grenades deal +10 % damage
            Create_Guerilla(doNotLocalize);
            // Attentive, Perception +4, Hearing Range +10
            // Martial Artist, Melee Resistance 10 %
            // Transhumanist, Can mutate all body parts
            // Hitman, Damage + 10 %, Willpower - 3
            // Thug, Damage +10 %, Accuracy - 10 %
            // Psychic, Gain "Psychic Scream"
            // Plumber, Immune to goo
            // Junkie, Acid Resistance
            // Astronaut, Jetpack Proficiency
        }
        public static void Create_Pacifist(bool doNotLocalize = true)
        {
            string skillName = "Pacifist_AbilityDef";
            PassiveModifierAbilityDef source = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("Talent"));
            PassiveModifierAbilityDef Pacifist = Helper.CreateDefFromClone(
                source,
                "8e907b1f-f94e-4047-b27a-4de7022868b9",
                skillName);
            Pacifist.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "90919b90-e7a6-47fc-9bd1-609e254f53eb",
                skillName);
            Pacifist.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "160dfb40-c5cf-414d-a04f-5ba23bcd761b",
                skillName);

            // Set necessary fields
            Pacifist.StatModifications = new ItemStatModification[]
              {
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Endurance,
                    Modification = StatModificationType.Add,
                    Value = -4
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Willpower,
                    Modification = StatModificationType.Add,
                    Value = 4
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Willpower,
                    Modification = StatModificationType.AddMax,
                    Value = 4
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.WillPoints,
                    Modification = StatModificationType.Add,
                    Value = 4
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.WillPoints,
                    Modification = StatModificationType.AddMax,
                    Value = 4
                }
              };
            Pacifist.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
            Pacifist.DamageKeywordPairs = new DamageKeywordPair[0];
            Pacifist.CharacterProgressionData.RequiredSpeed = 0;
            Pacifist.CharacterProgressionData.RequiredStrength = 0;
            Pacifist.CharacterProgressionData.RequiredWill = 0;
            Pacifist.ViewElementDef.DisplayName1 = new LocalizedTextBind("PACIFIST", doNotLocalize);
            Pacifist.ViewElementDef.Description = new LocalizedTextBind("Gain +4 willpower but lose -4 strength", doNotLocalize);
            Sprite PacifistIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_Pacifist.png");
            Pacifist.ViewElementDef.LargeIcon = PacifistIcon;
            Pacifist.ViewElementDef.SmallIcon = PacifistIcon;
        }
        public static void Create_Healer(bool doNotLocalize = true)
        {
            PassiveModifierAbilityDef healer = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("Helpful_AbilityDef"));
            healer.StatModifications[0].Value = 2;
            healer.StatModifications[1].Value = 2;
            healer.StatModifications[2].Value = 1.3f;
            healer.StatModifications[3].Value = 2;
            healer.StatModifications[4].Value = 2;
            healer.ViewElementDef.Description = new LocalizedTextBind("Gain +2 willpower and +30% bonus heal value", doNotLocalize);
        }
        public static void Create_Farsighted(bool doNotLocalize = true)
        {
            PassiveModifierAbilityDef brainiac = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("Brainiac_AbilityDef"));
            //brainiac.StatModifications[0].Value = 2;
            //brainiac.StatModifications[1].Value = 2;
            brainiac.StatModifications[2].Value = 4;
            //brainiac.StatModifications[3].Value = 2;
            //brainiac.StatModifications[4].Value = 2;
            brainiac.ViewElementDef.Description = new LocalizedTextBind("Gain +2 willpower and +4 perception", doNotLocalize);
        }
        public static void Create_Strongman(bool doNotLocalize = true)
        {
            PassiveModifierAbilityDef strongMan = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("Strongman_AbilityDef"));
            strongMan.StatModifications[0].Value = -10;
            strongMan.StatModifications[1].Value = 4;
            strongMan.StatModifications[2].Value = 4;
            strongMan.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
            strongMan.ViewElementDef.Description = new LocalizedTextBind("Gain +4 strength but lose -10 perception", doNotLocalize);
        }
        public static void Create_Thief(bool doNotLocalize = true)
        {
            PassiveModifierAbilityDef thief = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("Thief_AbilityDef"));
            thief.StatModifications[0].Value = 0.15f;
            //thief.StatModifications[1].Value = 1;
            thief.ViewElementDef.Description = new LocalizedTextBind("Gain +1 speed and +15% stealth", doNotLocalize);
        }
        public static void Create_GymRat(bool doNotLocalize = true)
        {
            PassiveModifierAbilityDef gymRat = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("GymRat_AbilityDef"));
            gymRat.StatModifications = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("Resourceful_AbilityDef")).StatModifications;
            //gymRat.StatModifications[0].Value = 2;
            //gymRat.StatModifications[1].Value = 2;
            //gymRat.StatModifications[2].Value = 1.25f;
            //gymRat.name = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("GymRat_AbilityDef")).name;
            //gymRat.ViewElementDef.DisplayName1 = new LocalizedTextBind("GYM RAT", doNotLocalize);
            gymRat.ViewElementDef.Description = new LocalizedTextBind("Gain +2 strength and +25% bonus carry weight", doNotLocalize);
            Sprite gymRatIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_GymRat-2.png");
            gymRat.ViewElementDef.LargeIcon = gymRatIcon;
            gymRat.ViewElementDef.SmallIcon = gymRatIcon;
        }
        public static void Create_Firefighter(bool doNotLocalize = true)
        {
            string skillName = "FireFighter_AbilityDef";
            DamageMultiplierAbilityDef source = Repo.GetAllDefs<DamageMultiplierAbilityDef>().FirstOrDefault(p => p.name.Contains("FireResistant_DamageMultiplierAbilityDef"));
            DamageMultiplierAbilityDef fireFighter = Helper.CreateDefFromClone(
                source,
                "8647a3e3-1fb0-44ca-9d6d-352613068070",
                skillName);
            fireFighter.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "7a96d706-9771-406b-bbfa-4705eaf3cb1c",
                skillName);
            fireFighter.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "b77223a3-c109-4a3e-8b59-a7d028f181f2",
                skillName);
            // Set necessary fields
            // TODO: Finding a way to add strength modifier, the below don't work, the base ability has no StatModifications
            //fireFighter.StatModifications = new ItemStatModification[]
            //  {
            //    new ItemStatModification()
            //    {
            //        TargetStat = StatModificationTarget.Endurance,
            //        Modification = StatModificationType.Add,
            //        Value = 2
            //    },
            //  };
            //fireFighter.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
            //fireFighter.DamageKeywordPairs = new DamageKeywordPair[0];
            fireFighter.CharacterProgressionData.RequiredSpeed = 0;
            fireFighter.CharacterProgressionData.RequiredStrength = 0;
            fireFighter.CharacterProgressionData.RequiredWill = 0;
            fireFighter.ViewElementDef.DisplayName1 = new LocalizedTextBind("FIREFIGHTER", doNotLocalize);
            fireFighter.ViewElementDef.Description = new LocalizedTextBind("Reduce fire damage By -50%", doNotLocalize);
        }
        public static void Create_NightOwl(bool doNotLocalize = true)
        {
            string skillName = "NightOwl_AbilityDef";
            PassiveModifierAbilityDef source = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("EnhancedVision_AbilityDef"));
            PassiveModifierAbilityDef NightOwl = Helper.CreateDefFromClone(
                source,
                "9ca95f13-49d9-49fd-90bc-f1f59c99003b",
                skillName);
            NightOwl.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "ff169374-0ec6-481d-a071-2e8abd407755",
                skillName);
            NightOwl.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "da7ef36f-c51b-4a4a-9650-502ce52de24e",
                skillName);
            // reset all passive modifications we don't need
            NightOwl.DamageKeywordPairs = new DamageKeywordPair[0];
            // Set necessary fields
            NightOwl.StatModifications = new ItemStatModification[]
             {
                new ItemStatModification() // reduce current WP
                {
                    TargetStat = StatModificationTarget.Willpower,
                    Modification = StatModificationType.Add,
                    Value = -2.0f
                },
                new ItemStatModification() // reduce max WP, otherwise the operative could gain the 2 WP back in mission
                {
                    TargetStat = StatModificationTarget.Willpower,
                    Modification = StatModificationType.AddMax,
                    Value = -2.0f
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Speed,
                    Modification = StatModificationType.Add,
                    Value = 2.0f
                }
             };
            NightOwl.ItemTagStatModifications[0].EquipmentStatModification.Value = 0.0f;
            NightOwl.CharacterProgressionData.RequiredSpeed = 0;
            NightOwl.CharacterProgressionData.RequiredStrength = 0;
            NightOwl.CharacterProgressionData.RequiredWill = 0;
            NightOwl.ViewElementDef.DisplayName1 = new LocalizedTextBind("NIGHT OWL", doNotLocalize);
            NightOwl.ViewElementDef.Description = new LocalizedTextBind("No penalty incurred in dark enviornments, +2 Speed, -2 WillPoints", doNotLocalize);
            Sprite NightOwlIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_NightOwl.png");
            NightOwl.ViewElementDef.LargeIcon = NightOwlIcon;
            NightOwl.ViewElementDef.SmallIcon = NightOwlIcon;
        }
        public static void Create_Athlete(bool doNotLocalize = true)
        {
            string skillName = "Athlete_AbilityDef";
            AddNavAreasAbilityDef source = Repo.GetAllDefs<AddNavAreasAbilityDef>().FirstOrDefault(p => p.name.Contains("Humanoid_HighJump_AbilityDef"));
            AddNavAreasAbilityDef athlete = Helper.CreateDefFromClone(
                source,
                "5e2e7ad9-164d-4ac0-ae6f-23570bcfa525",
                skillName);
            athlete.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "dab39934-f4b1-4348-beec-e281ce0fb807",
                skillName);
            athlete.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "2b6ea7c4-beaa-405e-99ab-d6e6bf304f8d",
                skillName);
            // Set necessary fields
            // TODO: Finding a way to add speed modifier, the below don't work, the base ability has no StatModifications
            //athlete.StatModifications = new ItemStatModification[]
            //{
            //    new ItemStatModification()
            //    {
            //        TargetStat = StatModificationTarget.Speed,
            //        Modification = StatModificationType.Add,
            //        Value = 1
            //    },
            //};
            //athlete.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
            //athlete.DamageKeywordPairs = new DamageKeywordPair[0];
            athlete.CharacterProgressionData.RequiredSpeed = 0;
            athlete.CharacterProgressionData.RequiredStrength = 0;
            athlete.CharacterProgressionData.RequiredWill = 0;
            athlete.ViewElementDef.DisplayName1 = new LocalizedTextBind("ATHLETE", doNotLocalize);
            athlete.ViewElementDef.Description = new LocalizedTextBind("Jump up one floor", doNotLocalize);
            Sprite athleteIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(t => t.name.Equals("E_ViewElement [ExpertRunner_AbilityDef]")).LargeIcon;
            athlete.ViewElementDef.LargeIcon = athleteIcon;
            athlete.ViewElementDef.SmallIcon = athleteIcon;
        }
        public static void Create_Guerilla(bool doNotLocalize = true)
        {
            string skillName = "Guerilla_AbilityDef";
            PassiveModifierAbilityDef source = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("ExpertThrower_AbilityDef"));
            PassiveModifierAbilityDef Guerilla = Helper.CreateDefFromClone(
                source,
                "a35b7814-1b64-4ce6-ab31-ab4a70ad1732",
                skillName);
            Guerilla.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "b752b25c-305d-4a01-aa3f-cf34d91c13ef",
                skillName);
            Guerilla.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "5feb11bf-72f0-4bee-8cf5-c27b84efc5b3",
                skillName);

            // Set necessary fields
            //Guerilla.StatModifications = new ItemStatModification[0]; // <- not necessary, is already null
            Guerilla.ItemTagStatModifications[0].EquipmentStatModification.TargetStat = StatModificationTarget.BonusAttackDamage;
            Guerilla.ItemTagStatModifications[0].EquipmentStatModification.Modification = StatModificationType.Multiply;
            Guerilla.ItemTagStatModifications[0].EquipmentStatModification.Value = 1.1f;
            //Guerilla.DamageKeywordPairs = new DamageKeywordPair[0]; // <- not necessary, is already null
            Guerilla.CharacterProgressionData.RequiredSpeed = 0;
            Guerilla.CharacterProgressionData.RequiredStrength = 0;
            Guerilla.CharacterProgressionData.RequiredWill = 0;
            Guerilla.ViewElementDef.DisplayName1 = new LocalizedTextBind("GUERILLA", doNotLocalize);
            Guerilla.ViewElementDef.Description = new LocalizedTextBind("Grenades deal +10% damage", doNotLocalize);
            Sprite GuerillaIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_Guerilla.png");
            Guerilla.ViewElementDef.LargeIcon = GuerillaIcon;
            Guerilla.ViewElementDef.SmallIcon = GuerillaIcon;
        }
    }
}