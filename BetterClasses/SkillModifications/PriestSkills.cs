using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Core;
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

            // Mutog Handler: 1AP 3WP, Target Mutog within 10 tiles gains "Dash" action. Limited to 1 use per turn
            Create_MutogHandler();
        }

        private static void Change_PsychicWard()
        {
            DamageMultiplierStatusDef pW = Repo.GetAllDefs<DamageMultiplierStatusDef>().FirstOrDefault(asa => asa.name.Equals("PsychicWard_StatusDef"));
            pW.Multiplier = 0.001f;
        }

        private static void Change_Biochemist()
        {
            Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }

        private static void Create_MutogHandler()
        {
            Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
        }
    }
}
