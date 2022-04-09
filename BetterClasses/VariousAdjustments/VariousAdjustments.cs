﻿using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects;
using Base.Entities.Statuses;
using Base.Levels;
using Base.UI;
using Base.Utils.Maths;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.Entities.DifficultySystem;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Tactical;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Effects;
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


namespace PhoenixRising.BetterClasses.VariousAdjustments
{
    class VariousAdjustments
    {
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            // Change Poison: -50% accuracy and -3 WP per turn
            Change_Poison();
            // Change various bionics
            Change_VariousBionics();
            // Corruptive cloud should not affect pandoran enemies
            Change_CorruptiveCloud();
            // Turrets: Shoot at 1/2 burst but cost 2AP to shoot , maybe reduce armor of all by 10?
            Change_Turrets();
            // Stomp: Gain 50 blast damage
            Change_Stomp();
            // Frenzy: Grant +8 SPD instead of 50% SPD
            Change_Frenzy();
            // Psychici resistance: fix effect and description to: Psychic Scream damage values are halved
            Change_PsychicResistance();
            // Enraged:
            Change_Enraged();
            // Mutoid Worms: limit each worm ability to 5 ammo (worms)
            Change_MutoidWorms();
            // Screaming Head: Mind Control Immunity
            Change_ScreamingHead();
            // Grenades: All grenades are produced instantly
            Change_Grenades();
            // Spider Drones: Armor down to 10 (from 30)
            Change_SpiderDrones();
            // Danchev MG: ER buff to 14 (up from 9)
            Change_DanchevMG();
            // Clarity Head: Replace Mind Control Immunity with Panic Immunity
            Change_ClarityHead();
            // Venom Torso: Add Weapon Tag to Poison Arm 
            Change_VenomTorso();
            // Worms: All worms speed increased to 9 (from 6), worm explosion gets Shred 3
            Change_Worms();
            // Arthron Shield Bearer: Close Quarters Evade
            Change_ArthronShieldBearer();
            // Haven Recruits: Come with Armour and Weapons on all difficulties
            Change_HavenRecruits();
            // Legendary difficulty: Increase deployment numbers as per "More Enemies Mod", settings: 1 / 1.1 / 1.3 / 1.75
            Change_LegendaryDifficulty();
            // Mech Arms: 200 emp damage
            Change_MechArms();
            // Vengeance Torso: Attacks against enemies within 10 tiles deal 10% more damage
            Change_VengeanceTorso();
            // Shadow Legs: Electric Kick replace shock damage with Sonic damage (value 20)
            Change_ShadowLegs();
            // Psychic Ward - fix and description to : Allies within 10 tiles are immune to panic and psychic scream damage
            Change_PsychicWard();
            // Vidar GL - Increase Shred to 20 (from 10), Add Acid 10. Increase AP cost to 2 (from 1)
            Change_VidarGL();
            // Destiny III - Give chance to fumble when non-proficient
            Change_Destiny();
            // Technichian remove MindFragger - Increase range to 2
            Change_TechRemoveFaceHugger();
        }

