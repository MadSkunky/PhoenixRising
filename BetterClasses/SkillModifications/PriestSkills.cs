﻿using Base.Core;
using Base.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class PriestSkills
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void ApplyChanges(bool doNotLocalize = true)
        {
            // Biochemist: Paralysis, Poison and Viral damage increased 25%

            // Enrage (Mutog): Target Mutog becomes "Enraged"
        }
    }
}
