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
            // Vengeance Torso: Attacks against enemies within 10 tiles deal 10% more damage
            Create_VengeanceTorso();
            // Shadow Legs: Electric Kick replace shock damage with Sonic damage (value 20)
            Change_ShadowLegs();
            // Psychic Ward - fix and description to : Allies within 10 tiles are immune to panic and psychic scream damage
            Change_PsychicImmunity();
            // Vidar GL - Increase Shred to 20 (from 10), Add Acid 10. Increase AP cost to 2 (from 1)
            Change_VidarGL();
            // Destiny III - Give chance to fumble when non-proficient
            Change_Destiny();
            // Technichian remove MindFragger - Increase range to 2
            Change_TechRemoveFaceHugger();
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

            ApplyDamageEffectAbilityDef stomp = Repo.GetAllDefs<ApplyDamageEffectAbilityDef>().FirstOrDefault(p => p.name.Equals("StomperLegs_Stomp_AbilityDef"));

            stomp.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
                {
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.ShockKeyword, Value = StompShockValue },
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.BlastKeyword, Value = StompBlastValue },
                };
        }
        public static void Change_Frenzy()
        {
            float frenzySpeed = 0.35f;

            FrenzyStatusDef frenzy = Repo.GetAllDefs<FrenzyStatusDef>().FirstOrDefault(p => p.name.Equals("Frenzy_StatusDef"));
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

            WeaponDef mAWorm = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("Mutoid_Arm_AcidWorm_WeaponDef"));
            WeaponDef mFWorm = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("Mutoid_Arm_FireWorm_WeaponDef"));
            WeaponDef mPWorm = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("Mutoid_Arm_PoisonWorm_WeaponDef"));

            mAWorm.ChargesMax = mutoidWormCharges;
            mFWorm.ChargesMax = mutoidWormCharges;
            mPWorm.ChargesMax = mutoidWormCharges;
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
            TacticalItemDef clarityHead = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(p => p.name.Equals("NJ_Jugg_BIO_Helmet_BodyPartDef"));
            DamageMultiplierStatusDef panicImmunityStatus = Repo.GetAllDefs<DamageMultiplierStatusDef>().FirstOrDefault(p => p.name.Equals("PanicImmunity_StatusDef"));

            string skillName = "BC_PanicImmunity_AbilityDef";
            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MindControlImmunity_AbilityDef"));
            ApplyStatusAbilityDef pI = Helper.CreateDefFromClone(
                source,
                "f2c51f91-fd5b-4f6f-bc19-cf6dfef831ba",
                skillName);
            pI.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "528863c2-92ca-4690-84be-fb3600316439",
                skillName);
            pI.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "59b36dfb-6b89-4771-8bdc-454bbe4e08e0",
                skillName);

            pI.StatusDef = panicImmunityStatus;
            clarityHead.Abilities = new AbilityDef[]
            {
                Repo.GetAllDefs<AbilityDef>().FirstOrDefault(p => p.name.Equals("BC_PanicImmunity_AbilityDef")),
                clarityHead.Abilities[1],
            };

            pI.ViewElementDef.DisplayName1 = new LocalizedTextBind("PANIC IMMUNITY", doNotLocalize);
            pI.ViewElementDef.Description = new LocalizedTextBind("Immune to panic", doNotLocalize);
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
        public static void Change_MindFragger()
        {
            int faceHuggerBlastDamage = 1;
            int faceHuggerAcidDamage = 5;
            int faceHuggerAOERadius = 2;

            TacticalItemDef faceHugger = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(p => p.name.Equals("Facehugger_Head_BodyPartDef"));

            string skillName = "BC_SwarmerAcidExplosion_Die_AbilityDef";
            RagdollDieAbilityDef source = Repo.GetAllDefs<RagdollDieAbilityDef>().FirstOrDefault(p => p.name.Equals("SwarmerAcidExplosion_Die_AbilityDef"));
            RagdollDieAbilityDef sAE = Helper.CreateDefFromClone(
                source,
                "1137345a-a18d-4800-b52e-b15d49f4dabf",
                skillName);
            sAE.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "10729876-f764-41b5-9b4e-c8cb98dca771",
                skillName);
            DamagePayloadEffectDef sAEEffect = Helper.CreateDefFromClone(
                Repo.GetAllDefs<DamagePayloadEffectDef>().FirstOrDefault(p => p.name.Equals("E_Element0 [SwarmerAcidExplosion_Die_AbilityDef]")),
                "ac9cd527-72d4-42d2-af32-5efbdf32812e",
                "E_Element0 [BC_SwarmerAcidExplosion_Die_AbilityDef]");

            sAE.DeathEffect = sAEEffect;
            sAEEffect.DamagePayload.DamageKeywords[0].Value = faceHuggerBlastDamage;
            sAEEffect.DamagePayload.DamageKeywords[1].Value = faceHuggerAcidDamage;
            sAEEffect.DamagePayload.AoeRadius = faceHuggerAOERadius;

            sAE.ViewElementDef.DisplayName1 = new LocalizedTextBind("ACID EXPLOSION", doNotLocalize);
            sAE.ViewElementDef.Description = new LocalizedTextBind("Upon death, the mind fragger bursts in an acid explosion damaging nearby targets", doNotLocalize);

            faceHugger.Abilities = new AbilityDef[]
            {
                faceHugger.Abilities[0],
                sAE,
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
            /*
            TacticalItemDef shieldBearer = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("Crabman_LeftArm_Shield_BodyPartDef"));
            shieldBearer.Abilities = new AbilityDef[]
            {
                Repo.GetAllDefs<AbilityDef>().FirstOrDefault(a => a.name.Equals("CloseQuarters_AbilityDef")),
            };
            */
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
            int mechArmsShockDamage = 0;
            int mechArmsEMPDamage = 200;

            WeaponDef mechArms = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(p => p.name.Equals("NJ_Technician_MechArms_WeaponDef"));
            DamageKeywordDef emp = Repo.GetAllDefs<DamageKeywordDef>().FirstOrDefault(p => p.name.Equals("EMP_DamageKeywordDataDef")); 
            mechArms.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
                {
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.ShockKeyword, Value = mechArmsShockDamage },
                new DamageKeywordPair{DamageKeywordDef = emp, Value = mechArmsEMPDamage },
                };
        }
        public static void Create_VengeanceTorso()
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
        public static void Change_PsychicImmunity()
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
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.DamageKeyword, Value = vGLNormal },
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
            int tRFHRange = 2;
            RemoveFacehuggerAbilityDef tRFH = Repo.GetAllDefs<RemoveFacehuggerAbilityDef>().FirstOrDefault(p => p.name.Equals("TechnicianRemoveFacehugger_AbilityDef"));
            tRFH.TargetingDataDef.Origin.Range = tRFHRange;
        }
    }
}
