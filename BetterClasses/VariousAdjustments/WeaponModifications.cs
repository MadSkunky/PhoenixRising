using Base.Core;
using Base.Defs;
using Base.UI;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Weapons;
using System;
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
            Change_PriestWeapons();
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
