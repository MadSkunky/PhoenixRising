using Base.Defs;
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
        public static void ApplyChanges(DefRepository Repo, SharedData Shared)
        {
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
                    Logger.Always("      Proficiency def name: " + pmad.name, false);
                    if (pmad.ItemTagStatModifications.Length > 0)
                    {
                        for (int i=0; i < pmad.ItemTagStatModifications.Length; i++)
                        {
                            pmad.ItemTagStatModifications[i].EquipmentStatModification.Value -= 0.1f;
                            Logger.Always("           Target item: " + pmad.ItemTagStatModifications[i].ItemTag.name, false);
                            Logger.Always("           Target stat: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.TargetStat, false);
                            Logger.Always("          Modification: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.Modification, false);
                            Logger.Always("                 Value: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.Value, false);
                        }
                    }
                    Logger.Always("----------------------------------------------------", false);
                }
            }
        }
    }
}
