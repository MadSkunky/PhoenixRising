using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Tactical.Entities.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.BetterClasses.SkillModifications
{
    class HeavySkills
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        public static void ApplyChanges(bool doNotLocalize = true)
        {
            // Return Fire: Fix to work on all classes
            TacticalAbilityDef returnFire = Repo.GetAllDefs<TacticalAbilityDef>().FirstOrDefault(tad => tad.name.Contains("ReturnFire_AbilityDef"));
            returnFire.ActorTags = new GameTagDef[0]; // Deletes all given tags => no restriction for any class

            // War Cry: -1 AP and -10% damage, doubled if WP of target < WP of caster

            // Rage Burst: Increase accuracy and cone angle

            // Dynamic Resistance: Copy from Acheron

            // Hunker Down: -25% incoming damage for 2 AP and 2 WP

            // Jetpack Control: 2 AP jump, 12 tiles range

            // Boom Blast: -30% range instead of +50%
        }
    }
}
