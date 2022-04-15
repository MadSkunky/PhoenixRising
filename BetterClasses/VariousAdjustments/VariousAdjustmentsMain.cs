using Base.Defs;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Tactical.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.BetterClasses.VariousAdjustments
{
    internal class VariousAdjustmentsMain
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            try
            {
                // Changes coding down from here
                VariousAdjustments.ApplyChanges();

                WeaponModifications.ApplyChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        // Harmony patches to deactivate autmatic standby in tactical missions
        [HarmonyPatch(typeof(TacticalActor), "TrySetStandBy")]
        internal static class BC_TacticalActor_TryGetStandBy_Patch
        {
            public static bool Prepare()
            {
                return Config.DeactivateTacticalAutoStandby;
            }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            // Using a radical way by overwriting the original method and always return (__result) false = never automatically activate standby
            private static bool Prefix(ref bool __result)
            {
                __result = false;
                return false;
            }
        }
        [HarmonyPatch(typeof(TacticalActorBase), "CanAct", new Type[0])]
        internal static class BC_TacticalActorBase_CanAct_Patch
        {
            public static bool Prepare()
            {
                return Config.DeactivateTacticalAutoStandby;
            }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static void Postfix(TacticalActorBase __instance, ref bool __result)
            {
                // Check if actor is from viewer faction (= player)
                if (__instance.IsFromViewerFaction)
                {
                    //  Set return value __result = true => no auto switch to other character after any action
                    __result = true;
                }
            }
        }
    }
}
