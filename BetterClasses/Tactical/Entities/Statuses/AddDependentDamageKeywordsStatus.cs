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

namespace PhoenixRising.BetterClasses.Tactical.Entities.Statuses
{
    [SerializeType(InheritCustomCreateFrom = typeof(Status))]
    public class AddDependentDamageKeywordsStatus : TacStatus
    {
        public AddDependentDamageKeywordsStatusDef AddDependentDamageKeywordStatusDef => BaseDef as AddDependentDamageKeywordsStatusDef;

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
                if (_appliedDamageKeywordPairs != null && _appliedDamageKeywordPairs.Count > 0)
                {
                    if (TacticalActor.GetBonusKeywords().Count() > 0)
                    {
                        foreach (DamageKeywordPair dkp in _appliedDamageKeywordPairs)
                        {
                            if (TacticalActor.GetBonusKeywords().Contains(dkp))
                            {
                                TacticalActor.RemoveDamageKeywordPair(dkp);
                            }
                        }
                    }
                    _appliedDamageKeywordPairs.Clear();
                }
                if (selectedEquipment is IDamageDealer damageDealer)
                {
                    Logger.Always($"Equipment is IDamageDealer, entering next step ...");
                    Logger.Always($"  Damage keywords on equipment {selectedEquipment}:");

                    foreach (DamageKeywordPair damageDealerDamageKeyword in damageDealer.GetDamagePayload().DamageKeywords)
                    {
                        Logger.Always($"    {damageDealerDamageKeyword}");

                        if (_dependentDamageKeywordDefs.Contains(damageDealerDamageKeyword.DamageKeywordDef))
                        {
                            Logger.Always($"      Damage keyword {damageDealerDamageKeyword.DamageKeywordDef} with {damageDealerDamageKeyword.Value} damage found in predefined array, adding another one of same type with {damageDealerDamageKeyword.Value * AddDependentDamageKeywordStatusDef.BonusDamagePerc} damage ...");
                            
                            DamageKeywordPair damageKeywordPair = new DamageKeywordPair()
                            {
                                DamageKeywordDef = damageDealerDamageKeyword.DamageKeywordDef,
                                Value = damageDealerDamageKeyword.Value * AddDependentDamageKeywordStatusDef.BonusDamagePerc
                            };
                            TacticalActor.AddDamageKeywordPair(damageKeywordPair);
                            _appliedDamageKeywordPairs.Add(damageKeywordPair);
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
            _dependentDamageKeywordDefs = AddDependentDamageKeywordStatusDef.DamageKeywordDefs;
            _appliedDamageKeywordPairs = new List<DamageKeywordPair>();
            switch (AddDependentDamageKeywordStatusDef.DamageMultiplierType)
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
            switch (AddDependentDamageKeywordStatusDef.DamageMultiplierType)
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

        private DamageKeywordDef[] _dependentDamageKeywordDefs;
        private List<DamageKeywordPair> _appliedDamageKeywordPairs;
    }
}
