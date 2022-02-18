using Base.Defs;
using PhoenixPoint.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.BetterClasses.VariousAdjustments
{
    class VariousAdjustmentsMain
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
    }
}
