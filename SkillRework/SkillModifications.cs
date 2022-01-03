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
        //private static readonly AssetsManager assetsManager = GameUtl.GameComponent<AssetsManager>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();

        public static void ApplyChanges()
        {
            try
            {
                // Get config setting for localized texts.
                bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

                // BattleFocus, currently used as placeholder, will go to Vengeance Torso
                Create_BattleFocus(Repo, Config);

                // Assault skills start -------------------------------------------------------

                // Quick aim changes, adding aim modification
                ApplyStatusAbilityDef quickAim = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("QuickAim_AbilityDef"));
                quickAim.UsesPerTurn = 2;
                BonusStatHolderStatusDef qaAccMod = Repo.GetAllDefs<BonusStatHolderStatusDef>().FirstOrDefault(b => b.name.Equals("E_AccuracyModifier [QuickAim_AbilityDef]"));
                qaAccMod.Value = -0.3f; // Acc bonus/malus to add, default 0.25f = +25%, new -0.3 = -30%
                ((AddAttackBoostStatusDef)quickAim.StatusDef).AdditionalStatusesToApply =
                    ((AddAttackBoostStatusDef)quickAim.StatusDef).AdditionalStatusesToApply.Append(qaAccMod).ToArray();
                quickAim.ViewElementDef.Description = new LocalizedTextBind(
                    "The Action Point cost of the next shot with a proficient weapon is reduced by 1 with -30% accuracy. Limited to 2 uses per turn.",
                    doNotLocalize);
                // Gun'n'Run
                Create_KillAndRun(Repo, Config);
                
                // Barrage
                Create_Barrage(Repo, Config);

                // Onslaught (DeterminedAdvance_AbilityDef) change: Receiver can get only 1 onslaught per turn.
                // This below works on the target but he can be targeted again from another Assault without any response => the Assault loses 2 AP and the target gets nothing
                // Looking for a solution, maybe MC fuctionality could be a solution (thx to Iko)
                //TacEffectStatusDef onslaughtStatus = Repo.GetAllDefs<TacEffectStatusDef>().FirstOrDefault(c => c.name.Contains("E_Status [DeterminedAdvance_AbilityDef]"));
                //onslaughtStatus.SingleInstance = true;
                // .... delayed ....

                // Rapid Clearance: Until end of turn, after killing an enemy next attack cost -2AP
                // Get Rapid Clearance ability def
                ApplyStatusAbilityDef rapidClearance = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("RapidClearance_AbilityDef"));
                // Clone status apply effect from Vanish
                StatusEffectDef applyStatusEffect = CreateDefFromClone(
                    Repo.GetAllDefs<StatusEffectDef>().FirstOrDefault(s => s.name.Equals("E_ApplyVanishStatusEffect [Vanish_AbilityDef]")),
                    "8ea85920-588b-4e1d-a8e6-31ffbe9d3a02",
                    "E_ApplyStatusEffect [RapidClearance_AbilityDef]");
                // Clone AP reduction status from QA
                ChangeAbilitiesCostStatusDef apReductionStatusEffect = CreateDefFromClone(
                    Repo.GetAllDefs<ChangeAbilitiesCostStatusDef>().FirstOrDefault(s => s.name.Equals("E_AbilityCostModifier [QuickAim_AbilityDef]")),
                    "e3062779-8f2f-4407-bc4f-a20f5c2d267b",
                    "E_AbilityCostModifier [RapidClearance_AbilityDef]");
                // change properties and references
                rapidClearance.ViewElementDef.Description = new LocalizedTextBind("Until end of turn, after killing an enemy next attack cost -2AP", doNotLocalize);
                apReductionStatusEffect.AbilityCostModification.RequiresProficientEquipment = false; // original QA true
                apReductionStatusEffect.AbilityCostModification.SkillTagCullFilter = new SkillTagDef[0]; // No restrictions, original QA melee and throwing grenades are disabled
                apReductionStatusEffect.AbilityCostModification.ActionPointMod = -0.5f; // -2 AP, original QA -1 AP
                applyStatusEffect.StatusDef = apReductionStatusEffect;
                (rapidClearance.StatusDef as OnActorDeathEffectStatusDef).EffectDef = applyStatusEffect;

                // Assault skills end -------------------------------------------------------


                // Sniper skills
                // Change Extreme Focus, set to 1 AP regardless of weapon type
                ChangeAbilitiesCostStatusDef extremeFocusAPcostMod = Repo.GetAllDefs<ChangeAbilitiesCostStatusDef>().FirstOrDefault(c => c.name.Contains("ExtremeFocus_AbilityDef"));
                extremeFocusAPcostMod.AbilityCostModification.ActionPointModType = TacticalAbilityModificationType.Set;
                extremeFocusAPcostMod.AbilityCostModification.ActionPointMod = 0.25f;
                extremeFocusAPcostMod.Visuals.Description = new LocalizedTextBind("Overwatch cost is set to 1 Action Point cost for all weapons", doNotLocalize);

                //Heavy skills
                // Fix for Return Fire to work on all classes
                TacticalAbilityDef returnFire = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains("ReturnFire_AbilityDef"));
                returnFire.ActorTags = new GameTagDef[0]; // Deletes all given tags => no restriction for any class

                // Berserker skills
                // Dash changes
                //RepositionAbilityDef dash = Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef"));

                // Faction perks
                // Mist Breather adding progression def
                ApplyEffectAbilityDef mistBreather = Repo.GetAllDefs<ApplyEffectAbilityDef>().FirstOrDefault(a => a.name.Equals("Exalted_MistBreather_AbilityDef"));
                AbilityCharacterProgressionDef mbProgressionDef = CreateDefFromClone(
                    Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef")).CharacterProgressionData,
                    "9eaf8809-01d9-4582-89e0-78c8596f5e7d",
                    "MistBreather_AbilityDef");
                mbProgressionDef.RequiredStrength = 0;
                mbProgressionDef.RequiredWill = 0;
                mbProgressionDef.RequiredSpeed = 0;
                mistBreather.CharacterProgressionData = mbProgressionDef;

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

        // New Battle Focus ability
        public static void Create_BattleFocus(DefRepository Repo, Settings Config)
        {
            string skillName = "BattleFocus_AbilityDef";
            bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

            // Source to clone from
            ApplyStatusAbilityDef masterMarksman = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(p => p.name.Equals("MasterMarksman_AbilityDef"));

            // Create Neccessary RuntimeDefs
            ApplyStatusAbilityDef battleFocusAbility = CreateDefFromClone(
                masterMarksman,
                "64fc75aa-93be-4d79-b5ac-191c5c7820da",
                skillName);
            AbilityCharacterProgressionDef progression = CreateDefFromClone(
                masterMarksman.CharacterProgressionData,
                "7ffae720-a656-454e-a95b-b861a673718a",
                skillName);
            TacticalTargetingDataDef targetingData = CreateDefFromClone(
                masterMarksman.TargetingDataDef,
                "fed0600a-14b3-4ef5-ac0c-31b3bf6f1e6c",
                skillName);
            TacticalAbilityViewElementDef vieElement = CreateDefFromClone(
                masterMarksman.ViewElementDef,
                "b498b9de-f10b-464c-a9f9-29a293568b04",
                skillName);
            StanceStatusDef stanceStatus = CreateDefFromClone( // Borrow status from Sneak Attack, Master Marksman status does not fit
                Repo.GetAllDefs<StanceStatusDef>().FirstOrDefault(p => p.name.Equals("E_SneakAttackStatus [SneakAttack_AbilityDef]")),
                "05929419-7d20-47aa-b700-fa6bc6602716",
                "E_Status [" + skillName + "]");
            VisibleActorsInRangeEffectConditionDef visibleActorsInRangeEffectCondition = CreateDefFromClone(
                (VisibleActorsInRangeEffectConditionDef)masterMarksman.TargetApplicationConditions[0],
                "63a34054-28de-488e-ae4a-af451434f0d4",
                skillName);

            // Set fields
            battleFocusAbility.CharacterProgressionData = progression;
            battleFocusAbility.TargetingDataDef = targetingData;
            battleFocusAbility.ViewElementDef = vieElement;
            battleFocusAbility.StatusDef = stanceStatus;
            battleFocusAbility.TargetApplicationConditions = new EffectConditionDef[] { visibleActorsInRangeEffectCondition };
            progression.RequiredStrength = 0;
            progression.RequiredWill = 0;
            progression.RequiredSpeed = 0;
            targetingData.Origin.Range = 10.0f;
            vieElement.DisplayName1 = new LocalizedTextBind("BATTLE FOCUS", doNotLocalize);
            vieElement.Description = new LocalizedTextBind("If there are enemies within 10 tiles your attacks gain +10% damage", doNotLocalize);
            // TODO: Change to own Icon
            stanceStatus.EffectName = skillName;
            stanceStatus.ShowNotification = true;
            stanceStatus.Visuals = battleFocusAbility.ViewElementDef;
            stanceStatus.StatModifications[0].Value = 1.1f;
            visibleActorsInRangeEffectCondition.TargetingData = battleFocusAbility.TargetingDataDef;
            visibleActorsInRangeEffectCondition.ActorsInRange = true;
        }

        // New Kill'n'Run ability
        public static void Create_KillAndRun(DefRepository Repo, Settings Config)
        {
            string skillName = "KillAndRun_AbilityDef";
            bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

            // Source to clone from for main ability: Inspire
            ApplyStatusAbilityDef inspireAbility = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("Inspire_AbilityDef"));

            // Create Neccessary RuntimeDefs
            ApplyStatusAbilityDef killAndRunAbility = CreateDefFromClone(
                inspireAbility,
                "3e0e991e-e0bf-4630-b2ca-110e68790fb7",
                skillName);
            AbilityCharacterProgressionDef progression = CreateDefFromClone(
                inspireAbility.CharacterProgressionData,
                "e3f25d2a-7668-4223-bb82-73a3f2f926aa",
                skillName);
            TacticalAbilityViewElementDef viewElement = CreateDefFromClone(
                inspireAbility.ViewElementDef,
                "8a740c8d-43b6-4ef1-9b93-b2c329566f27",
                skillName);
            OnActorDeathEffectStatusDef onActorDeathEffectStatus = CreateDefFromClone(
                inspireAbility.StatusDef as OnActorDeathEffectStatusDef,
                "7cfcb266-6730-4642-88d5-8a212104b9cc",
                "E_KillListenerStatus [" + skillName + "]");
            RepositionAbilityDef dashAbility = CreateDefFromClone( // Create an own Dash ability from standard Dash
                Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef")),
                "de8cd8a9-f2eb-4b8a-a408-a2a1913930c4",
                "KillAndRun_Dash_AbilityDef");
            TacticalTargetingDataDef dashTargetingData = CreateDefFromClone( // ... and clone its targeting data
                Repo.GetAllDefs<TacticalTargetingDataDef>().FirstOrDefault(t => t.name.Equals("E_TargetingData [Dash_AbilityDef]")),
                "18e86a2b-6031-4c84-a2a0-cb6ad2423b56",
                "KillAndRun_Dash_AbilityDef");
            StatusRemoverEffectDef statusRemoverEffect = CreateDefFromClone( // Borrow effect from Manual Control
                Repo.GetAllDefs<StatusRemoverEffectDef>().FirstOrDefault(a => a.name.Equals("E_RemoveStandBy [ManualControlStatus]")),
                "77b65001-7b75-4fbc-a89e-cf3e3e8ca69f",
                "E_StatusRemoverEffect [" + skillName + "]");
            AddAbilityStatusDef addAbiltyStatus = CreateDefFromClone( // Borrow status from Deplay Beacon (final mission)
                Repo.GetAllDefs<AddAbilityStatusDef>().FirstOrDefault(a => a.name.Equals("E_AddAbilityStatus [DeployBeacon_StatusDef]")),
                "ac18e0d8-530d-4077-b372-71c9f82e2b88",
                skillName);
            MultiStatusDef multiStatus = CreateDefFromClone( // Borrow multi status from Rapid Clearance
                Repo.GetAllDefs<MultiStatusDef>().FirstOrDefault(m => m.name.Equals("E_MultiStatus [RapidClearance_AbilityDef]")),
                "be7115e5-ce6b-47da-bead-311f3978f242",
                skillName);
            FirstMatchExecutionDef cameraAbility = CreateDefFromClone(
                Repo.GetAllDefs<FirstMatchExecutionDef>().FirstOrDefault(bd => bd.name.Equals("E_DashCameraAbility [NoDieCamerasTacticalCameraDirectorDef]")),
                "75d8137e-06f7-4840-8156-23366c4daea7",
                "E_KnR_Dash_CameraAbility [NoDieCamerasTacticalCameraDirectorDef]");
            cameraAbility.FilterDef = CreateDefFromClone(
                Repo.GetAllDefs<TacCameraAbilityFilterDef>().FirstOrDefault(c => c.name.Equals("E_DashAbilityFilter [NoDieCamerasTacticalCameraDirectorDef]")),
                "bf422b08-5b84-4b6a-a0cd-74ce1bfbc2fc",
                "E_KnR_Dash_CameraAbilityFilter [NoDieCamerasTacticalCameraDirectorDef]");
            (cameraAbility.FilterDef as TacCameraAbilityFilterDef).TacticalAbilityDef = dashAbility;

            // Add new KnR Dash ability to animation action handler for dash (same animation)
            foreach (TacActorSimpleAbilityAnimActionDef def in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(b => b.name.Contains("Dash")))
            {
                def.AbilityDefs = def.AbilityDefs.Append(dashAbility).ToArray();
            }

            // Set fields
            killAndRunAbility.CharacterProgressionData = progression;
            killAndRunAbility.ViewElementDef = viewElement;
            killAndRunAbility.SkillTags = new SkillTagDef[0];
            killAndRunAbility.StatusDef = multiStatus;
            killAndRunAbility.StatusApplicationTrigger = StatusApplicationTrigger.StartTurn;

            //dashAbility.CharacterProgressionData = progression;
            dashAbility.TargetingDataDef = dashTargetingData;
            dashAbility.TargetingDataDef.Origin.Range = 10.0f;
            dashAbility.ViewElementDef = viewElement;
            //dashAbility.IgnoreForEndOfCharTurn = true;
            dashAbility.SuppressAutoStandBy = true;
            dashAbility.DisablingStatuses = new StatusDef[] { onActorDeathEffectStatus };
            dashAbility.UsesPerTurn = 1;
            dashAbility.ActionPointCost = 0.0f;
            dashAbility.WillPointCost = 0.0f;
            //dashAbility.PreparationActorEffectDef = statusEffect;
            dashAbility.SamePositionIsValidTarget = true;
            dashAbility.AmountOfMovementToUseAsRange = -1.0f;

            viewElement.DisplayName1 = new LocalizedTextBind("KILL'N'RUN", doNotLocalize);
            viewElement.Description = new LocalizedTextBind("Once per turn, take a free move after killing an enemy.", doNotLocalize);
            // Borrow icon from electric kick (shadow legs ability)
            // TODO: Change to own Icon
            Sprite knR_IconSprite = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(t => t.name.Equals("E_View [ElectricKick_AbilityDef]")).LargeIcon;
            viewElement.LargeIcon = knR_IconSprite;
            viewElement.SmallIcon = knR_IconSprite;
            //viewElement.DisplayWithEquipmentMismatch = false;
            //viewElement.DisplayWithRequiredAbilitiesMissing = false;
            //viewElement.DisplayWithTraitMismatch = false;
            viewElement.HideFromPassives = true;
            //viewElement.ShowInStatusScreen = false;
            //viewElement.ShouldFlash = true;

            multiStatus.Statuses = new StatusDef[] { onActorDeathEffectStatus, addAbiltyStatus };

            onActorDeathEffectStatus.EffectName = "KnR_KillTriggerListener";
            onActorDeathEffectStatus.Visuals = viewElement;
            onActorDeathEffectStatus.VisibleOnPassiveBar = true;
            onActorDeathEffectStatus.DurationTurns = 0;
            onActorDeathEffectStatus.EffectDef = statusRemoverEffect;
            //onActorDeathEffectStatus.EffectDef = statusEffect;

            statusRemoverEffect.StatusToRemove = "KnR_KillTriggerListener";

            //statusEffect.StatusDef = addAbiltyStatus;
            //statusEffect.StatusDef = onActorDeathEffectStatus;

            addAbiltyStatus.DurationTurns = 0;
            addAbiltyStatus.SingleInstance = true;
            addAbiltyStatus.AbilityDef = dashAbility;
            //addAbiltyStatus.UnapplyIfAbilityExists = true;
        }

        // New Barrage ability
        public static void Create_Barrage(DefRepository Repo, Settings Config)
        {
            string skillName = "Barrage_AbilityDef";
            bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

            // Source to clone from
            ShootAbilityDef rageBurst = Repo.GetAllDefs<ShootAbilityDef>().FirstOrDefault(p => p.name.Equals("RageBurst_ShootAbilityDef"));

            // Create Neccessary RuntimeDefs
            ShootAbilityDef barrageAbility = CreateDefFromClone(
                rageBurst,
                "fc5f5cf1-1349-42ff-adc4-515d7ceddde4",
                skillName);
            AbilityCharacterProgressionDef progression = CreateDefFromClone(
                rageBurst.CharacterProgressionData,
                "fa68ad15-a29b-4c66-b34a-fde332fc9d49",
                skillName);
            TacticalAbilityViewElementDef viewElement = CreateDefFromClone(
                rageBurst.ViewElementDef,
                "13005fbc-2613-4a01-9355-0701ae350ca5",
                skillName);
            SceneViewElementDef sceneView = CreateDefFromClone(
                rageBurst.SceneViewElementDef,
                "b1eefbc3-fb40-4733-a0cf-4efeecfc3af3",
                skillName);

            // Set fields
            barrageAbility.CharacterProgressionData = progression;
            barrageAbility.ViewElementDef = viewElement;
            barrageAbility.SceneViewElementDef = sceneView;
            barrageAbility.SkillTags = new SkillTagDef[] { rageBurst.SkillTags[0] };
            barrageAbility.ActionPointCost = 0.75f;
            barrageAbility.WillPointCost = 4.0f;
            barrageAbility.ActorTags = new GameTagDef[] { Repo.GetAllDefs<GameTagDef>().FirstOrDefault(t => t.name.Equals("Assault_ClassTagDef")) };
            barrageAbility.EquipmentTags = new GameTagDef[] { Repo.GetAllDefs<GameTagDef>().FirstOrDefault(t => t.name.Equals("AssaultRifleItem_TagDef")) };
            barrageAbility.AttackType = AttackType.RageBurst;
            barrageAbility.TargetsCount = 1;
            barrageAbility.ExecutionsCount = 2;
            barrageAbility.ForceFirstPersonCam = false;
            barrageAbility.ProjectileSpreadMultiplier = 0.7f;
            progression.RequiredStrength = 0;
            progression.RequiredWill = 0;
            progression.RequiredSpeed = 0;
            viewElement.DisplayName1 = new LocalizedTextBind("BARRAGE", doNotLocalize);
            viewElement.Description = new LocalizedTextBind("Next shot with Assault Rifle uses double burst and gains +30% acc", doNotLocalize);
            // TODO: Change to own Icon, current borrowed from Deadly Dou
            Sprite barrage_IconSprite = Repo.GetAllDefs<TacticalAbilityViewElementDef>().FirstOrDefault(t => t.name.Equals("E_View [DeadlyDuo_ShootAbilityDef]")).LargeIcon;
            viewElement.LargeIcon = barrage_IconSprite;
            viewElement.SmallIcon = barrage_IconSprite;
            viewElement.MultiTargetSelectionButtonTexts = new string[0];
        }

        // Creating new runtime def by cloning from existing def
        public static T CreateDefFromClone<T>(T source, string guid, string name) where T : BaseDef
        {
            try
            {
                Type type = null;
                string resultName = "";
                if (source != null)
                {
                    Logger.Debug("CreateDefFromClone with source type: " + source.GetType().Name);
                    Logger.Debug("CreateDefFromClone with source name: " + source.name);
                    int start = source.name.IndexOf('[') + 1;
                    int end = source.name.IndexOf(']');
                    string toReplace = !name.Contains("[") && start > 0 && end > start ? source.name.Substring(start, end - start) : source.name;
                    resultName = source.name.Replace(toReplace, name);
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
    }
}
