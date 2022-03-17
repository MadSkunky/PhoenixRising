using Base.Serialization.General;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PhoenixRising.BetterClasses.Tactical.Entities.Statuses
{
    [SerializeType(InheritCustomCreateFrom = typeof(TacStatusDef))]
    [CreateAssetMenu(fileName = "ModifyDamageKeywordStatusDef", menuName = "Defs/Statuses/ModifyDamageKeywordStatus")]
    public class AddDependentDamageKeywordsStatusDef : TacStatusDef
    {
        public DamageKeywordDef[] DamageKeywordDefs;
        public float BonusDamagePerc = 0f;
        public DamageMultiplierType DamageMultiplierType = DamageMultiplierType.Outgoing;
    }
}
