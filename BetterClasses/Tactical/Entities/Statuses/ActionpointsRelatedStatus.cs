using Base.Entities.Statuses;
using Base.Serialization.General;
using Harmony;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Tactical.Entities.Statuses;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PhoenixRising.BetterClasses.Tactical.Entities.Statuses
{
    [SerializeType(InheritCustomCreateFrom = typeof(Status))]
    public class ActionpointsRelatedStatus : TacStatus
    {
        public ActionpointsRelatedStatusDef ActionpointsRelatedStatusDef => BaseDef as ActionpointsRelatedStatusDef;

        private void ApplyModification(StatusStat apStat)
        {
            Logger.Debug("----------------------------------------------------", false);
            Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
            float num = (apStat.Value - ActionpointsRelatedStatusDef.ActionpointsLowBound) / (apStat.Max - ActionpointsRelatedStatusDef.ActionpointsLowBound);
            num = Mathf.Clamp01(num);
            float num2 = num * ActionpointsRelatedStatusDef.MaxBoost;
            //num2 = Mathf.Max(num2, 1f);
            Logger.Debug($"Calculated modification: {num2}");
            foreach (StatModificationTarget targetStat in ActionpointsRelatedStatusDef.StatModificationTargets)
            {
                BaseStat baseStat = TacticalActor.CharacterStats.TryGetStat(targetStat);
                Logger.Debug($"Stat before modification '{baseStat}'");
                Logger.Debug($"Stat modifications before modification '{baseStat.GetValueModifications().Join()}'");
                baseStat.RemoveStatModificationsWithSource(ActionpointsRelatedStatusDef, true);
                Logger.Debug($"Stat modifications after removing modification '{baseStat.GetValueModifications().Join()}'");
                if (baseStat is StatusStat)
                {
                    num2 += 1;
                    baseStat.AddStatModification(new StatModification(StatModificationType.MultiplyMax, ToString(), num2, ActionpointsRelatedStatusDef, num2), true);
                    baseStat.AddStatModification(new StatModification(StatModificationType.MultiplyRestrictedToBounds, ToString(), num2, ActionpointsRelatedStatusDef, num2), true);
                }
                else
                {
                    baseStat.AddStatModification(new StatModification(StatModificationType.Add, ToString(), num2, ActionpointsRelatedStatusDef, num2), true);
                }
                baseStat.ReapplyModifications();
                Logger.Debug($"Stat after modification '{baseStat}'");
                Logger.Debug($"Stat modifications after modification '{baseStat.GetValueModifications().Join()}'");
            }
            Logger.Debug("----------------------------------------------------", false);
        }

        private void ActionpointsChangedHandler(BaseStat stat, StatChangeType change, float prevValue, float unclampedValue)
        {
            Logger.Debug("----------------------------------------------------", false);
            Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
            Logger.Debug("----------------------------------------------------", false);
            StatusStat apStat = stat as StatusStat;
            ApplyModification(apStat);
        }

        public override void OnApply(StatusComponent statusComponent)
        {
            Logger.Debug("----------------------------------------------------", false);
            Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
            Logger.Debug("----------------------------------------------------", false);
            base.OnApply(statusComponent);
            if (TacticalActor == null)
            {
                RequestUnapply(statusComponent);
                return;
            }
            ApplyModification(TacticalActor.CharacterStats.Health);
            TacticalActor.CharacterStats.ActionPoints.StatChangeEvent += ActionpointsChangedHandler;
        }

        public override void OnUnapply()
        {
            Logger.Debug("----------------------------------------------------", false);
            Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
            Logger.Debug("----------------------------------------------------", false);
            base.OnUnapply();
            foreach (StatModificationTarget targetStat in ActionpointsRelatedStatusDef.StatModificationTargets)
            {
                BaseStat baseStat = TacticalActor.CharacterStats.TryGetStat(targetStat);
                baseStat.RemoveStatModificationsWithSource(ActionpointsRelatedStatusDef, true);
                baseStat.ReapplyModifications();
            }
            TacticalActor.CharacterStats.ActionPoints.StatChangeEvent -= ActionpointsChangedHandler;
        }
    }
}
