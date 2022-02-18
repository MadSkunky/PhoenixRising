using Base.Core;
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
    class Adjustments
    {
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
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
            // MindFragger: Gains Acid explosion (acid value = 10) when attached 
            Change_MindFragger();
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
        }
        public static void Change_Turrets()
        {
            int turretAPToUsePerc = 50;
            int turretArmor = 10;
            int turretAutoFireShotCount = 4;

           WeaponDef turret = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("NJ_TechTurretGun_WeaponDef"));
           turret.APToUsePerc = turretAPToUsePerc;
           turret.Armor = turretArmor;
           turret.DamagePayload.AutoFireShotCount = turretAutoFireShotCount;

            WeaponDef prcrTurret = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("NJ_PRCRTechTurretGun_WeaponDef"));
            prcrTurret.APToUsePerc = turretAPToUsePerc;
            prcrTurret.Armor = turretArmor;
            prcrTurret.DamagePayload.AutoFireShotCount = turretAutoFireShotCount;

            WeaponDef laserTurret = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("PX_LaserTechTurretGun_WeaponDef"));
            laserTurret.APToUsePerc = turretAPToUsePerc;
            laserTurret.Armor = turretArmor;
            laserTurret.DamagePayload.AutoFireShotCount = turretAutoFireShotCount;
        }
        public static void Change_Stomp()
        {
            int StompShockValue = 200;
            int StompBlastValue = 50;

            ApplyDamageEffectAbilityDef stomp = Repo.GetAllDefs<ApplyDamageEffectAbilityDef>().FirstOrDefault(p => p.name.Contains("StomperLegs_Stomp_AbilityDef"));
            
              stomp.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
                {
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.ShockKeyword, Value = StompShockValue },
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.BlastKeyword, Value = StompBlastValue },
                };
        }
        public static void Change_Frenzy()
        {
            float frenzySpeed = 0.25f;

            FrenzyStatusDef frenzy = Repo.GetAllDefs<FrenzyStatusDef>().FirstOrDefault(p => p.name.Contains("Frenzy_StatusDef"));
            frenzy.SpeedCoefficient = frenzySpeed;
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

            WeaponDef mAWorm = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("Mutoid_Arm_AcidWorm_WeaponDef"));
            WeaponDef mFWorm = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("Mutoid_Arm_FireWorm_WeaponDef"));
            WeaponDef mPWorm = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("Mutoid_Arm_PoisonWorm_WeaponDef"));

            mAWorm.ChargesMax = mutoidWormCharges;
            mFWorm.ChargesMax = mutoidWormCharges;
            mPWorm.ChargesMax = mutoidWormCharges;
        }
        public static void Change_ScreamingHead()
        {
          TacticalItemDef screamingHead = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(p => p.name.Contains("AN_Priest_Head03_BodyPartDef"));

            screamingHead.Abilities = new AbilityDef[]
            {
              screamingHead.Abilities[0],
              Repo.GetAllDefs<AbilityDef>().FirstOrDefault(p => p.name.Contains("MindControlImmunity_AbilityDef"))
            };
        }
        public static void Change_Grenades()
        {
            float grenadeManufacturePoints = 0;

            WeaponDef handGrenade = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("PX_HandGrenade_WeaponDef"));
            WeaponDef virophage = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("PX_VirophageGrenade_WeaponDef"));
            WeaponDef emp = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("SY_EMPGrenade_WeaponDef"));
            WeaponDef poison = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("SY_PoisonGrenade_WeaponDef"));
            WeaponDef sonic = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("SY_SonicGrenade_WeaponDef"));
            WeaponDef shredding = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("PX_ShredderGrenade_WeaponDef"));
            WeaponDef acid = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("AN_AcidGrenade_WeaponDef"));
            WeaponDef fire = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("NJ_IncindieryGrenade_WeaponDef"));

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

            TacticalItemDef spiderDrone = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(p => p.name.Contains("SpiderDrone_Torso_BodyPartDef"));
            spiderDrone.Armor = spiderDroneArmor;
        }
        public static void Change_DanchevMG()
        {
            float danchevMGSpreadDegrees = 2.86240523f;

            WeaponDef danchevMG = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("PX_PoisonMachineGun_WeaponDef"));
            danchevMG.SpreadDegrees = danchevMGSpreadDegrees;
        }
        public static void Change_ClarityHead()
        {
            TacticalItemDef clarityHead = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(p => p.name.Contains("NJ_Jugg_BIO_Helmet_BodyPartDef"));
            DamageMultiplierStatusDef panicImmunity = Repo.GetAllDefs<DamageMultiplierStatusDef>().FirstOrDefault(p => p.name.Contains("PanicImmunity_StatusDef"));
            ApplyStatusAbilityDef clarityHeadStatus = (ApplyStatusAbilityDef)clarityHead.Abilities[0];

            clarityHeadStatus.StatusDef = panicImmunity;
        }
        public static void Change_VenomTorso()
        {

        }
        public static void Change_MindFragger()
        {

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

        }
        public static void Change_LegendaryDifficulty()
        {

        }
        public static void Change_MechArms()
        {
            int mechArmsShockDamage = 0;
            int mechArmsEMPDamage = 200;

            WeaponDef mechArms = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Contains("NJ_Technician_MechArms_WeaponDef"));
            DamageKeywordDef emp = Repo.GetAllDefs<DamageKeywordDef>().FirstOrDefault(p => p.name.Contains("EMP_DamageKeywordDataDef")); 
            mechArms.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
                {
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.ShockKeyword, Value = mechArmsShockDamage },
                new DamageKeywordPair{DamageKeywordDef = emp, Value = mechArmsEMPDamage },
                };
        }
    }
}