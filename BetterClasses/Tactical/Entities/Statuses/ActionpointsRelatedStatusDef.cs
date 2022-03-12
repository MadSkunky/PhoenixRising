﻿using Base.Serialization.General;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Tactical.Entities.Statuses;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PhoenixRising.BetterClasses.Tactical.Entities.Statuses
{
    [SerializeType(InheritCustomCreateFrom = typeof(TacStatusDef))]
    [CreateAssetMenu(fileName = "ActionpointsRelatedStatusDef", menuName = "Defs/Statuses/ActionpointsRelatedStatus")]
    public class ActionpointsRelatedStatusDef : TacStatusDef
    {
        public override void ValidateObject()
        {
            Logger.Debug("----------------------------------------------------", false);
            Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
            Logger.Debug("----------------------------------------------------", false);
            base.ValidateObject();
            if (StatModificationTargets.Any((x) => x == StatModificationTarget.ActionPoints))
            {
                Debug.LogError("ActionpointsRelatedStatus should not modify the Actionpoint stat of the Actors!", this);
            }
        }

        public float ActionpointsLowBound = 1f;

        public float MaxBoost = 1f;

        public StatModificationTarget[] StatModificationTargets;
    }
}
