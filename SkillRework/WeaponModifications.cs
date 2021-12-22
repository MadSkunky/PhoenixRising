using Base.Core;
using Base.Defs;
using Base.UI;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities.Abilities;
using System;
using System.Linq;

namespace PhoenixRising.SkillRework
{
    class WeaponModifications
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = SkillReworkMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void ApplyChanges()
        {
            try
            {
                // Get config setting for localized texts.
                bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

                // Short-burst skill for Assaults with accuracy buff, base from standard shoot ability, icon like Trooper or AssaultRifleTalent, maybe inverse
                ShootAbilityDef weaponShoot = Repo.GetAllDefs<ShootAbilityDef>().FirstOrDefault(s => s.name.Equals("Weapon_ShootAbilityDef"));

                // Base shooting abilities for burst weapons
                string skillName = "SingleBurst_ShootAbilityDef";
                ShootAbilityDef singleBurst = SkillModifications.CreateDefFromClone(
                    weaponShoot,
                    "f87aa4d0-acfc-4deb-b617-906a1db1618f",
                    skillName);
                singleBurst.ActionPointCost = 0.25f;
                singleBurst.ExecutionsCount = 1;
                TacticalAbilityViewElementDef sbVisuals = SkillModifications.CreateDefFromClone(
                    weaponShoot.ViewElementDef,
                    "5051f147-a231-4015-ba82-d7f6749bb754",
                    skillName);
                sbVisuals.DisplayName1 = new LocalizedTextBind("FIRE SHORT BURST", doNotLocalize);
                sbVisuals.Description = new LocalizedTextBind("Shoot a short burst at target enemy or target point", doNotLocalize);
                singleBurst.ViewElementDef = sbVisuals;

                skillName = "DoubleBurst_ShootAbilityDef";
                ShootAbilityDef doubleBurst = SkillModifications.CreateDefFromClone(
                    weaponShoot,
                    "51e33db7-6bec-4144-8f9f-d23dc25e3e67",
                    skillName);
                doubleBurst.ActionPointCost = 0.5f;
                doubleBurst.ExecutionsCount = 2;
                TacticalAbilityViewElementDef dbVisuals = SkillModifications.CreateDefFromClone(
                    weaponShoot.ViewElementDef,
                    "a7049213-abd8-445d-a643-fffd7439d1cc",
                    skillName);
                dbVisuals.DisplayName1 = new LocalizedTextBind("FIRE NORMAL BURST", doNotLocalize);
                dbVisuals.Description = new LocalizedTextBind("Shoot a normal burst at target enemy or target point", doNotLocalize);
                doubleBurst.ViewElementDef = dbVisuals;

                skillName = "TripleBurst_ShootAbilityDef";
                ShootAbilityDef tripleBurst = SkillModifications.CreateDefFromClone(
                    weaponShoot,
                    "5548762b-61ae-45c8-ae09-ee8163b423c3",
                    skillName);
                tripleBurst.ActionPointCost = 0.75f;
                tripleBurst.ExecutionsCount = 3;
                TacticalAbilityViewElementDef tbVisuals = SkillModifications.CreateDefFromClone(
                    weaponShoot.ViewElementDef,
                    "0e5a2f1b-e19e-4715-a458-a34b0c0e29d8",
                    skillName);
                tbVisuals.DisplayName1 = new LocalizedTextBind("FIRE LONG BURST", doNotLocalize);
                tbVisuals.Description = new LocalizedTextBind("Shoot a long burst at target enemy or target point", doNotLocalize);
                tripleBurst.ViewElementDef = tbVisuals;

                Logger.Debug($"{singleBurst.name}: {singleBurst.ViewElementDef.DisplayName1.LocalizeEnglish()}, description: {singleBurst.ViewElementDef.Description.LocalizeEnglish()}", false);
                Logger.Debug($"{doubleBurst.name}: {doubleBurst.ViewElementDef.DisplayName1.LocalizeEnglish()}, description: {doubleBurst.ViewElementDef.Description.LocalizeEnglish()}", false);
                Logger.Debug($"{tripleBurst.name}: {tripleBurst.ViewElementDef.DisplayName1.LocalizeEnglish()}, description: {tripleBurst.ViewElementDef.Description.LocalizeEnglish()}", false);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
