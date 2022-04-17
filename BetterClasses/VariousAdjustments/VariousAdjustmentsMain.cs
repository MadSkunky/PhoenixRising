using Base.Defs;
using Base.Entities.Statuses;
using Base.Utils.Maths;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        // -------------------------------------------------------------------------
        // Harmony patch(es) to fix that Project Hekate deletes the viral resistance ability of Mutoids
        // Cause is that both use the same ability and so Hekate faction status deletes the one from Mutoids
        // AddAbilityStatusDef.OnApply() ff
        // or create (clone) a new virus resistance ability for Hekate or Mutoids
        // -------------------------------------------------------------------------

        // -------------------------------------------------------------------------
        // Harmony patch to fix double reduction when resistances are present (mainly Nanotech)
        [HarmonyPatch(typeof(DamageOverTimeStatus), "LowerDamageOverTimeLevel")]
        internal static class BC_DamageOverTimeStatus_LowerDamageOverTimeLevel_Patch
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static bool Prefix(DamageOverTimeStatus __instance, float amount = 1f)
            {
                // This part doubles the reduction if any resistance is given (damage multiplier < 1)
                //if (Utl.LesserThan(__instance.GetDamageMultiplier(), 1f, 1E-05f))
                //{
                //    amount *= 2f;
                //}
                __instance.AddDamageOverTimeLevel(-amount);
                if (__instance.IntValue <= 0)
                {
                    __instance.RequestUnapply(__instance.StatusComponent);
                    return false;
                }
                _ = AccessTools.Method(typeof(TacStatus), "OnValueChanged").Invoke(__instance, null);
                return false;
            }
        }
        // -------------------------------------------------------------------------

        // -------------------------------------------------------------------------
        // Harmony patches to deactivate automatic standby and switch to another character in tactical missions
        [HarmonyPatch(typeof(TacticalActor), "TrySetStandBy")]
        internal static class BC_TacticalActor_TryGetStandBy_Patch
        {
            public static bool Prepare()
            {
                return Config.DeactivateTacticalAutoStandby;
            }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            // If actor NOT has ended turn (manually, OW, HD) set result to false and don't excecute original method TrySetStandBy() (return false)
            private static bool Prefix(TacticalActor __instance, ref bool __result)
            {
                return __instance.HasEndedTurn || (__result = false);
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
                StatusDef overWatch = Repo.GetAllDefs<StatusDef>().FirstOrDefault(sd1 => sd1.name.Equals("Overwatch_StatusDef"));
                StatusDef hunkerDown = Repo.GetAllDefs<StatusDef>().FirstOrDefault(sd2 => sd2.name.Equals("E_CloseQuatersStatus [HunkerDown_AbilityDef]"));
                bool flag = __instance.Status.HasStatus(overWatch) || __instance.Status.HasStatus(hunkerDown);
                // Check if actor is from viewer faction (= player) and don't used overwatch or hunker down
                if (__instance.IsFromViewerFaction && !flag)
                {
                    //  Set return value __result = true => no auto switch to other character after any action
                    __result = true;
                }
            }
        }
        // -------------------------------------------------------------------------
    }
}
