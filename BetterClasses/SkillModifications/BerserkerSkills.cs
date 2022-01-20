using Base.Core;
using Base.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class BerserkerSkills
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void ApplyChanges(bool doNotLocalize = true)
        {
            // Dash changes -- cancelled, delayed
            //RepositionAbilityDef dash = Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef"));

            // Adrenaline Rush: 1 AP for one handed weapons and skills, no WP restriction

            // Melee Specialist: +10% damage instead of +25%

            // Personal Space: Until next turn, attack first enemy entering melee range
        }
    }
}
