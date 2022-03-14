using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class PriestSkills
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            // Psychic Ward: Fix psychic damage immunity or at least resistance, otherwise at least change description
            Change_PsychicWard();
            
            // Biochemist: Paralysis, Poison and Viral damage increased 25%
            Change_Biochemist();

            // Lay Waste: 1 AP, 3 WP, If your current Willpower score is higher than target's deal 30 damage for each point of WP difference
            Create_LayWaste();
        }

        private static void Change_PsychicWard()
        {
            DamageMultiplierStatusDef pW = Repo.GetAllDefs<DamageMultiplierStatusDef>().FirstOrDefault(asa => asa.name.Equals("PsychicWard_StatusDef"));
            pW.Multiplier = 0.001f;
        }

        private static void Change_Biochemist()
        {
            float damageMod = 1.25f;
            string skillName = "BC_Biochemist_AbilityDef";

            ApplyStatusAbilityDef source = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(dma => dma.name.Equals("BodypartDamageMultiplier_AbilityDef"));

            ApplyStatusAbilityDef Biochemist = Helper.CreateDefFromClone(
                source,
                "87d0f9a4-0d26-4c2a-badb-cef90ae746a5",
                skillName);
            Biochemist.CharacterProgressionData = Helper.CreateDefFromClone(
                source.CharacterProgressionData,
                "1845e667-c394-4248-bf21-10fa661aeb6f",
                skillName);
            Biochemist.ViewElementDef = Helper.CreateDefFromClone(
                source.ViewElementDef,
                "683abf9c-46ea-4406-a943-ae3864fd1ce4",
                skillName);
            Biochemist.StatusDef = Helper.CreateDefFromClone(
                source.StatusDef,
                "",
                skillName);
            (Biochemist.StatusDef as TacStatusDef).Visuals = Biochemist.ViewElementDef;

            Biochemist.ViewElementDef.DisplayName1.LocalizationKey = "PR_BC_BIOCHEMIST";
            Biochemist.ViewElementDef.Description.LocalizationKey = "PR_BC_BIOCHEMIST_DESC";
            
            //foreach (ApplyStatusAbilityDef temp in Repo.GetAllDefs<ApplyStatusAbilityDef>().Where(asa => asa.Active == false))
            //{
            //    Logger.Always(temp.name);
            //}
        }

        private static void Create_LayWaste()
        {
            Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
            Logger.Debug("----------------------------------------------------", false);
        }
    }
}
