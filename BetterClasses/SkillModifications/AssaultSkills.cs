using Base.Cameras.ExecutionNodes;
using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects;
using Base.Entities.Statuses;
using Base.UI;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Cameras.Filters;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Statuses;
using System.Linq;
using UnityEngine;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class AssaultSkills
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void ApplyChanges(bool doNotLocalize = true)
        {
            // Quick Aim: Adding aim modification
            ApplyStatusAbilityDef quickAim = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("QuickAim_AbilityDef"));
            quickAim.UsesPerTurn = 2;
            BonusStatHolderStatusDef qaAccMod = Repo.GetAllDefs<BonusStatHolderStatusDef>().FirstOrDefault(b => b.name.Equals("E_AccuracyModifier [QuickAim_AbilityDef]"));
            qaAccMod.Value = -0.3f; // Acc bonus/malus to add, default 0.25f = +25%, new -0.3 = -30%
            ((AddAttackBoostStatusDef)quickAim.StatusDef).AdditionalStatusesToApply =
                ((AddAttackBoostStatusDef)quickAim.StatusDef).AdditionalStatusesToApply.Append(qaAccMod).ToArray();
            quickAim.ViewElementDef.Description = new LocalizedTextBind(
                "The Action Point cost of the next shot with a proficient weapon is reduced by 1 with -30% accuracy. Limited to 2 uses per turn.",
                doNotLocalize);

            // Kill'n'Run: Recive one free Dash move when killing an enemy, once per turn
            Create_KillAndRun(Repo, Config);

            // Onslaught (DeterminedAdvance_AbilityDef): Receiver can get only 1 onslaught per turn.
            // This below works on the target but he can be targeted again from another Assault without any response => the Assault loses 2 AP and the target gets nothing
            // Looking for a solution, maybe MC fuctionality could be a solution (thx to Iko)
            //TacEffectStatusDef onslaughtStatus = Repo.GetAllDefs<TacEffectStatusDef>().FirstOrDefault(c => c.name.Contains("E_Status [DeterminedAdvance_AbilityDef]"));
            //onslaughtStatus.SingleInstance = true;
            // .... delayed ....

            // Rapid Clearance: Until end of turn, after killing an enemy next attack cost -2AP
            // Get Rapid Clearance ability def
            ApplyStatusAbilityDef rapidClearance = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("RapidClearance_AbilityDef"));
            // Clone status apply effect from Vanish
            StatusEffectDef applyStatusEffect = Helper.CreateDefFromClone(
                Repo.GetAllDefs<StatusEffectDef>().FirstOrDefault(s => s.name.Equals("E_ApplyVanishStatusEffect [Vanish_AbilityDef]")),
                "8ea85920-588b-4e1d-a8e6-31ffbe9d3a02",
                "E_ApplyStatusEffect [RapidClearance_AbilityDef]");
            // Clone AP reduction statuses from QA
            AddAttackBoostStatusDef addAttackBoostStatus = Helper.CreateDefFromClone( // applies the AP reduction status only for the next attack
                Repo.GetAllDefs<AddAttackBoostStatusDef>().FirstOrDefault(s => s.name.Equals("E_Status [QuickAim_AbilityDef]")),
                "9385a73f-8d20-4022-acc1-9210e2e29b8f",
                "E_AttackBoostStatus [RapidClearance_AbilityDef]");
            ChangeAbilitiesCostStatusDef apReductionStatusEffect = Helper.CreateDefFromClone(
                Repo.GetAllDefs<ChangeAbilitiesCostStatusDef>().FirstOrDefault(s => s.name.Equals("E_AbilityCostModifier [QuickAim_AbilityDef]")),
                "e3062779-8f2f-4407-bc4f-a20f5c2d267b",
                "E_AbilityCostModifier [RapidClearance_AbilityDef]");
            // change properties and references
            apReductionStatusEffect.AbilityCostModification.RequiresProficientEquipment = false; // original QA true
            apReductionStatusEffect.AbilityCostModification.SkillTagCullFilter = new SkillTagDef[0]; // No restrictions, original QA disables melee and throwing grenades
            apReductionStatusEffect.AbilityCostModification.ActionPointMod = -0.5f; // -2 AP, original QA -1 AP
            addAttackBoostStatus.Visuals = rapidClearance.ViewElementDef;
            addAttackBoostStatus.SkillTagCullFilter = new SkillTagDef[0]; // No restrictions, original QA disables melee and throwing grenades
            addAttackBoostStatus.NumberOfAttacks = 2;
            addAttackBoostStatus.AdditionalStatusesToApply = new TacStatusDef[] { apReductionStatusEffect };
            applyStatusEffect.StatusDef = addAttackBoostStatus;
            rapidClearance.ViewElementDef.Description = new LocalizedTextBind("Until end of turn, after killing an enemy next attack cost -2AP", doNotLocalize);
            (rapidClearance.StatusDef as OnActorDeathEffectStatusDef).EffectDef = applyStatusEffect;

            // Barrage: 2 bursts with increased accuracy for 3 AP and 4 WP
            Create_Barrage(Repo, Config);
        }

        // New Kill'n'Run ability
        public static void Create_KillAndRun(DefRepository Repo, Settings Config)
        {
            string skillName = "KillAndRun_AbilityDef";
            bool doNotLocalize = Config.DoNotLocalizeChangedTexts;

            // Source to clone from for main ability: Inspire
            ApplyStatusAbilityDef inspireAbility = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("Inspire_AbilityDef"));

            // Create Neccessary RuntimeDefs
            ApplyStatusAbilityDef killAndRunAbility = Helper.CreateDefFromClone(
                inspireAbility,
                "3e0e991e-e0bf-4630-b2ca-110e68790fb7",
                skillName);
            AbilityCharacterProgressionDef progression = Helper.CreateDefFromClone(
                inspireAbility.CharacterProgressionData,
                "e3f25d2a-7668-4223-bb82-73a3f2f926aa",
                skillName);
            TacticalAbilityViewElementDef viewElement = Helper.CreateDefFromClone(
                inspireAbility.ViewElementDef,
                "8a740c8d-43b6-4ef1-9b93-b2c329566f27",
                skillName);
            OnActorDeathEffectStatusDef onActorDeathEffectStatus = Helper.CreateDefFromClone(
                inspireAbility.StatusDef as OnActorDeathEffectStatusDef,
                "7cfcb266-6730-4642-88d5-8a212104b9cc",
                "E_KillListenerStatus [" + skillName + "]");
            RepositionAbilityDef dashAbility = Helper.CreateDefFromClone( // Create an own Dash ability from standard Dash
                Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef")),
                "de8cd8a9-f2eb-4b8a-a408-a2a1913930c4",
                "KillAndRun_Dash_AbilityDef");
            TacticalTargetingDataDef dashTargetingData = Helper.CreateDefFromClone( // ... and clone its targeting data
                Repo.GetAllDefs<TacticalTargetingDataDef>().FirstOrDefault(t => t.name.Equals("E_TargetingData [Dash_AbilityDef]")),
                "18e86a2b-6031-4c84-a2a0-cb6ad2423b56",
                "KillAndRun_Dash_AbilityDef");
            StatusRemoverEffectDef statusRemoverEffect = Helper.CreateDefFromClone( // Borrow effect from Manual Control
                Repo.GetAllDefs<StatusRemoverEffectDef>().FirstOrDefault(a => a.name.Equals("E_RemoveStandBy [ManualControlStatus]")),
                "77b65001-7b75-4fbc-a89e-cf3e3e8ca69f",
                "E_StatusRemoverEffect [" + skillName + "]");
            AddAbilityStatusDef addAbiltyStatus = Helper.CreateDefFromClone( // Borrow status from Deplay Beacon (final mission)
                Repo.GetAllDefs<AddAbilityStatusDef>().FirstOrDefault(a => a.name.Equals("E_AddAbilityStatus [DeployBeacon_StatusDef]")),
                "ac18e0d8-530d-4077-b372-71c9f82e2b88",
                skillName);
            MultiStatusDef multiStatus = Helper.CreateDefFromClone( // Borrow multi status from Rapid Clearance
                Repo.GetAllDefs<MultiStatusDef>().FirstOrDefault(m => m.name.Equals("E_MultiStatus [RapidClearance_AbilityDef]")),
                "be7115e5-ce6b-47da-bead-311f3978f242",
                skillName);
            FirstMatchExecutionDef cameraAbility = Helper.CreateDefFromClone(
                Repo.GetAllDefs<FirstMatchExecutionDef>().FirstOrDefault(bd => bd.name.Equals("E_DashCameraAbility [NoDieCamerasTacticalCameraDirectorDef]")),
                "75d8137e-06f7-4840-8156-23366c4daea7",
                "E_KnR_Dash_CameraAbility [NoDieCamerasTacticalCameraDirectorDef]");
            cameraAbility.FilterDef = Helper.CreateDefFromClone(
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
            ShootAbilityDef barrageAbility = Helper.CreateDefFromClone(
                rageBurst,
                "fc5f5cf1-1349-42ff-adc4-515d7ceddde4",
                skillName);
            AbilityCharacterProgressionDef progression = Helper.CreateDefFromClone(
                rageBurst.CharacterProgressionData,
                "fa68ad15-a29b-4c66-b34a-fde332fc9d49",
                skillName);
            TacticalAbilityViewElementDef viewElement = Helper.CreateDefFromClone(
                rageBurst.ViewElementDef,
                "13005fbc-2613-4a01-9355-0701ae350ca5",
                skillName);
            SceneViewElementDef sceneView = Helper.CreateDefFromClone(
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
    }
}
