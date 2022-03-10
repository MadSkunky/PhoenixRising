using Base.Core;
using Base.Defs;
using Base.UI;
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
    class TechnicianSkills
    {
        // Get config, definition repository and shared data
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        private static readonly bool doNotLocalize = BetterClassesMain.doNotLocalize;

        public static void ApplyChanges()
        {
            // Electric Reinforcements: 10 tiles range, +10 armor, 1 AP and 3 WP
            Change_ElectricReinforcements();

            // Stability: Gain 5% extra accuracy per remaining AP up to 20%
            Create_Stability();

            // Amplify Pain: If your next attack deals special damage, double that damage (Bleeding, Paralysis, Viral, Poison, Fire, EMP, Sonic, Shock, Virophage)
            Create_AmplifyPain();
        }

        private static void Change_ElectricReinforcements()
        {
            float armorBonus = 10f;
            ApplyStatusAbilityDef eR = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(asa => asa.name.Equals("ElectricReinforcement_AbilityDef"));
            ItemSlotStatsModifyStatusDef eRStatus = (ItemSlotStatsModifyStatusDef)eR.StatusDef;

            eR.TargetingDataDef.Origin.Range = 10;
            eR.ActionPointCost = 0.25f;
            eR.WillPointCost = 3;
            eR.ViewElementDef.Description = new LocalizedTextBind($"Give yourself and allies within 20 tiles a bonus of {armorBonus} armour for 1 turn. This effect does not stack.", doNotLocalize);
            eRStatus.StatsModifications[0].Value = armorBonus;
            eRStatus.StatsModifications[1].Value = armorBonus;
        }

        private static void Create_Stability()
        {
            Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
            Logger.Debug("----------------------------------------------------", false);
        }

        private static void Create_AmplifyPain()
        {
            Logger.Debug("'" + MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name + "()' not implemented yet!");
            Logger.Debug("----------------------------------------------------", false);
        }
    }
}