        private static void Change_Poison()
        {
            DamageOverTimeStatusDef poisonDOT = Repo.GetAllDefs<DamageOverTimeStatusDef>().FirstOrDefault(dot => dot.name.Equals("Poison_DamageOverTimeStatusDef"));
            poisonDOT.Visuals.Description.LocalizationKey = "PR_BC_POISON_STATUS_DESC";
        }
        // Harmony patch for Poison DOT to additionally apply -50% accuracy (Trembling status) and -3 WP per turn
        [HarmonyPatch(typeof(DamageOverTimeStatus), "ApplyEffect")]
        internal static class BC_DamageOverTimeStatus_ApplyEffect_Patch
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static void Postfix(DamageOverTimeStatus __instance)
            {
                if (__instance.DamageOverTimeStatusDef.name.Equals("Poison_DamageOverTimeStatusDef"))
                {
                    TacticalActor base_TacticalActor = (TacticalActor)AccessTools.Property(typeof(TacStatus), "TacticalActor").GetValue(__instance, null);
                    StatusComponent statusComponent = (StatusComponent)AccessTools.Property(typeof(TacStatus), "StatusComponent").GetValue(__instance, null);
                    StatMultiplierStatusDef trembling = Repo.GetAllDefs<StatMultiplierStatusDef>().FirstOrDefault(sms => sms.name.Equals("Trembling_StatusDef"));

                    if (__instance.IntValue <= 0 && base_TacticalActor.Status.HasStatus(trembling))
                    {
                        StatMultiplierStatus status = base_TacticalActor.Status.GetStatus<StatMultiplierStatus>(trembling);
                        status.RequestUnapply(status.StatusComponent);
                        return;
                    }

                    if (!base_TacticalActor.Status.HasStatus(trembling))
                    {
                        _ = base_TacticalActor.Status.ApplyStatus(trembling);
                    }

                    float newWP = Mathf.Max(base_TacticalActor.CharacterStats.WillPoints.Min, base_TacticalActor.CharacterStats.WillPoints - 3.0f);
                    base_TacticalActor.CharacterStats.WillPoints.Set(newWP);
                }
            }
        }

        private static void Change_VariousBionics()
        {
            // Juggernaut Torso & Armadillo Legs: Speed -1 -> 0
            BodyPartAspectDef juggernautTorso = Repo.GetAllDefs<BodyPartAspectDef>().FirstOrDefault(p => p.name.Equals("E_BodyPartAspect [NJ_Jugg_BIO_Torso_BodyPartDef]"));
            BodyPartAspectDef juggernautLegs = Repo.GetAllDefs<BodyPartAspectDef>().FirstOrDefault(p => p.name.Equals("E_BodyPartAspect [NJ_Jugg_BIO_Legs_ItemDef]"));
            juggernautTorso.Speed = juggernautLegs.Speed = 0;

            // Neural Torso: Grants Mounted Weapons and Tech Arms Proficiencies (RoboticArmItem_TagDef)
            TacticalItemDef neuralTorso = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(p => p.name.Equals("NJ_Exo_BIO_Torso_BodyPartDef"));
            AbilityDef mountedWeaponProficiency = Repo.GetAllDefs<AbilityDef>().FirstOrDefault(a1 => a1.name.Equals("MountedWeaponTalent_AbilityDef"));
            if (!neuralTorso.Abilities.Contains(mountedWeaponProficiency))
            {
                neuralTorso.Abilities = neuralTorso.Abilities.AddToArray(mountedWeaponProficiency);
            }

            GameTagDef roboticArmTag = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("RoboticArmItem_TagDef"));
            if (!neuralTorso.Tags.Contains(roboticArmTag))
            {
                neuralTorso.Tags.Add(roboticArmTag);
            }
        }

        public static void Change_CorruptiveCloud()
        {
            ApplyDamageEffectAbilityDef acidCloud = Repo.GetAllDefs<ApplyDamageEffectAbilityDef>().FirstOrDefault(a => a.name.Equals("Acheron_CorrosiveCloud_AbilityDef"));

            //acidCloud.TargetingDataDef.Origin.TargetTags = new GameTagsList()
            //{
            //    Repo.GetAllDefs<GameTagDef>().FirstOrDefault(a => a.name.Equals("Human_TagDef")),
            //    acidCloud.TargetingDataDef.Origin.TargetTags[1],
            //};

            acidCloud.DamagePayload.AoeRadius = 2;
        }
        public static void Change_Turrets()
        {
            int turretAPToUsePerc = 50;
            int turretArmor = 10;
            int turretAutoFireShotCount = 4;

            WeaponDef turret = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("NJ_TechTurretGun_WeaponDef"));
            turret.APToUsePerc = turretAPToUsePerc;
            turret.Armor = turretArmor;
            turret.DamagePayload.AutoFireShotCount = turretAutoFireShotCount;

            WeaponDef prcrTurret = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("NJ_PRCRTechTurretGun_WeaponDef"));
            prcrTurret.APToUsePerc = turretAPToUsePerc;
            prcrTurret.Armor = turretArmor;
            prcrTurret.DamagePayload.AutoFireShotCount = turretAutoFireShotCount;

            WeaponDef laserTurret = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("PX_LaserTechTurretGun_WeaponDef"));
            laserTurret.APToUsePerc = turretAPToUsePerc;
            laserTurret.Armor = turretArmor;
            laserTurret.DamagePayload.AutoFireShotCount = turretAutoFireShotCount;
        }
        public static void Change_Stomp()
        {
            int StompShockValue = 200;
            int StompBlastValue = 50;
            //float range = 5.0f;

            ApplyDamageEffectAbilityDef stomp = Repo.GetAllDefs<ApplyDamageEffectAbilityDef>().FirstOrDefault(p => p.name.Equals("StomperLegs_Stomp_AbilityDef"));
            //ApplyDamageEffectAbilityDef chironStomp = Repo.GetAllDefs<ApplyDamageEffectAbilityDef>().FirstOrDefault(p => p.name.Equals("AreaStun_AbilityDef"));
            //stomp.TargetingDataDef = chironStomp.TargetingDataDef;
            //stomp.TargetingDataDef.Origin.Range = range;
            //stomp.DamagePayload = chironStomp.DamagePayload;
            //stomp.DamagePayload.DamageKeywords.FirstOrDefault(dkp => dkp.DamageKeywordDef == Shared.SharedDamageKeywords.BlastKeyword).Value = StompBlastValue;
            //stomp.DamagePayload.DamageKeywords.FirstOrDefault(dkp => dkp.DamageKeywordDef == Shared.SharedDamageKeywords.ShockKeyword).Value = StompShockValue;
            //stomp.DamagePayload.Range = range;
            stomp.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
                {
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.ShockKeyword, Value = StompShockValue },
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.BlastKeyword, Value = StompBlastValue },
                };
        }
        public static void Change_Frenzy()
        {
            float frenzySpeed = 0.33f;

            FrenzyStatusDef frenzy = Repo.GetAllDefs<FrenzyStatusDef>().FirstOrDefault(p => p.name.Equals("Frenzy_StatusDef"));
            frenzy.SpeedCoefficient = frenzySpeed;
            LocalizedTextBind description = new LocalizedTextBind("", doNotLocalize);
            foreach (ViewElementDef visuals in Repo.GetAllDefs<ViewElementDef>().Where(tav => tav.name.Contains("Frenzy_")))
            {
                visuals.Description.LocalizationKey = visuals.name.Contains("Status") ? "PR_BC_FRENZY_STATUS_DESC" : "PR_BC_FRENZY_DESC";
            }
        }
        public static void Change_PsychicResistance()
        {

        }
        public static void Change_Enraged()
        {

        }
        public static void Change_MutoidWorms()
        {
            int mutoidWormCharges = 5;
            float range = 25.0f;

            WeaponDef mAWorm = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("Mutoid_Arm_AcidWorm_WeaponDef"));
            WeaponDef mFWorm = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("Mutoid_Arm_FireWorm_WeaponDef"));
            WeaponDef mPWorm = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("Mutoid_Arm_PoisonWorm_WeaponDef"));

            mAWorm.ChargesMax = mutoidWormCharges;
            mAWorm.DamagePayload.Range = range;
            mFWorm.ChargesMax = mutoidWormCharges;
            mFWorm.DamagePayload.Range = range;
            mPWorm.ChargesMax = mutoidWormCharges;
            mPWorm.DamagePayload.Range = range;
        }
        public static void Change_ScreamingHead()
        {
          TacticalItemDef screamingHead = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(p => p.name.Equals("AN_Priest_Head03_BodyPartDef"));

            screamingHead.Abilities = new AbilityDef[]
            {
              screamingHead.Abilities[0],
              Repo.GetAllDefs<AbilityDef>().FirstOrDefault(p => p.name.Equals("MindControlImmunity_AbilityDef"))
            };
        }
        public static void Change_Grenades()
        {
            float grenadeManufacturePoints = 0;

            WeaponDef handGrenade = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("PX_HandGrenade_WeaponDef"));
            WeaponDef virophage = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("PX_VirophageGrenade_WeaponDef"));
            WeaponDef emp = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("SY_EMPGrenade_WeaponDef"));
            WeaponDef poison = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("SY_PoisonGrenade_WeaponDef"));
            WeaponDef sonic = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("SY_SonicGrenade_WeaponDef"));
            WeaponDef shredding = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("PX_ShredderGrenade_WeaponDef"));
            WeaponDef acid = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("AN_AcidGrenade_WeaponDef"));
            WeaponDef fire = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("NJ_IncindieryGrenade_WeaponDef"));

            handGrenade.ManufacturePointsCost = grenadeManufacturePoints;
            virophage.ManufacturePointsCost = grenadeManufacturePoints;
            emp.ManufacturePointsCost = grenadeManufacturePoints;  
            poison.ManufacturePointsCost = grenadeManufacturePoints;
            sonic.ManufacturePointsCost = grenadeManufacturePoints;
            shredding.ManufacturePointsCost = grenadeManufacturePoints;
            acid.ManufacturePointsCost = grenadeManufacturePoints;
            fire.ManufacturePointsCost = grenadeManufacturePoints;
        }
        public static void Change_SpiderDrones()
        {
            int spiderDroneArmor = 10;

            TacticalItemDef spiderDrone = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(p => p.name.Equals("SpiderDrone_Torso_BodyPartDef"));
            spiderDrone.Armor = spiderDroneArmor;
        }
        public static void Change_DanchevMG()
        {
            float danchevMGSpreadDegrees = 2.86240523f;

            WeaponDef danchevMG = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("PX_PoisonMachineGun_WeaponDef"));
            danchevMG.SpreadDegrees = danchevMGSpreadDegrees;
        }
        public static void Change_ClarityHead()
        {
            //TacticalItemDef clarityHead = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(p => p.name.Equals("NJ_Jugg_BIO_Helmet_BodyPartDef"));
            //DamageMultiplierStatusDef panicImmunityStatus = Repo.GetAllDefs<DamageMultiplierStatusDef>().FirstOrDefault(p => p.name.Equals("PanicImmunity_StatusDef"));
            //
            //string skillName = "BC_PanicImmunity_AbilityDef";
            //ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MindControlImmunity_AbilityDef"));
            //ApplyStatusAbilityDef pI = Helper.CreateDefFromClone(
            //    source,
            //    "f2c51f91-fd5b-4f6f-bc19-cf6dfef831ba",
            //    skillName);
            //pI.CharacterProgressionData = Helper.CreateDefFromClone(
            //    source.CharacterProgressionData,
            //    "528863c2-92ca-4690-84be-fb3600316439",
            //    skillName);
            //pI.ViewElementDef = Helper.CreateDefFromClone(
            //    source.ViewElementDef,
            //    "59b36dfb-6b89-4771-8bdc-454bbe4e08e0",
            //    skillName);
            //
            //pI.StatusDef = panicImmunityStatus;
            //clarityHead.Abilities = new AbilityDef[]
            //{
            //    Repo.GetAllDefs<AbilityDef>().FirstOrDefault(p => p.name.Equals("BC_PanicImmunity_AbilityDef")),
            //    clarityHead.Abilities[1],
            //};
            //
            //pI.ViewElementDef.DisplayName1 = new LocalizedTextBind("PANIC IMMUNITY", doNotLocalize);
            //pI.ViewElementDef.Description = new LocalizedTextBind("Immune to panic", doNotLocalize);
        }
        public static void Change_VenomTorso()
        {
            WeaponDef venomTorso = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("AN_Berserker_Shooter_LeftArm_WeaponDef"));

            venomTorso.Tags = new GameTagsList()
            {
                venomTorso.Tags[0],
                venomTorso.Tags[1],
                venomTorso.Tags[2],
                Repo.GetAllDefs<GameTagDef>().FirstOrDefault(p => p.name.Equals("GunWeapon_TagDef"))
            };
        }
        public static void Change_Worms()
        {
            int wormSpeed = 9;
            int wormShredDamage = 3;
            int aWormAcidDamage = 30;
            int aWormBlastDamage = 10;
            int fWormFireDamage = 40;
            int pWormBlastDamage = 25;
            int pWormPoisonDamage = 50;

            BodyPartAspectDef acidWorm = Repo.GetAllDefs<BodyPartAspectDef>().FirstOrDefault(a => a.name.Equals("E_BodyPartAspect [Acidworm_Torso_BodyPartDef]"));
            BodyPartAspectDef fireWorm = Repo.GetAllDefs<BodyPartAspectDef>().FirstOrDefault(a => a.name.Equals("E_BodyPartAspect [Fireworm_Torso_BodyPartDef]"));
            BodyPartAspectDef poisonWorm = Repo.GetAllDefs<BodyPartAspectDef>().FirstOrDefault(a => a.name.Equals("E_BodyPartAspect [Poisonworm_Torso_BodyPartDef]"));
            ApplyDamageEffectAbilityDef aWormDamage = Repo.GetAllDefs<ApplyDamageEffectAbilityDef>().FirstOrDefault(a => a.name.Equals("AcidwormExplode_AbilityDef"));
            ApplyDamageEffectAbilityDef fWormDamage = Repo.GetAllDefs<ApplyDamageEffectAbilityDef>().FirstOrDefault(a => a.name.Equals("FirewormExplode_AbilityDef"));
            ApplyDamageEffectAbilityDef pWormDamage = Repo.GetAllDefs<ApplyDamageEffectAbilityDef>().FirstOrDefault(a => a.name.Equals("PoisonwormExplode_AbilityDef"));

            acidWorm.Speed = wormSpeed;
            fireWorm.Speed = wormSpeed;
            poisonWorm.Speed = wormSpeed;

            aWormDamage.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
                {
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.BlastKeyword, Value = aWormBlastDamage },
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.AcidKeyword, Value = aWormAcidDamage },
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.ShreddingKeyword, Value = wormShredDamage },
                };

             fWormDamage.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
                {
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.BurningKeyword, Value = fWormFireDamage },
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.ShreddingKeyword, Value = wormShredDamage },
                };

             pWormDamage.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
                {
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.BlastKeyword, Value = pWormBlastDamage },
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.PoisonousKeyword, Value = pWormPoisonDamage },
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.ShreddingKeyword, Value = wormShredDamage },
                };
        }
        public static void Change_ArthronShieldBearer()
        {
        }
        public static void Change_HavenRecruits()
        {
            bool hasArmor = true;
            bool hasWeapon = true;

            GameDifficultyLevelDef easy = Repo.GetAllDefs<GameDifficultyLevelDef>().FirstOrDefault(a => a.name.Equals("Easy_GameDifficultyLevelDef"));
            GameDifficultyLevelDef standard = Repo.GetAllDefs<GameDifficultyLevelDef>().FirstOrDefault(a => a.name.Equals("Standard_GameDifficultyLevelDef"));
            GameDifficultyLevelDef hard = Repo.GetAllDefs<GameDifficultyLevelDef>().FirstOrDefault(a => a.name.Equals("Hard_GameDifficultyLevelDef"));
            GameDifficultyLevelDef veryhard = Repo.GetAllDefs<GameDifficultyLevelDef>().FirstOrDefault(a => a.name.Equals("VeryHard_GameDifficultyLevelDef"));

            easy.RecruitsGenerationParams.HasArmor = hasArmor;
            easy.RecruitsGenerationParams.HasWeapons = hasWeapon;
            standard.RecruitsGenerationParams.HasArmor = hasArmor;
            standard.RecruitsGenerationParams.HasWeapons = hasWeapon;
            hard.RecruitsGenerationParams.HasArmor= hasArmor;
            hard.RecruitsGenerationParams.HasWeapons= hasWeapon;
            veryhard.RecruitsGenerationParams.HasArmor = hasArmor;
            veryhard.RecruitsGenerationParams.HasWeapons = hasWeapon;
        }
        public static void Change_LegendaryDifficulty()
        {
            /*
            float low = 1.1f;
            float medium = 1.3f;
            float high = 1.6f;
            float extreme = 2;

            DynamicDifficultySettingsDef dDSettings = Repo.GetAllDefs<DynamicDifficultySettingsDef>().FirstOrDefault(a => a.name.Equals("DynamicDifficultySettingsDef"));

            dDSettings.ThreatLevels[0].ThreatLevelModifier = low;
            dDSettings.ThreatLevels[1].ThreatLevelModifier= medium;
            dDSettings.ThreatLevels[2].ThreatLevelModifier= high;
            dDSettings.ThreatLevels[3].ThreatLevelModifier= extreme;
            */
        }
        public static void Change_MechArms()
        {
            int mechArmsShockDamage = 180;
            int mechArmsEMPDamage = 200;
            int usesPerTurn = 1;

            WeaponDef mechArms = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("NJ_Technician_MechArms_WeaponDef"));
            DamageKeywordDef emp = Repo.GetAllDefs<DamageKeywordDef>().FirstOrDefault(p => p.name.Equals("EMP_DamageKeywordDataDef")); 
            mechArms.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
            {
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.ShockKeyword, Value = mechArmsShockDamage },
                new DamageKeywordPair{DamageKeywordDef = emp, Value = mechArmsEMPDamage }
            };
            ShootAbilityDef techArmStrike = Repo.GetAllDefs<ShootAbilityDef>().FirstOrDefault(s => s.name.Equals("TechnicianStrike_ShootAbilityDef"));
            techArmStrike.UsesPerTurn = usesPerTurn;
        }
        public static void Change_VengeanceTorso()
        {
            TacticalItemDef vTorso = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(p => p.name.Equals("SY_Shinobi_BIO_Torso_BodyPartDef"));

            vTorso.Abilities[1] = Repo.GetAllDefs<AbilityDef>().FirstOrDefault(p => p.name.Equals("BattleFocus_AbilityDef"));
        }
        public static void Change_ShadowLegs()
        {
            int shadowLegsSonicDamage = 20;

            BashAbilityDef shadowLegs = Repo.GetAllDefs<BashAbilityDef>().FirstOrDefault(p => p.name.Equals("ElectricKick_AbilityDef"));

            shadowLegs.DamagePayload.DamageKeywords[0].DamageKeywordDef = Shared.SharedDamageKeywords.SonicKeyword;
            shadowLegs.DamagePayload.DamageKeywords[0].Value = shadowLegsSonicDamage;
        }
        public static void Change_PsychicWard()
        {
            ApplyStatusAbilityDef psychicWard = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("PsychicWard_AbilityDef"));
            psychicWard.ViewElementDef.Description = new LocalizedTextBind("Allies within 10 tiles are immune to panic and psychic scream damage", doNotLocalize);
        }
        public static void Change_VidarGL()
        {
            int vGLNormal = 50;
            int vGLShred = 20;
            int vGLAcid = 10;
            int vGlAPCost = 50;

            WeaponDef vGL = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("FS_AssaultGrenadeLauncher_WeaponDef"));

            vGL.DamagePayload.DamageKeywords = new List<DamageKeywordPair>
            {
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.BlastKeyword, Value = vGLNormal },
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.ShreddingKeyword, Value = vGLShred },
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.AcidKeyword, Value = vGLAcid },
            };

            vGL.APToUsePerc = vGlAPCost;
            
        }
        public static void Change_Destiny()
        {
            WeaponDef destiny3 = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("PX_LaserArrayPack_WeaponDef"));
            destiny3.FumblePerc = 50;          
        }
        public static void Change_TechRemoveFaceHugger()
        {
            //int tRFHRange = 2;
            //RemoveFacehuggerAbilityDef tRFH = Repo.GetAllDefs<RemoveFacehuggerAbilityDef>().FirstOrDefault(p => p.name.Equals("TechnicianRemoveFacehugger_AbilityDef"));
            //tRFH.TargetingDataDef.Origin.Range = tRFHRange;
        }
    }
}
