using Base.Assets;
using Base.Assets.StreamableSystem;
using Base.Cameras.ExecutionNodes;
using Base.Cameras.Filters;
using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects;
using Base.Entities.Effects.ApplicationConditions;
using Base.Entities.Statuses;
using Base.Serialization;
using Base.Serialization.General;
using Base.Serialization.Streams;
using Base.UI;
using Base.Utils;
using Newtonsoft.Json;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Cameras.Filters;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Effects;
using PhoenixPoint.Tactical.Entities.Effects.ApplicationConditions;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PhoenixRising.SkillRework
{
    class SkillModifications
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = SkillReworkMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        private static readonly AssetsManager assetsManager = GameUtl.GameComponent<AssetsManager>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();

        public static void ApplyChanges()
        {
            try
            {
                // Get config setting for localized texts.
                bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

                // New skill BattleFocus, cloned from MasterMarksman
                string skillName = "BattleFocus_AbilityDef";
                ApplyStatusAbilityDef masterMarksman = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef"));
                // Create clones for all neccessary defs with new GUIDs and names
                ApplyStatusAbilityDef battleFocus = CreateDefFromClone(
                    masterMarksman,
                    "64fc75aa-93be-4d79-b5ac-191c5c7820da",
                    skillName);
                AbilityCharacterProgressionDef bfProgressionDef = CreateDefFromClone(
                    masterMarksman.CharacterProgressionData,
                    "7ffae720-a656-454e-a95b-b861a673718a",
                    skillName);
                bfProgressionDef.RequiredStrength = 0;
                bfProgressionDef.RequiredWill = 0;
                bfProgressionDef.RequiredSpeed = 0;
                battleFocus.CharacterProgressionData = bfProgressionDef;
                TacticalTargetingDataDef bfTargetingDataDef = CreateDefFromClone(
                    masterMarksman.TargetingDataDef,
                    "fed0600a-14b3-4ef5-ac0c-31b3bf6f1e6c",
                    skillName);
                bfTargetingDataDef.Origin.Range = 10.0f;
                battleFocus.TargetingDataDef = bfTargetingDataDef;
                TacticalAbilityViewElementDef battleFocusVisuals = CreateDefFromClone(
                    masterMarksman.ViewElementDef,
                    "b498b9de-f10b-464c-a9f9-29a293568b04",
                    skillName);
                battleFocusVisuals.DisplayName1 = new LocalizedTextBind("BATTLE FOCUS", doNotLocalize);
                battleFocusVisuals.Description = new LocalizedTextBind("If there are enemies within 10 tiles your attacks gain +10% damage", doNotLocalize);
                // TODO: Change the icon
                battleFocus.ViewElementDef = battleFocusVisuals;
                // Get base for status effect from Sneak Attack, Master Marksmans type does not support damage manipulation
                StanceStatusDef sneakAttackStatusDef = Repo.GetAllDefs<StanceStatusDef>().FirstOrDefault(p => p.name.Equals("E_SneakAttackStatus [SneakAttack_AbilityDef]"));
                StanceStatusDef bfStatusDef = CreateDefFromClone(
                    sneakAttackStatusDef,
                    "05929419-7d20-47aa-b700-fa6bc6602716",
                    skillName);
                bfStatusDef.name = $"E_Status [{skillName}]";
                bfStatusDef.EffectName = skillName;
                bfStatusDef.ShowNotification = true;
                bfStatusDef.Visuals = battleFocus.ViewElementDef;
                bfStatusDef.StatModifications[0].Value = 1.1f;
                battleFocus.StatusDef = bfStatusDef;
                VisibleActorsInRangeEffectConditionDef sourceConditionDef = (VisibleActorsInRangeEffectConditionDef)masterMarksman.TargetApplicationConditions[0];
                VisibleActorsInRangeEffectConditionDef bfConditionDef = CreateDefFromClone(
                    sourceConditionDef,
                    "63a34054-28de-488e-ae4a-af451434f0d4",
                    skillName);
                bfConditionDef.TargetingData = battleFocus.TargetingDataDef;
                bfConditionDef.ActorsInRange = true;
                battleFocus.TargetApplicationConditions = new EffectConditionDef[] { bfConditionDef };

                // Create JSON string from new skill, attention, it's HUGE (+2k lines for Battle Focus)
                //JsonSerializerSettings settings = new JsonSerializerSettings
                //{
                //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                //};
                //string bfJSON = JsonConvert.SerializeObject(battleFocus, Formatting.Indented, settings);
                //Logger.Always(bfJSON, false);

                // Snap Shot - skill creation delayed, using Battle Focus instead
                //AbilityCharacterProgressionDef sProgressionDef = CreateDefFromClone(
                //    sourceAbility.CharacterProgressionData,
                //    "0115e297-87f8-47c3-9a23-fc302f275153",
                //    skillName);
                //sProgressionDef.RequiredStrength = 0;
                //sProgressionDef.RequiredWill = 0;
                //sProgressionDef.RequiredSpeed = 0;
                //snapShot.CharacterProgressionData = sProgressionDef;

                // Gun'n'Run
                skillName = "KillAndRun_AbilityDef";
                ApplyStatusAbilityDef inspire = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("Inspire_AbilityDef"));
                ApplyStatusAbilityDef KillAndRun = CreateDefFromClone(
                    inspire,
                    "3e0e991e-e0bf-4630-b2ca-110e68790fb7",
                    skillName);
                KillAndRun.SkillTags = new SkillTagDef[0];
                AbilityCharacterProgressionDef KnRProgressionDef = CreateDefFromClone(
                    inspire.CharacterProgressionData,
                    "e3f25d2a-7668-4223-bb82-73a3f2f926aa",
                    skillName);
                KillAndRun.CharacterProgressionData = KnRProgressionDef;
                TacticalAbilityViewElementDef KnRVisuals = CreateDefFromClone(
                    inspire.ViewElementDef,
                    "8a740c8d-43b6-4ef1-9b93-b2c329566f27",
                    skillName);
                KnRVisuals.DisplayName1 = new LocalizedTextBind("KILL'N'RUN", doNotLocalize);
                KnRVisuals.Description = new LocalizedTextBind("Once per turn, take a free move after killing an enemy.", doNotLocalize);
                // Borrow icon from electric kick (shadow legs ability)
                // TODO: Find a way to access unused Sprite from assets
                Sprite gnrIcon = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(t => t.name.Equals("E_View [ElectricKick_AbilityDef]")).LargeIcon;
                KnRVisuals.LargeIcon = gnrIcon;
                KnRVisuals.SmallIcon = gnrIcon;
                KillAndRun.ViewElementDef = KnRVisuals;
                KillAndRun.ViewElementDef.HideFromPassives = true;

                RepositionAbilityDef KnRReposition = CreateDefFromClone(
                    Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef")),
                    "de8cd8a9-f2eb-4b8a-a408-a2a1913930c4",
                    "KillAndRun_Move_AbilityDef");
                KnRReposition.CharacterProgressionData = KnRProgressionDef;
                KnRReposition.TargetingDataDef = CreateDefFromClone(
                    Repo.GetAllDefs<TacticalTargetingDataDef>().FirstOrDefault(t => t.name.Equals("E_TargetingData [Dash_AbilityDef]")),
                    "18e86a2b-6031-4c84-a2a0-cb6ad2423b56",
                    "E_TargetingData [KillAndRun_Move_AbilityDef]");
                KnRReposition.TargetingDataDef.Origin.Range = 10.0f;
                KnRReposition.ViewElementDef = KnRVisuals;
                KnRReposition.ViewElementDef.DisplayWithEquipmentMismatch = false;
                KnRReposition.ViewElementDef.DisplayWithRequiredAbilitiesMissing = false;
                KnRReposition.ViewElementDef.DisplayWithTraitMismatch = false;
                KnRReposition.IgnoreForEndOfCharTurn = true;
                KnRReposition.SuppressAutoStandBy = true;
                KnRReposition.UsesPerTurn = 1;
                KnRReposition.ActionPointCost = 0.0f;
                KnRReposition.WillPointCost = 0.0f;
                KnRReposition.PreparationActorEffectDef = CreateDefFromClone(
                    Repo.GetAllDefs<StatusEffectDef>().FirstOrDefault(s => s.name.Equals("E_ApplyVanishStatusEffect [Vanish_AbilityDef]")),
                    "8ea85920-588b-4e1d-a8e6-31ffbe9d3a02",
                    "E_ReApplyOnActorDeathStatusEffect [" + skillName + "]");
                KnRReposition.SamePositionIsValidTarget = true;
                KnRReposition.AmountOfMovementToUseAsRange = -1.0f;
                // Add new KnR move ability to animation action handler for dash (same animation?)
                foreach (TacActorSimpleAbilityAnimActionDef def in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(b => b.name.Contains("Dash")))
                {
                    def.AbilityDefs = def.AbilityDefs.Append(KnRReposition).ToArray();
                }
                TacCameraAbilityFilterDef KnRRepositionCameraFilterDef = CreateDefFromClone(
                    Repo.GetAllDefs<TacCameraAbilityFilterDef>().FirstOrDefault(c => c.name.Equals("E_DashAbilityFilter [NoDieCamerasTacticalCameraDirectorDef]")),
                    "bf422b08-5b84-4b6a-a0cd-74ce1bfbc2fc",
                    "E_KillnRunMoveAbilityFilter [NoDieCamerasTacticalCameraDirectorDef]");
                KnRRepositionCameraFilterDef.TacticalAbilityDef = KnRReposition;
                FirstMatchExecutionDef KnRFmeDef = CreateDefFromClone(
                    Repo.GetAllDefs<FirstMatchExecutionDef>().FirstOrDefault(bd => bd.name.Equals("E_DashCameraAbility [NoDieCamerasTacticalCameraDirectorDef]")),
                    "75d8137e-06f7-4840-8156-23366c4daea7",
                    "E_KillnRunMoveCameraAbility [NoDieCamerasTacticalCameraDirectorDef]");
                KnRFmeDef.FilterDef = KnRRepositionCameraFilterDef;

                StatusRemoverEffectDef KnRStatusRemoverEffect = CreateDefFromClone(
                    Repo.GetAllDefs<StatusRemoverEffectDef>().FirstOrDefault(a => a.name.Equals("E_RemoveStandBy [ManualControlStatus]")),
                    "77b65001-7b75-4fbc-a89e-cf3e3e8ca69f",
                    "E_RemoveDeathEffectStatus [" + skillName + "]");
                KnRStatusRemoverEffect.StatusToRemove = "KnR_DeathEffectStatus";

                OnActorDeathEffectStatusDef KnRDeathEffectStatus = CreateDefFromClone(
                    (OnActorDeathEffectStatusDef)inspire.StatusDef,
                    "7cfcb266-6730-4642-88d5-8a212104b9cc",
                    skillName);
                KnRDeathEffectStatus.EffectName = "KnR_DeathEffectStatus";
                KnRDeathEffectStatus.VisibleOnPassiveBar = true;
                KnRDeathEffectStatus.Visuals = KillAndRun.ViewElementDef;
                KnRDeathEffectStatus.EffectDef = KnRStatusRemoverEffect;
                KnRReposition.DisablingStatuses = new StatusDef[] { KnRDeathEffectStatus };
                ((StatusEffectDef)KnRReposition.PreparationActorEffectDef).StatusDef = KnRDeathEffectStatus;

                AddAbilityStatusDef KnRAddAbilityStatus = CreateDefFromClone(
                    Repo.GetAllDefs<AddAbilityStatusDef>().FirstOrDefault(a => a.name.Equals("E_AddAbilityStatus [DeployBeacon_StatusDef]")),
                    "ac18e0d8-530d-4077-b372-71c9f82e2b88",
                    skillName);
                KnRAddAbilityStatus.Duration = -1;
                KnRAddAbilityStatus.AbilityDef = KnRReposition; //Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef"));

                MultiStatusDef KnRMultiStatus = CreateDefFromClone(
                    Repo.GetAllDefs<MultiStatusDef>().FirstOrDefault(m => m.name.Equals("E_MultiStatus [RapidClearance_AbilityDef]")),
                    "be7115e5-ce6b-47da-bead-311f3978f242",
                    skillName);
                KnRMultiStatus.Statuses = new StatusDef[] { KnRDeathEffectStatus, KnRAddAbilityStatus };

                KillAndRun.StatusDef = KnRMultiStatus;

                //StatusEffectDef KnRStatusEffect = CreateDefFromClone(
                //    Repo.GetAllDefs<StatusEffectDef>().FirstOrDefault(s => s.name.Equals("E_ApplyVanishStatusEffect [Vanish_AbilityDef]")),
                //    "",
                //    "E_ApplyMoveStatusEffect [" + skillName + "]");
                //KnRStatusEffect.StatusDef = KnRAddAbilityStatus;
                
                // Fix for Return Fire to work on all classes
                TacticalAbilityDef returnFire = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains("ReturnFire_AbilityDef"));
                returnFire.ActorTags = new GameTagDef[0]; // Deletes all given tags => no restriction for any class

                // Change Extreme Focus, set to 1 AP regardless of weapon type
                ChangeAbilitiesCostStatusDef extremeFocusAPcostMod = Repo.GetAllDefs<ChangeAbilitiesCostStatusDef>().FirstOrDefault(c => c.name.Contains("ExtremeFocus_AbilityDef"));
                extremeFocusAPcostMod.AbilityCostModification.ActionPointModType = TacticalAbilityModificationType.Set;
                extremeFocusAPcostMod.AbilityCostModification.ActionPointMod = 0.25f;
                extremeFocusAPcostMod.Visuals.Description = new LocalizedTextBind("Overwatch cost is set to 1 Action Point cost for all weapons", true);

                // Quick aim changes
                ApplyStatusAbilityDef quickAim = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("QuickAim_AbilityDef"));
                quickAim.UsesPerTurn = 2;
                BonusStatHolderStatusDef qaAccMod = Repo.GetAllDefs<BonusStatHolderStatusDef>().FirstOrDefault(b => b.name.Equals("E_AccuracyModifier [QuickAim_AbilityDef]"));
                qaAccMod.Value = -0.3f; // Acc bonus/malus to add, default 0.25f = +25%, new -0.3 = -30%
                TacStatusDef[] qaStatusDefs = new TacStatusDef[]
                {
                    ((AddAttackBoostStatusDef)quickAim.StatusDef).AdditionalStatusesToApply[0],
                    qaAccMod
                };
                ((AddAttackBoostStatusDef)quickAim.StatusDef).AdditionalStatusesToApply = qaStatusDefs;
                quickAim.ViewElementDef.Description = new LocalizedTextBind(
                    "The Action Point cost of the next shot with a proficient weapon is reduced by 1 with -30% accuracy. Limited to 2 uses per turn.",
                    doNotLocalize);

                // Mist Breather adding progression def
                ApplyEffectAbilityDef mistBreather = Repo.GetAllDefs<ApplyEffectAbilityDef>().FirstOrDefault(a => a.name.Equals("Exalted_MistBreather_AbilityDef"));
                AbilityCharacterProgressionDef mbProgressionDef = CreateDefFromClone(
                    masterMarksman.CharacterProgressionData,
                    "9eaf8809-01d9-4582-89e0-78c8596f5e7d",
                    "MistBreather_AbilityDef");
                mbProgressionDef.RequiredStrength = 0;
                mbProgressionDef.RequiredWill = 0;
                mbProgressionDef.RequiredSpeed = 0;
                mistBreather.CharacterProgressionData = mbProgressionDef;

                // Dash changes
                //RepositionAbilityDef dash = Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef"));

                // Tweaking the weapon proficiency perks incl. descriptions
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

                                Logger.Debug("  Target item: " + pmad.ItemTagStatModifications[i].ItemTag.name);
                                Logger.Debug("  Target stat: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.TargetStat);
                                Logger.Debug(" Modification: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.Modification);
                                Logger.Debug("        Value: " + pmad.ItemTagStatModifications[i].EquipmentStatModification.Value);
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

        // Creating new runtime def by cloning from existing def
        public static T CreateDefFromClone<T>(T source, string guid, string name) where T : BaseDef
        {
            try
            {
                Type type;
                string resultName = "";
                if (source != null)
                {
                    Logger.Debug("CreateDefFromClone with source type: " + source.GetType().Name);
                    Logger.Debug("CreateDefFromClone with source name: " + source.name);
                    type = null;
                    int start = source.name.IndexOf('[') + 1;
                    int end = source.name.IndexOf(']');
                    resultName = start > 0 && end > start ? source.name.Substring(start, end - start) : source.name;
                    resultName = source.name.Replace(resultName, name);
                }
                else
                {
                    Logger.Debug("CreateDefFromClone only with type: " + typeof(T).Name);
                    type = typeof(T);
                    resultName = name;
                }
                T result = (T)Repo.CreateRuntimeDef(
                    source,
                    type,
                    guid);
                result.name = resultName;
                Logger.Debug("CreateDefFromClone result type: " + result.GetType().Name);
                Logger.Debug("CreateDefFromClone result name: " + result.name);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
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
        /*
        // Patch the return fire selection function
        [HarmonyPatch(typeof(TacticalLevelController), "GetReturnFireAbilities")]
        internal static class GetReturnFireAbilities_Patches
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static bool Prefix(ref List<ReturnFireAbility> __result,
                                        TacticalLevelController __instance,
                                        TacticalActor shooter,
                                        Weapon weapon,
                                        TacticalAbilityTarget target,
                                        ShootAbility shootAbility,
                                        bool getOnlyPossibleTargets = false,
                                        List<TacticalActor> casualties = null)
            {
                try
                {
                    Logger.Debug("------------ Enter GetReturnFireAbilities --------------");
                    Weapon weapon2 = weapon;
                    WeaponDef weaponDef = (weapon2 != null) ? weapon2.WeaponDef : null;
                    if (target.AttackType == AttackType.ReturnFire
                        || target.AttackType == AttackType.Overwatch
                        || target.AttackType == AttackType.Synced
                        || target.AttackType == AttackType.ZoneControl
                        || (weaponDef != null && weaponDef.NoReturnFireFromTargets))
                    {
                        Logger.Debug("AttackType check negative -> leaving GetReturnFireAbilities with NULL result!");
                        Logger.Debug("------- Exit GetReturnFireAbilities sucessful ----------");
                        __result = null;
                        return false;
                    }
                    Logger.Debug("AttackType check successful passed.");
                    List<ReturnFireAbility> result;
                    using (new MultiForceDummyTargetableLock(__instance.Map.GetActors<TacticalActor>(null)))
                    {
                        foreach (TacticalActor tmpActor in __instance.Map.GetActors<TacticalActor>(null))
                        {
                            Logger.Debug("------------------------- Begin Actor data ----------------------------");
                            Logger.Debug("Actor name: " + tmpActor.GetDisplayName());
                            if (tmpActor.GetAbilities<ReturnFireAbility>() != null)
                            {
                                Logger.Debug("                     Actor return fire abilities: " + string.Join(", ", tmpActor.GetAbilities<ReturnFireAbility>()));
                                Logger.Debug("RF ability IgnoreNoValidTargetsFilter is enabled: " + tmpActor.GetAbilities<ReturnFireAbility>().FirstOrDefault().IsEnabled(IgnoredAbilityDisabledStatesFilter.IgnoreNoValidTargetsFilter));
                                Logger.Debug("                       RF ability disabled state: " + tmpActor.GetAbilities<ReturnFireAbility>().FirstOrDefault().GetDisabledState().Key);
                            }
                            else
                            {
                                Logger.Debug("No return fire abilities fourn on actor. ");
                            }
                            Logger.Debug("------------------------- End Actor data ------------------------------");
                        }
                        List<ReturnFireAbility> list = (from actor in __instance.Map.GetActors<TacticalActor>(null)
                                                        where actor.IsAlive && actor.RelationTo(shooter) == FactionRelation.Enemy
                                                        from ability in
                                                            from a in actor.GetAbilities<ReturnFireAbility>()
                                                            orderby a.ReturnFireDef.ReturnFirePriority
                                                            select a
                                                        where ability.IsEnabled(IgnoredAbilityDisabledStatesFilter.IgnoreNoValidTargetsFilter)
                                                        group ability by actor into actorReturns
                                                        let actorAbility = actorReturns.First<ReturnFireAbility>()
                                                        orderby actorAbility.TacticalActor == target.GetTargetActor() descending
                                                        select actorAbility).Where(delegate (ReturnFireAbility returnFireAbility)
                                                        {
                                                            Logger.Debug("Return fire ablity on actor " + returnFireAbility.TacticalActorBase.DisplayName + "found: " + returnFireAbility.AbilityDef.name);
                                                            TacticalActor actor = returnFireAbility.TacticalActor;
                                                            if (weapon != null)
                                                            {
                                                                Logger.Debug("Weapon != null: " + weapon.DisplayName + " -> place target for shooting.");
                                                                shooter.TargetDummy.PlaceForShooting(weapon, target.ShootFromPos, target, false);
                                                            }
                                                            if (!returnFireAbility.IsValidTarget(shooter))
                                                            {
                                                                Logger.Debug("Return fire has no valid target!");
                                                                return false;
                                                            }
                                                            if (returnFireAbility.ReturnFireDef.RiposteWithBashAbility)
                                                            {
                                                                Logger.Debug("Return fire has riposte with bash set, returns true.");
                                                                return true;
                                                            }
                                                            if (getOnlyPossibleTargets
                                                                && target.Actor != actor
                                                                && (target.MultiAbilityTargets == null || !target.MultiAbilityTargets.Any((TacticalAbilityTarget mat) => mat.Actor == actor))
                                                                && (casualties == null || !casualties.Contains(actor)))
                                                            {
                                                                Logger.Debug("Multiple check (getOnlyPossibleTargets & target.Actor != actor ... is true, returns false!");
                                                                return false;
                                                            }
                                                            ShootAbility defaultShootAbility = returnFireAbility.GetDefaultShootAbility();
                                                            TacticalAbilityTarget attackActorTarget = defaultShootAbility.GetAttackActorTarget(shooter, AttackType.ReturnFire);
                                                            if (attackActorTarget == null || !Utl.Equals(attackActorTarget.ShootFromPos, defaultShootAbility.Actor.Pos, 1E-05f))
                                                            {
                                                                Logger.Debug("attackTarget is NULL or (attackActorTarget.ShootFromPos, defaultShootAbility.Actor.Pos) is NOT equal, returns false!");
                                                                return false;
                                                            }
                                                            TacticalActor tacticalActor = null;
                                                            if (returnFireAbility.TacticalActor.TacticalPerception.CheckFriendlyFire(returnFireAbility.Weapon,
                                                                                                                                        attackActorTarget.ShootFromPos,
                                                                                                                                        attackActorTarget,
                                                                                                                                        out tacticalActor,
                                                                                                                                        FactionRelation.Neutral | FactionRelation.Friend))
                                                            {
                                                                Logger.Debug("Check friendly fire triggered, return false!");
                                                                return false;
                                                            }
                                                            if (!returnFireAbility.TacticalActor.TacticalPerception.HasFloorSupportAt(returnFireAbility.TacticalActor.Pos))
                                                            {
                                                                Logger.Debug("Check perception triggered, return false!");
                                                                return false;
                                                            }
                                                            TacticalActorBase tacticalActor2 = returnFireAbility.TacticalActor;
                                                            Vector3 pos = returnFireAbility.TacticalActor.Pos;
                                                            TacticalActorBase shooter2 = shooter;
                                                            bool checkAllPoints = false;
                                                            TacticalAbilityTarget target2 = target;
                                                            return TacticalFactionVision.CheckVisibleLineBetweenActors(tacticalActor2,
                                                                                                                        pos,
                                                                                                                        shooter2,
                                                                                                                        checkAllPoints,
                                                                                                                        new Vector3?((target2 != null) ? target2.ShootFromPos : shooter.Pos),
                                                                                                                        0.5f,
                                                                                                                        null);
                                                        }).ToList<ReturnFireAbility>();
                        if (weapon != null)
                        {
                            Logger.Debug("Weapon check successful passed. Call 'PlaceForShooting' on shooter: " + shooter.DisplayName);
                            shooter.TargetDummy.PlaceForShooting(weapon, shooter.Pos, target, false);
                        }
                        result = list;
                    }
                    Logger.Debug("Return Fire List count: " + result.Count);
                    __result = result;
                    Logger.Debug("------- Exit GetReturnFireAbilities sucessful ----------");
                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    Logger.Debug("------- Exit GetReturnFireAbilities with error! --------");
                    return true;
                }
            }
        }
        */
    }
}
