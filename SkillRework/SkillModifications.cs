using Base.Defs;
using Base.UI;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.SkillRework
{
    class SkillModifications
    {
        public static void ApplyChanges(DefRepository Repo, SharedData Shared, Settings Config)
        {
            Dictionary<string, Dictionary<string, string>> textDict = Helper.NotLocalizedTextMap;
            foreach (PassiveModifierAbilityDef pmad in Repo.GetAllDefs<PassiveModifierAbilityDef>())
            {
                if (pmad.CharacterProgressionData != null && pmad.name.Contains("Talent"))
                {
                    // Assault rifle proficiency
                    if (pmad.name.Contains("Assault"))
                    {
                        GameTagDef ARtagDef = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gtd => gtd.name.Contains("AssaultRifleItem_TagDef"));
                        pmad.ItemTagStatModifications[0].ItemTag = ARtagDef;
                    }
                    // Change descrition text, not localized (currently)
                    string newText = Helper.NotLocalizedTextMap[pmad.ViewElementDef.name][ViewElement.Description];
                    Helper.ChangeUItext(ref pmad.ViewElementDef.Description, newText);

                    Logger.Debug("Proficiency def name: " + pmad.name);
                    Logger.Debug("Viewelement name:     " + pmad.ViewElementDef.name);
                    Logger.Debug("Display1 name:        " + pmad.ViewElementDef.DisplayName1.Localize());
                    Logger.Debug("Description:          " + pmad.ViewElementDef.Description.Localize());
                    float configMod = -0.1f + Config.BuffsForAdditionalProficiency[Proficiency.Buff]; // first -0.1 to normalise to 0.0 (proficiency perks all have +0.1 buff)
                    if (pmad.ItemTagStatModifications.Length > 0)
                    {
                        for (int i=0; i < pmad.ItemTagStatModifications.Length; i++)
                        {
                            pmad.ItemTagStatModifications[i].EquipmentStatModification.Value += configMod;
                            Logger.Debug("  Target item: " + pmad.ItemTagStatModifications[i].ItemTag.name, false);
                            Logger.Debug("  Target stat: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.TargetStat, false);
                            Logger.Debug(" Modification: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.Modification, false);
                            Logger.Debug("        Value: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.Value, false);
                        }
                    }
                    Logger.Debug("----------------------------------------------------", false);
                }
            }
        }
    }
}
