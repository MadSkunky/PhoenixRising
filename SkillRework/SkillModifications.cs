using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Statuses;
using Base.UI;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Equipments;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PhoenixRising.SkillRework
{
    class SkillModifications
    {
        public static void ApplyChanges(DefRepository Repo, Settings Config)
        {
            try
            {
                // Get config setting for localized texts.
                bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

                foreach (PassiveModifierAbilityDef pmad in Repo.GetAllDefs<PassiveModifierAbilityDef>())
                {
                    if (pmad.CharacterProgressionData != null && pmad.name.Contains("Talent"))
                    {
                        // Assault rifle proficiency fix, was set to shotguns
                        if (pmad.name.Contains("Assault"))
                        {
                            GameTagDef ARtagDef = Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gtd => gtd.name.Contains("AssaultRifleItem_TagDef"));
                            pmad.ItemTagStatModifications[0].ItemTag = ARtagDef;
                        }

                        // Change descrition text, not localized (currently), old one mentions fixed buffs that are taken away or set differently by this mod
                        string newText = Helper.NotLocalizedTextMap[pmad.ViewElementDef.name][ViewElement.Description];
                        pmad.ViewElementDef.Description = new LocalizedTextBind(newText, doNotLocalize);

                        Logger.Debug("Proficiency def name: " + pmad.name);
                        Logger.Debug("Viewelement name:     " + pmad.ViewElementDef.name);
                        Logger.Debug("Display1 name:        " + pmad.ViewElementDef.DisplayName1.Localize());
                        Logger.Debug("Description:          " + pmad.ViewElementDef.Description.Localize());

                        // Get modification from config, but first -0.1 to normalise to 0.0 (proficiency perks are all set to +0.1 buff)
                        float newStatModification = -0.1f + Config.BuffsForAdditionalProficiency[Proficiency.Buff];
                        // Loop through all subsequent item stat modifications
                        if (pmad.ItemTagStatModifications.Length > 0)
                        {
                            for (int i=0; i < pmad.ItemTagStatModifications.Length; i++)
                            {
                                pmad.ItemTagStatModifications[i].EquipmentStatModification.Value += newStatModification;

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
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    
        public static void CreateSkill(DefRepository Repo, Settings Config)
        {
            // Get config setting for localized texts.
            bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

            // Loading texture from file and create a usable Sprite
            string filePath = Path.Combine(SkillReworkMain.TexturesDirectory, "Pepe.png");
            int width = 128;
            int height = 128;
            Sprite newSprite = null;
            Texture2D texture = null;
            if (File.Exists(filePath) && Helper.LoadTexture2DfromFile(ref texture, filePath, width, height))
            {
                newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.0f, 0.0f));
            }

            Logger.Debug("-----------------------------------------------------------------------");
            Logger.Debug("Repo PassiveModifierAbilityDef count before: " + Repo.GetAllDefs<PassiveModifierAbilityDef>().Count());
            string abilityName = "FirstAdded_AbilityDef";
            PassiveModifierAbilityDef sourceAbility = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("Devoted_AbilityDef"));
            if (sourceAbility != null)
            {
                // TODO: Don't use automatically created GUIDs, these will crash the game when loading a saved after restarting the game
                // Should use Repo.CreateRuntimeDef(sourceAbility, GUID); with fixed GUID per skill and subparts, saved on file or in code.
                PassiveModifierAbilityDef addedAbility = Repo.CreateRuntimeDef<PassiveModifierAbilityDef>(sourceAbility);
                addedAbility.ViewElementDef = Repo.CreateRuntimeDef<TacticalAbilityViewElementDef>(sourceAbility.ViewElementDef);
                addedAbility.CharacterProgressionData = Repo.CreateRuntimeDef<AbilityCharacterProgressionDef>(sourceAbility.CharacterProgressionData);
                addedAbility.name = abilityName;
                addedAbility.StatModifications = new ItemStatModification[] {
                        new ItemStatModification {
                            TargetStat = StatModificationTarget.Speed,
                            Modification = StatModificationType.Add,
                            Value = 1
                        } };
                addedAbility.ViewElementDef.name = "E_ViewElement [" + abilityName + "]";
                addedAbility.ViewElementDef.DisplayName1 = new LocalizedTextBind("MadSkunky GottaGoFast", doNotLocalize);
                addedAbility.ViewElementDef.Description = new LocalizedTextBind("Additional +1 to Speed", doNotLocalize);
                addedAbility.ViewElementDef.LargeIcon = newSprite;
                addedAbility.ViewElementDef.SmallIcon = newSprite;
                addedAbility.CharacterProgressionData.name = "E_CharacterProgressionData [" + abilityName + "]";
                Logger.Debug("------------------------- Source ability ---------------------------");
                Logger.Debug("Guid: " + sourceAbility.Guid);
                Logger.Debug("Name: " + sourceAbility.name);
                Logger.Debug("Modification target: " + sourceAbility.StatModifications[0].TargetStat);
                Logger.Debug("Modification modtype: " + sourceAbility.StatModifications[0].Modification);
                Logger.Debug("Modification value: " + sourceAbility.StatModifications[0].Value);
                Logger.Debug("ViewElementDef: " + sourceAbility.ViewElementDef.name);
                Logger.Debug("Localized name: " + sourceAbility.ViewElementDef.DisplayName1.Localize());
                Logger.Debug("Localized description: " + sourceAbility.ViewElementDef.Description.Localize());
                Logger.Debug("CharacterProgressionData: " + sourceAbility.CharacterProgressionData.name);
                Logger.Debug("------------------------- New added ability ---------------------------");
                Logger.Debug("Guid: " + addedAbility.Guid);
                Logger.Debug("Name: " + addedAbility.name);
                Logger.Debug("Modification target: " + addedAbility.StatModifications[0].TargetStat);
                Logger.Debug("Modification modtype: " + addedAbility.StatModifications[0].Modification);
                Logger.Debug("Modification value: " + addedAbility.StatModifications[0].Value);
                Logger.Debug("ViewElementDef: " + addedAbility.ViewElementDef.name);
                Logger.Debug("Localized name: " + addedAbility.ViewElementDef.DisplayName1.Localize());
                Logger.Debug("Localized description: " + addedAbility.ViewElementDef.Description.Localize());
                Logger.Debug("CharacterProgressionData: " + addedAbility.CharacterProgressionData.name);
                Logger.Debug("-----------------------------------------------------------------------");
            }
            Logger.Debug("Repo PassiveModifierAbilityDef count after: " + Repo.GetAllDefs<PassiveModifierAbilityDef>().Count());
            Logger.Debug("-----------------------------------------------------------------------");
            PassiveModifierAbilityDef testAbility = Repo.GetAllDefs<PassiveModifierAbilityDef>().FirstOrDefault(p => p.name.Equals("FirstAdded_AbilityDef"));
            if (testAbility != null)
            {
                Logger.Debug("---------------- New ability from Repo in new object ------------------");
                Logger.Debug("Guid: " + testAbility.Guid);
                Logger.Debug("Name: " + testAbility.name);
                Logger.Debug("Modification target: " + testAbility.StatModifications[0].TargetStat);
                Logger.Debug("Modification modtype: " + testAbility.StatModifications[0].Modification);
                Logger.Debug("Modification value: " + testAbility.StatModifications[0].Value);
                Logger.Debug("ViewElementDef: " + testAbility.ViewElementDef.name);
                Logger.Debug("Localized name: " + testAbility.ViewElementDef.DisplayName1.Localize());
                Logger.Debug("Localized description: " + testAbility.ViewElementDef.Description.Localize());
                Logger.Debug("CharacterProgressionData: " + testAbility.CharacterProgressionData.name);
                Logger.Debug("-----------------------------------------------------------------------");
            }
        }
    }
}
