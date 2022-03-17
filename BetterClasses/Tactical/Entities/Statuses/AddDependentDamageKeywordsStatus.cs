using Base.Defs;
using Base.Entities.Statuses;
using Base.Serialization.General;
using com.ootii.Collections;
using Harmony;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixRising.BetterClasses.Tactical.Entities.Statuses
{
    [SerializeType(InheritCustomCreateFrom = typeof(Status))]
    public class AddDependentDamageKeywordsStatus : TacStatus
    {
        public AddDependentDamageKeywordsStatusDef ModifyDamageKeywordStatusDef => BaseDef as AddDependentDamageKeywordsStatusDef;

        private void OnEquipmentChanged(Equipment selectedEquipment)
        {
            try
            {
                Logger.Always("----------------------------------------------------", false);
                Logger.Always($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
                Logger.Always($"Selected equipment: {selectedEquipment}");

                if (TacticalActor.GetBonusKeywords().Count() > 0)
                {
                    Logger.Always($"  Already set bonus damage keywords on actor: {TacticalActor.GetBonusKeywords().Join()}");
                }
                if (_damageKeywordPairs != null && _damageKeywordPairs.Count > 0)
                {
                    if (TacticalActor.GetBonusKeywords().Count() > 0)
                    {
                        foreach (DamageKeywordPair dkp in _damageKeywordPairs)
                        {
                            if (TacticalActor.GetBonusKeywords().Contains(dkp))
                            {
                                TacticalActor.RemoveDamageKeywordPair(dkp);
                            }
                        }
                    }
                    _damageKeywordPairs.Clear();
                }
                if (selectedEquipment is IDamageDealer damageDealer
                    && damageDealer.GetDamagePayload().DamageKeywords.Any(dk => _dependentKeywords.Contains(dk.DamageKeywordDef)))
                {
                    Logger.Always($"Equipment is IDamageDealer, entering next step ...");
                    Logger.Always($"  Damage keywords on equipment {selectedEquipment}:");

                    foreach (DamageKeywordPair dkp in damageDealer.GetDamagePayload().DamageKeywords)
                    {
                        Logger.Always($"    {dkp}");

                        if (_dependentKeywords.Contains(dkp.DamageKeywordDef))
                        {
                            Logger.Always($"      Damage keyword {dkp.DamageKeywordDef} with {dkp.Value} damage found in predefined array, adding another one of same type with {dkp.Value * ModifyDamageKeywordStatusDef.BonusDamagePerc} damage ...");
                            
                            DamageKeywordPair damageKeywordPair = new DamageKeywordPair()
                            {
                                DamageKeywordDef = dkp.DamageKeywordDef,
                                Value = dkp.Value * ModifyDamageKeywordStatusDef.BonusDamagePerc
                            };
                            TacticalActor.AddDamageKeywordPair(damageKeywordPair);
                            _damageKeywordPairs.Add(damageKeywordPair);
                        }
                    }
                    if (TacticalActor.GetBonusKeywords().Count() > 0)
                    {
                        Logger.Always($"  Resutling bonus damage keywords on actor: {TacticalActor.GetBonusKeywords().Join()}");
                    }
                    else
                    {
                        Logger.Always($"  No bonus damage keywords set and added, probably none found in predifened array.");
                    }
                }

                Logger.Always("----------------------------------------------------", false);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public override void OnApply(StatusComponent statusComponent)
        {
            Logger.Always("----------------------------------------------------", false);
            Logger.Always($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");

            base.OnApply(statusComponent);
            if (TacticalActor == null)
            {
                RequestUnapply(statusComponent);
                return;
            }
            _dependentKeywords = ModifyDamageKeywordStatusDef.DamageKeywordDefs;
            _damageKeywordPairs = new List<DamageKeywordPair>();
            switch (ModifyDamageKeywordStatusDef.DamageMultiplierType)
            {
                case DamageMultiplierType.Outgoing:
                    TacticalActor.Equipments.EquipmentChangedEvent += OnEquipmentChanged;
                    OnEquipmentChanged(TacticalActor.Equipments.SelectedEquipment);
                    break;
                case DamageMultiplierType.Incoming:
                case DamageMultiplierType.All:
                default:
                    break;
            }

            Logger.Always("----------------------------------------------------", false);
        }

        public override void OnUnapply()
        {
            Logger.Always("----------------------------------------------------", false);
            Logger.Always($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");

            base.OnUnapply();
            switch (ModifyDamageKeywordStatusDef.DamageMultiplierType)
            {
                case DamageMultiplierType.Outgoing:
                    TacticalActor.Equipments.EquipmentChangedEvent -= OnEquipmentChanged;
                    break;
                case DamageMultiplierType.Incoming:
                case DamageMultiplierType.All:
                default:
                    break;
            }

            Logger.Always("----------------------------------------------------", false);
        }

        private DamageKeywordDef[] _dependentKeywords;
        private List<DamageKeywordPair> _damageKeywordPairs;
    }
}
