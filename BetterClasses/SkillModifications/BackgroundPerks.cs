using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Statuses;
using Base.UI;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
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
        //private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void ApplyChanges(bool doNotLocalize = true)
        {
            // Pacifist, WP +4, but - 4 STR
            Create_Pacifist(doNotLocalize);
            // Healer, WP + 2, Healing + 30 %, <- vanilla, needs no change
            Change_Healer(doNotLocalize);
            // Farsighted, WP + 2, Perception + 4
            Change_Farsighted(doNotLocalize);
            // Firefighter, Fire Resistance, STR + 2
            Create_Firefighter(doNotLocalize);
            // Strongman, STR +4, Perception - 10
            Change_Strongman(doNotLocalize);
            // Gym Rat, STR + 2, Carry Weight +25 %
            Change_GymRat(doNotLocalize);
            // Night Owl, Nightvision, +2 SPD, -2WP
            Create_NightOwl(doNotLocalize);
            // Athlete, "Jump", +1 SPD
            Create_Athlete(doNotLocalize);
            // Thief, Stealth + 15 %, SPD + 1
            Change_Thief(doNotLocalize);
            // Special Forces, Accuracy + 10 %
            Create_SpecialForces(doNotLocalize);
            // Guerilla, Grenades deal +10 % damage
            Create_Guerilla(doNotLocalize);
            // Attentive, Perception +4, Hearing Range +10
            Create_Attentive(doNotLocalize);
            // Martial Artist, Melee Resistance 10 %
            Create_MartialArtist(doNotLocalize);
            // Transhumanist, Can mutate all body parts
            //Create_Transhumanist(doNotLocalize);
            // Hitman, Damage + 10 %, Willpower - 3
            Create_Hitman(doNotLocalize);
            // Thug, Damage +10 %, Accuracy - 10 %
            Create_Thug(doNotLocalize);
            // Psychic, Gain "Psychic Scream"
            Create_Psychic(doNotLocalize);
            // Plumber, Immune to goo
            Create_Plumber(doNotLocalize);
            // Junkie, Acid Resistance
            Create_Junkie(doNotLocalize);
            // Astronaut, Jetpack Proficiency
            Create_Astronaut(doNotLocalize);
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
            float strength = -4.0f;
            float willpower = 4.0f;
            Pacifist.StatModifications = new ItemStatModification[]
            {
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Endurance,
                    Modification = StatModificationType.Add,
                    Value = strength
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Endurance,
                    Modification = StatModificationType.AddMax,
                    Value = strength
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Willpower,
                    Modification = StatModificationType.Add,
                    Value = willpower
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Willpower,
                    Modification = StatModificationType.AddMax,
                    Value = willpower
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.WillPoints,
                    Modification = StatModificationType.Add,
                    Value = willpower
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.WillPoints,
                    Modification = StatModificationType.AddMax,
                    Value = willpower
                }
            };
            Pacifist.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
            Pacifist.DamageKeywordPairs = new DamageKeywordPair[0];
            Pacifist.CharacterProgressionData.RequiredSpeed = 0;
            Pacifist.CharacterProgressionData.RequiredStrength = 0;
            Pacifist.CharacterProgressionData.RequiredWill = 0;
            Pacifist.ViewElementDef.DisplayName1 = new LocalizedTextBind("PACIFIST", doNotLocalize);
            Pacifist.ViewElementDef.Description = new LocalizedTextBind("Gain 4 points to willpower but lose 4 strength", doNotLocalize);
            Sprite PacifistIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_Pacifist.png");
            Pacifist.ViewElementDef.LargeIcon = PacifistIcon;
            Pacifist.ViewElementDef.SmallIcon = PacifistIcon;
        }
        public static void Change_Healer(bool doNotLocalize = true)
        {
            PassiveModifierAbilityDef healer = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("Helpful_AbilityDef"));
            for (int i = 0; i < healer.StatModifications.Length; i++)
            {
                if (healer.StatModifications[i].TargetStat == StatModificationTarget.BonusHealValue)
                {
                    healer.StatModifications[i].Value = 1.3f;
                }
                if (healer.StatModifications[i].TargetStat == StatModificationTarget.WillPoints
                    || healer.StatModifications[i].TargetStat == StatModificationTarget.Willpower)
                {
                    healer.StatModifications[i].Value = 2.0f;
                }
            }
            healer.ViewElementDef.Description = new LocalizedTextBind("Gain 2 points to willpower and 30% bonus heal value", doNotLocalize);
        }
        public static void Change_Farsighted(bool doNotLocalize = true)
        {
            PassiveModifierAbilityDef brainiac = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("Brainiac_AbilityDef"));
            for (int i =0; i < brainiac.StatModifications.Length; i++)
            {
                if (brainiac.StatModifications[i].TargetStat == StatModificationTarget.Perception)
                {
                    brainiac.StatModifications[i].Value = 4.0f;
                }
            }
            brainiac.ViewElementDef.Description = new LocalizedTextBind("Gain 2 willpower and 4 perception", doNotLocalize);
        }
        public static void Change_Strongman(bool doNotLocalize = true)
        {
            PassiveModifierAbilityDef strongMan = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("Strongman_AbilityDef"));
            for (int i = 0; i < strongMan.StatModifications.Length; i++)
            {
                if (strongMan.StatModifications[i].TargetStat == StatModificationTarget.Perception)
                {
                    strongMan.StatModifications[i].Value = -10.0f;
                }
                if (strongMan.StatModifications[i].TargetStat == StatModificationTarget.Endurance)
                {
                    strongMan.StatModifications[i].Value = 4.0f;
                }
            }
            strongMan.ItemTagStatModifications = new EquipmentItemTagStatModification[0]; // delete weapon buff and proficiency
            strongMan.ViewElementDef.Description = new LocalizedTextBind("Gain 4 strength but lose 10 perception", doNotLocalize);
        }
        public static void Change_GymRat(bool doNotLocalize = true)
        {
            PassiveModifierAbilityDef gymRat = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("GymRat_AbilityDef"));
            gymRat.StatModifications = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Contains("Resourceful_AbilityDef")).StatModifications;
            gymRat.ViewElementDef.DisplayName1 = new LocalizedTextBind("GYM RAT", doNotLocalize);
            gymRat.ViewElementDef.Description = new LocalizedTextBind("Gain 2 strength and 25% bonus carry weight", doNotLocalize);
            Sprite gymRatIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_GymRat-2.png");
            gymRat.ViewElementDef.LargeIcon = gymRatIcon;
            gymRat.ViewElementDef.SmallIcon = gymRatIcon;
        }
        public static void Create_Firefighter(bool doNotLocalize = true)
        {
            string skillName = "FireFighter_AbilityDef";
            DamageMultiplierAbilityDef source = Repo.GetAllDefs<DamageMultiplierAbilityDef>().FirstOrDefault(p => p.name.Equals("FireResistant_DamageMultiplierAbilityDef"));
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
            fireFighter.ViewElementDef.Description = new LocalizedTextBind("Reduce fire damage by 50%", doNotLocalize);
        }
        public static void Create_NightOwl(bool doNotLocalize = true)
        {
            string skillName = "NightOwl_AbilityDef";
            PassiveModifierAbilityDef source = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("EnhancedVision_AbilityDef"));
            PassiveModifierAbilityDef nightOwl = Helper.CreateDefFromClone(
                source,
                "9ca95f13-49d9-49fd-90bc-f1f59c99003b",
                skillName);
            nightOwl.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "ff169374-0ec6-481d-a071-2e8abd407755",
                skillName);
            nightOwl.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "da7ef36f-c51b-4a4a-9650-502ce52de24e",
                skillName);
            // reset all passive modifications we don't need
            nightOwl.DamageKeywordPairs = new DamageKeywordPair[0];
            // Set necessary fields
            float willpower = -2.0f;
            float speed = 2.0f;
            nightOwl.StatModifications = new ItemStatModification[]
            {
                new ItemStatModification() // reduce current WP
                {
                    TargetStat = StatModificationTarget.Willpower,
                    Modification = StatModificationType.Add,
                    Value = willpower
                },
                new ItemStatModification() // reduce max WP, otherwise the operative could gain the 2 WP back in mission
                {
                    TargetStat = StatModificationTarget.Willpower,
                    Modification = StatModificationType.AddMax,
                    Value = willpower
                },
                new ItemStatModification() // same for WillPoints
                {
                    TargetStat = StatModificationTarget.WillPoints,
                    Modification = StatModificationType.Add,
                    Value = willpower
                },
                new ItemStatModification() // and WillPoints max
                {
                    TargetStat = StatModificationTarget.WillPoints,
                    Modification = StatModificationType.AddMax,
                    Value = willpower
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Speed,
                    Modification = StatModificationType.Add,
                    Value = speed
                }
            };
            for (int i = 0; i < nightOwl.ItemTagStatModifications.Length; i++)
            {
                if (nightOwl.ItemTagStatModifications[i].EquipmentStatModification.Value > 0.0f)
                {
                    nightOwl.ItemTagStatModifications[i].EquipmentStatModification.Value = 0.0f;
                }
            }
            nightOwl.CharacterProgressionData.RequiredSpeed = 0;
            nightOwl.CharacterProgressionData.RequiredStrength = 0;
            nightOwl.CharacterProgressionData.RequiredWill = 0;
            nightOwl.ViewElementDef.DisplayName1 = new LocalizedTextBind("NIGHT OWL", doNotLocalize);
            nightOwl.ViewElementDef.Description = new LocalizedTextBind("No penalty incurred in dark enviornments, +2 speed, -2 willpower", doNotLocalize);
            Sprite NightOwlIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_NightOwl.png");
            nightOwl.ViewElementDef.LargeIcon = NightOwlIcon;
            nightOwl.ViewElementDef.SmallIcon = NightOwlIcon;
        }
        public static void Create_Athlete(bool doNotLocalize = true)
        {
            string skillName = "Athlete_AbilityDef";
            AddNavAreasAbilityDef source = Repo.GetAllDefs<AddNavAreasAbilityDef>().FirstOrDefault(p => p.name.Equals("Humanoid_HighJump_AbilityDef"));
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
        public static void Change_Thief(bool doNotLocalize = true)
        {
            PassiveModifierAbilityDef thief = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("Thief_AbilityDef"));
            for (int i = 0; i < thief.StatModifications.Length; i++)
            {
                if (thief.StatModifications[i].TargetStat == StatModificationTarget.Stealth)
                {
                    thief.StatModifications[i].Value = 0.15f;
                }
            }
            thief.ViewElementDef.Description = new LocalizedTextBind("Gain 1 speed and 15% stealth", doNotLocalize);
        }
        public static void Create_SpecialForces(bool doNotLocalize = true)
        {
            string skillName = "SpecialForces_AbilityDef";
            PassiveModifierAbilityDef source = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("EagleEyed_AbilityDef"));
            PassiveModifierAbilityDef specialForces = Helper.CreateDefFromClone(
                source,
                "15150fb6-0088-4124-bf7c-31146a2006ed",
                skillName);
            specialForces.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "ebc8f83b-b4a1-42e9-8bdb-03d34abca012",
                skillName);
            specialForces.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "001c6300-51f6-44a4-890d-b9f1a6587f53",
                skillName);

            // Set necessary fields
            for (int i = 0; i < specialForces.StatModifications.Length; i++)
            {
                if (specialForces.StatModifications[i].TargetStat == StatModificationTarget.Accuracy)
                {
                    specialForces.StatModifications[i].Value = 0.1f;
                }
            }
            specialForces.CharacterProgressionData.RequiredSpeed = 0;
            specialForces.CharacterProgressionData.RequiredStrength = 0;
            specialForces.CharacterProgressionData.RequiredWill = 0;
            specialForces.ViewElementDef.DisplayName1 = new LocalizedTextBind("SPECIAL FORCES", doNotLocalize);
            specialForces.ViewElementDef.Description = new LocalizedTextBind("Gain 10% accuracy", doNotLocalize);
            Sprite specialForcesIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_SpecOp-3.png");
            specialForces.ViewElementDef.LargeIcon = specialForcesIcon;
            specialForces.ViewElementDef.SmallIcon = specialForcesIcon;
        }
        public static void Create_Guerilla(bool doNotLocalize = true)
        {
            string skillName = "Guerilla_AbilityDef";
            PassiveModifierAbilityDef source = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("ExpertThrower_AbilityDef"));
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
        public static void Create_Attentive(bool doNotLocalize = true)
        {
            string skillName = "Attentive_AbilityDef";
            PassiveModifierAbilityDef source = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("SelfDefenseSpecialist_AbilityDef"));
            PassiveModifierAbilityDef attentive = Helper.CreateDefFromClone(
                source,
                "7da2f9bc-8175-4b62-81ef-af66c6cd8a58",
                skillName);
            attentive.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "447fced9-3f37-4fce-8b81-e484e2a433b2",
                skillName);
            attentive.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "c0988f23-cd33-435d-a8a0-56a28168d98b",
                skillName);
            attentive.StatModifications = new ItemStatModification[]
              {
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Perception,
                    Modification = StatModificationType.Add,
                    Value = 4
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.HearingRange,
                    Modification = StatModificationType.Add,
                    Value = 10
                },
              };
            attentive.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
            attentive.ViewElementDef.DisplayName1 = new LocalizedTextBind("ATTENTIVE", doNotLocalize);
            attentive.ViewElementDef.Description = new LocalizedTextBind("Gain 4 perception and 10 tile hearing range", doNotLocalize);
            Sprite attentiveIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_FastLearner.png");
            attentive.ViewElementDef.LargeIcon = attentiveIcon;
            attentive.ViewElementDef.SmallIcon = attentiveIcon;
        }
        public static void Create_Transhumanist(bool doNotLocalize = true)
        {
        }
        public static void Create_Hitman(bool doNotLocalize = true)
        {
            string skillName = "Hitman_AbilityDef";
            PassiveModifierAbilityDef source = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("Focused_AbilityDef"));
            PassiveModifierAbilityDef hitMan = Helper.CreateDefFromClone(
                source,
                "0d856da5-b098-4e88-a09b-480f71e9470e",
                skillName);
            hitMan.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "285336cf-7c14-4b04-8ea0-e284b9bc1e7d",
                skillName);
            hitMan.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "181083f5-7fa7-490e-afdc-30a39d313dbe",
                skillName);

            // Set necessary fields
            float willpower = -3.0f;
            float bonusDamage = 1.1f;
            hitMan.StatModifications = new ItemStatModification[]
            {
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Willpower,
                    Modification = StatModificationType.Add,
                    Value = willpower
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.Willpower,
                    Modification = StatModificationType.AddMax,
                    Value = willpower
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.BonusAttackDamage,
                    Modification = StatModificationType.Multiply,
                    Value = bonusDamage
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.WillPoints,
                    Modification = StatModificationType.Add,
                    Value = willpower
                },
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.WillPoints,
                    Modification = StatModificationType.AddMax,
                    Value = willpower
                }
            };
            hitMan.ItemTagStatModifications = new EquipmentItemTagStatModification[0];
            hitMan.DamageKeywordPairs = new DamageKeywordPair[0];
            hitMan.CharacterProgressionData.RequiredSpeed = 0;
            hitMan.CharacterProgressionData.RequiredStrength = 0;
            hitMan.CharacterProgressionData.RequiredWill = 0;
            hitMan.ViewElementDef.DisplayName1 = new LocalizedTextBind("HITMAN", doNotLocalize);
            hitMan.ViewElementDef.Description = new LocalizedTextBind("Gain 10% damage but lose 3 willpower", doNotLocalize);
            Sprite hitManIcon = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(pm => pm.name.Equals("SilencedWeaponTalent_AbilityDef")).ViewElementDef.LargeIcon;
            hitMan.ViewElementDef.LargeIcon = hitManIcon;
            hitMan.ViewElementDef.SmallIcon = hitManIcon;
        }
        public static void Create_Thug(bool doNotLocalize = true)
        {
            string skillName = "Thug_AbilityDef";
            PassiveModifierAbilityDef source = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("Reckless_AbilityDef"));
            PassiveModifierAbilityDef Thug = Helper.CreateDefFromClone(
                source,
                "a1bb97ba-862f-4c0d-98b7-efbcd6f9021d",
                skillName);
            Thug.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "211ed239-6be7-42c4-8a85-0d9c9c73c645",
                skillName);
            Thug.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "e95de50c-8c04-4a2c-b219-296c816def67",
                skillName);
            //Thug.StatModifications[0].Value = 1.1f; // no change, not necessary
            //Thug.StatModifications[1].Value = -0.1f; // no change, not necessary
            Thug.ViewElementDef.DisplayName1 = new LocalizedTextBind("THUG", doNotLocalize);
            Thug.ViewElementDef.Description = new LocalizedTextBind("Gain 10% damage but lose 10% accuracy", doNotLocalize);
            Sprite thugIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_Heartless.png");
            Thug.ViewElementDef.LargeIcon = thugIcon;
            Thug.ViewElementDef.SmallIcon = thugIcon;
        }
        public static void Create_Psychic(bool doNotLocalize = true)
        {
            string skillName = "Psychic_AbilityDef";
            PsychicScreamAbilityDef source = Repo.GetAllDefs<PsychicScreamAbilityDef>().FirstOrDefault(p => p.name.Equals("Priest_PsychicScream_AbilityDef"));
            PsychicScreamAbilityDef psychic = Helper.CreateDefFromClone(
                source,
                "5fe50c69-3081-4502-98bf-1ba9d6911c99",
                skillName);
            psychic.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "ed634d13-34ec-43ef-9940-04400369535f",
                skillName);
            psychic.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "e853a40d-7117-46bb-9504-7c0dea1fff97",
                skillName);
            foreach (TacActorSimpleAbilityAnimActionDef animActionDef in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(
                aad => aad.name.Contains("PsychicScream")
                && aad.name.Contains("Soldier_Utka_AnimActionsDef")))
            {
                animActionDef.AbilityDefs = animActionDef.AbilityDefs.Append(psychic).ToArray();
                Logger.Debug("Anim Action '" + animActionDef.name + "' set for abilities:");
                foreach (AbilityDef ad in animActionDef.AbilityDefs)
                {
                    Logger.Debug("  " + ad.name);
                }
            }
            Logger.Debug("---------------------------------------------------------------", false);
            // Set necessary fields
            psychic.CharacterProgressionData.RequiredSpeed = 0;
            psychic.CharacterProgressionData.RequiredStrength = 0;
            psychic.CharacterProgressionData.RequiredWill = 0;
            psychic.ViewElementDef.DisplayName1 = new LocalizedTextBind("PSYCHIC", doNotLocalize);
            psychic.ViewElementDef.Description = new LocalizedTextBind("Gain psychic scream", doNotLocalize);
            Sprite psychicIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(tav => tav.name.Equals("E_ViewElement [PsychicResistant_DamageMultiplierAbilityDef]")).LargeIcon;
            psychic.ViewElementDef.LargeIcon = psychicIcon;
            psychic.ViewElementDef.SmallIcon = psychicIcon;
        }
        public static void Create_Plumber(bool doNotLocalize = true)
        {
            string skillName = "Plumber_AbilityDef";
            GooDamageMultiplierAbilityDef source = Repo.GetAllDefs<GooDamageMultiplierAbilityDef>().FirstOrDefault(p => p.name.Equals("GooImmunity_AbilityDef"));
            GooDamageMultiplierAbilityDef plumber = Helper.CreateDefFromClone(
                source,
                "f3aa9070-fbba-4fe7-8909-4b098e53187c",
                skillName);
            plumber.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "17382410-b088-442f-bd45-31aa43461fb6",
                skillName);
            plumber.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "533d07ea-4928-43ba-a4af-91bb2be260fe",
                skillName);
            plumber.ViewElementDef.DisplayName1 = new LocalizedTextBind("PLUMBER", doNotLocalize);
            Sprite plumberIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_Plumber.png");
            plumber.ViewElementDef.LargeIcon = plumberIcon;
            plumber.ViewElementDef.SmallIcon = plumberIcon;
        }
        public static void Create_Junkie(bool doNotLocalize = true)
        {
            string skillName = "Junkie_AbilityDef";
            DamageMultiplierAbilityDef source = Repo.GetAllDefs<DamageMultiplierAbilityDef>().FirstOrDefault(p => p.name.Equals("AcidResistant_DamageMultiplierAbilityDef"));
            DamageMultiplierAbilityDef junkie = Helper.CreateDefFromClone(
                source,
                "610c2c16-3572-4c5b-b75d-a05f2520266e",
                skillName);
            junkie.CharacterProgressionData = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                "fe81789d-940a-497e-bc2e-847a2ecadc05",
                skillName);
            junkie.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "7a0f28dd-ee19-4c52-a2af-ab894bf70845",
                skillName);
            junkie.ViewElementDef.DisplayName1 = new LocalizedTextBind("JUNKIE", doNotLocalize);
            Sprite junkieIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_PersonalTrack_Stimpack-2.png");
            junkie.ViewElementDef.LargeIcon = junkieIcon;
            junkie.ViewElementDef.SmallIcon = junkieIcon;
        }
        public static void Create_Astronaut(bool doNotLocalize = true)
        {
            string skillName = "Astronaut_AbilityDef";
            ClassProficiencyAbilityDef source = Repo.GetAllDefs<ClassProficiencyAbilityDef>().FirstOrDefault(p => p.name.Equals("UseAttachedEquipment_AbilityDef"));
            ClassProficiencyAbilityDef astronaut = Helper.CreateDefFromClone(
                source,
                "52a59fad-179c-4126-a28e-2de988137a78",
                skillName);
            astronaut.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "30779905-64da-4a23-8176-716d00beb8c8",
                skillName);
            astronaut.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "c89a96a1-aaf3-4413-b355-bcb4646f263e",
                skillName);
            // Set necessary fields
            astronaut.CharacterProgressionData.RequiredSpeed = 0;
            astronaut.CharacterProgressionData.RequiredStrength = 0;
            astronaut.CharacterProgressionData.RequiredWill = 0;
            astronaut.ViewElementDef.DisplayName1 = new LocalizedTextBind("ASTRONAUT", doNotLocalize);
            astronaut.ViewElementDef.Description = new LocalizedTextBind("Gain Jet Pack proficiency", doNotLocalize);
            Sprite astronautIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_CharacterAbility_LaunchMissile-2.png");
            astronaut.ViewElementDef.LargeIcon = astronautIcon;
            astronaut.ViewElementDef.SmallIcon = astronautIcon;
        }
        public static void Create_MartialArtist(bool doNotLocalize = true)
        {
            // create a new array with both melee damage types
            //DamageTypeBaseEffectDef[] meleeDamageTypes = new DamageTypeBaseEffectDef[]
            //{
            //    Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtb => dtb.name.Equals("Slash_StandardDamageTypeEffectDef")),
            //    Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtb => dtb.name.Equals("MeleeBash_StandardDamageTypeEffectDef")),
            //    Repo.GetAllDefs<DamageTypeBaseEffectDef>().FirstOrDefault(dtb => dtb.name.Equals("Bash_StandardDamageTypeEffectDef"))
            //};
            // set all melee weapon defs to the a melee damage type
            // Don't used! It messes up different melee weapons with additional status damages
            //Fix_MeleeWeaponDamageType(meleeDamageTypes);

            string skillName = "BC_MartialArtist_AbilityDef";
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("CloseQuarters_AbilityDef"));
            ApplyStatusAbilityDef martialArtist = Helper.CreateDefFromClone(
                source,
                "1a688a8d-96df-41f4-83ea-c554de05a7a4",
                skillName);
            martialArtist.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "f3115a62-c8a9-4d4f-ab95-78bb1a0ada4e",
                skillName);
            martialArtist.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "b387840f-2407-44cf-a2d8-cc219f4525d7",
                skillName);
            martialArtist.StatusDef = Helper.CreateDefFromClone(
               source.StatusDef,
                "c6daf964-6de7-4fab-8fa5-0a8f598fcaec",
                skillName);
            DamageMultiplierStatusDef maStatus = (DamageMultiplierStatusDef)martialArtist.StatusDef;
            maStatus.Multiplier = 0.9f;
            maStatus.Range = 1.5f;
            martialArtist.CharacterProgressionData.RequiredSpeed = 0;
            martialArtist.CharacterProgressionData.RequiredStrength = 0;
            martialArtist.CharacterProgressionData.RequiredWill = 0;
            martialArtist.ViewElementDef.DisplayName1 = new LocalizedTextBind("MARTIAL ARTIST", doNotLocalize);
            martialArtist.ViewElementDef.Description = new LocalizedTextBind("Reduce incoming damage in melee range by 10%", doNotLocalize);
            Sprite martialArtistIcon = Helper.CreateSpriteFromImageFile("UI_AbilitiesIcon_TentacularBody_MeleeAttackRetaliation-2.png");
            martialArtist.ViewElementDef.LargeIcon = martialArtistIcon;
            martialArtist.ViewElementDef.SmallIcon = martialArtistIcon;
        }
        public static void Fix_MeleeWeaponDamageType(DamageTypeBaseEffectDef[] meleeDamageTypes)
        {
            Logger.Always("Fix_MeleeWeaponDamageType called, fixed melee weapons:", false);
            int count = 0;
            foreach (WeaponDef weapon in Repo.GetAllDefs<WeaponDef>())
            {
                if (weapon.DamagePayload.DamageDeliveryType == DamageDeliveryType.Melee && !meleeDamageTypes.Contains(weapon.DamagePayload.DamageType))
                {
                    Logger.Always("   <" + weapon.name + "> with damage type <" + weapon.DamagePayload.DamageType.name + "> set to <" + meleeDamageTypes[0].name + ">");
                    weapon.DamagePayload.DamageType = meleeDamageTypes[0];
                    count++;
                }
            }
            if (count == 0)
            {
                Logger.Always("  None");
            }
            Logger.Always("------------------------------------------------------------", false);
        }
    }
}