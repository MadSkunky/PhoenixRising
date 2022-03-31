using Base.Core;
using Base.Defs;
using Base.UI;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoenixRising.BetterClasses.VariousAdjustments
{
    class WeaponModifications
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            Change_Iconoclast();
            Change_NergalsWrath();
            Change_Crossbows();
            Change_PriestWeapons();
        }

        private static void Change_Iconoclast()
        {
            WeaponDef Iconoclast = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(ec => ec.name.Equals("AN_Shotgun_WeaponDef"));
            Iconoclast.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
            {
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.DamageKeyword,
                    Value = 30
                }
            };
        }

        private static void Change_NergalsWrath()
        {
            WeaponDef NergalsWrath = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(ec => ec.name.Equals("AN_HandCannon_WeaponDef"));
            NergalsWrath.APToUsePerc = 25;
            NergalsWrath.DamagePayload.DamageKeywords = new List<DamageKeywordPair>()
            {
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.DamageKeyword,
                    Value = 50
                },
                new DamageKeywordPair()
                {
                    DamageKeywordDef = Shared.SharedDamageKeywords.ShreddingKeyword,
                    Value = 5
                }
            };
        }

        private static void Change_Crossbows()
        {
            WeaponDef ErosCrb = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(ec => ec.name.Equals("SY_Crossbow_WeaponDef"));
            WeaponDef BonusErosCrb = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(ec => ec.name.Equals("SY_Crossbow_Bonus_WeaponDef"));
            ItemDef ErosCrb_Ammo = Repo.GetAllDefs<ItemDef>().FirstOrDefault(ec => ec.name.Equals("SY_Crossbow_AmmoClip_ItemDef"));
            WeaponDef PsycheCrb = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(ec => ec.name.Equals("SY_Venombolt_WeaponDef"));
            ItemDef PsycheCrb_Ammo = Repo.GetAllDefs<ItemDef>().FirstOrDefault(ec => ec.name.Equals("SY_Venombolt_AmmoClip_ItemDef"));
            ErosCrb.ChargesMax = Config.BaseCrossbow_Ammo;
            BonusErosCrb.ChargesMax = Config.BaseCrossbow_Ammo;
            ErosCrb_Ammo.ChargesMax = Config.BaseCrossbow_Ammo;
            PsycheCrb.ChargesMax = Config.VenomCrossbow_Ammo;
            PsycheCrb_Ammo.ChargesMax = Config.VenomCrossbow_Ammo;
        }

        public static void Change_PriestWeapons()
        {
            int redeemerViral = 4;
            int subjectorViral = 8;

            WeaponDef redeemer = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("AN_Redemptor_WeaponDef"));
            WeaponDef subjector = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("AN_Subjector_WeaponDef"));

            redeemer.DamagePayload.DamageKeywords[2].Value = redeemerViral;
            subjector.DamagePayload.DamageKeywords[2].Value = subjectorViral;
        }
    }
}
