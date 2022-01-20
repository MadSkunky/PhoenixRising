using Base.Core;
using Base.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class TechnicianSkills
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void ApplyChanges(bool doNotLocalize = true)
        {
            // Electric Reinforcements: 10 tiles range, +10 armor, 1 AP and 3 WP

            // Stability: Gain 5% extra accuracy per remaining AP up to 20%

            // Amplify Pain: If your next attack deals special damage, double that damage (Bleeding, Paralysis, Viral, Poison, Fire, EMP, Sonic, Shock, Virophage)
        }
    }
}
