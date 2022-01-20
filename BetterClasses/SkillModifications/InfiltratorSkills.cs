using Base.Core;
using Base.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class InfiltratorSkills
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void ApplyChanges(bool doNotLocalize = true)
        {
            // Sneak Attack: Direct fire and melee +60 damage while not potted

            // Master Archer: Shoot your Crossbow 3 times at one target, reveal your position

            // Cautious: +10% stealth
        }
    }
}
