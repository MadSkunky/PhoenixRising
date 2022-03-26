using Base.Entities.Statuses;
using Base.Serialization.General;
using Base.Utils.Maths;
using com.ootii.Collections;
using Harmony;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixRising.BetterClasses.Tactical.Entities.DamageKeywords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PhoenixRising.BetterClasses.Tactical.Entities.Statuses
{
    [SerializeType(InheritCustomCreateFrom = typeof(Status))]
    public class AddDependentDamageKeywordsStatus : TacStatus
    {
        public AddDependentDamageKeywordsStatusDef AddDependentDamageKeywordStatusDef => BaseDef as AddDependentDamageKeywordsStatusDef;

        public override void OnApply(StatusComponent statusComponent)
        {
            Logger.Debug("----------------------------------------------------", false);
            Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");

            base.OnApply(statusComponent);
            if (TacticalActor == null)
            {
                RequestUnapply(statusComponent);

                Logger.Debug("----------------------------------------------------", false);

                return;
            }

            Logger.Debug($"Actor: {TacticalActor}");

            _dependentDamageKeywordDefs = AddDependentDamageKeywordStatusDef.DamageKeywordDefs;
            _keywordMap = new Dictionary<DamageKeywordDef, DamageKeywordDef>();
            string prefix = SharedSoloEffectorDamageKeywordsDataDef.Prefix;
            foreach (DamageKeywordDef dependentKeywordDef in _dependentDamageKeywordDefs)
            {
                string searchName = dependentKeywordDef.name.Replace(prefix, string.Empty);
                DamageKeywordDef keywordDef = Repo.GetAllDefs<DamageKeywordDef>().FirstOrDefault(dk => dk.name.Equals(searchName));
                _keywordMap.Add(keywordDef, dependentKeywordDef);
            }
            _appliedDamageKeywordValues = new Dictionary<DamageKeywordDef, float>();
            _trackedAbility = null;
            TacticalActor.Equipments.EquipmentChangedEvent += OnEquipmentChanged;
            TacticalLevel.AbilityActivatingEvent += OnAbilityActivated;
            TacticalLevel.AbilityExecutedEvent += OnAbilityExecuted;
            OnEquipmentChanged(TacticalActor.Equipments.SelectedEquipment);

            Logger.Debug("----------------------------------------------------", false);
        }

        public override void OnUnapply()
        {
            Logger.Debug("----------------------------------------------------", false);
            Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
            Logger.Debug($"Actor: {TacticalActor}");

            base.OnUnapply();
            TacticalActor.Equipments.EquipmentChangedEvent -= OnEquipmentChanged;
            TacticalLevel.AbilityActivatingEvent -= OnAbilityActivated;
            TacticalLevel.AbilityExecutedEvent -= OnAbilityExecuted;
            RemoveKeywords();

            Logger.Debug("----------------------------------------------------", false);
        }

        private void OnEquipmentChanged(Equipment selectedEquipment)
        {
            try
            {
                Logger.Debug("----------------------------------------------------", false);
                Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
                Logger.Debug($"Actor: {TacticalActor}");
                Logger.Debug($"Selected equipment: {selectedEquipment}");

                if (TacticalActor.GetBonusKeywords().Count() > 0)
                {
                    Logger.Debug($"  Already set bonus damage keywords on actor: {TacticalActor.GetBonusKeywords().Join()}");
                }
                RemoveKeywords();
                if (selectedEquipment is IDamageDealer damageDealer)
                {
                    AddKeywords(damageDealer);
                }

                Logger.Debug("----------------------------------------------------", false);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnAbilityActivated(TacticalAbility ability, object parameter)
        {
            try
            {
                if (ability.TacticalActor == TacticalActor
                    && ability != TacticalActor.Equipments.SelectedWeapon?.DefaultShootAbility
                    && ability is IDamageDealer damageDealer)
                {
                    Logger.Debug("----------------------------------------------------", false);
                    Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
                    Logger.Debug($"Actor: {TacticalActor}");
                    Logger.Debug($"Activated ability: {ability}");

                    if (TacticalActor.GetBonusKeywords().Count() > 0)
                    {
                        Logger.Debug($"  Already set bonus damage keywords on actor: {TacticalActor.GetBonusKeywords().Join()}");
                    }
                    RemoveKeywords();
                    AddKeywords(damageDealer);
                    _trackedAbility = ability;

                    Logger.Debug("----------------------------------------------------", false);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnAbilityExecuted(TacticalAbility ability, object parameter)
        {
            try
            {
                if (ability.TacticalActor == TacticalActor
                    && ability != TacticalActor.Equipments.SelectedWeapon?.DefaultShootAbility
                    && ability == _trackedAbility)
                {
                    Logger.Debug("----------------------------------------------------", false);
                    Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
                    Logger.Debug($"Actor: {TacticalActor}");
                    Logger.Debug($"Executed ability: {ability}");

                    if (TacticalActor.GetBonusKeywords().Count() > 0)
                    {
                        Logger.Debug($"  Already set bonus damage keywords on actor: {TacticalActor.GetBonusKeywords().Join()}");
                    }
                    RemoveKeywords();
                    OnEquipmentChanged(TacticalActor.Equipments.SelectedEquipment);
                    _trackedAbility = null;

                    Logger.Debug("----------------------------------------------------", false);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void AddKeywords(IDamageDealer damageDealer)
        {
            Logger.Debug($"Equipment is IDamageDealer, entering next step ...");
            Logger.Debug($"  Damage keywords on {damageDealer}:");

            foreach (DamageKeywordPair damageDealerDamageKeyword in damageDealer.GetDamagePayload().DamageKeywords)
            {
                Logger.Debug($"    {damageDealerDamageKeyword}");

                if (_keywordMap.Keys.Contains(damageDealerDamageKeyword.DamageKeywordDef))
                {
                    Logger.Debug($"      Predefined damage keyword {damageDealerDamageKeyword} found.");

                    float multiplier = AddDependentDamageKeywordStatusDef.BonusDamagePerc;
                    if (damageDealer.TryGetWeapon() is Weapon weapon)
                    {
                        if (weapon.WeaponDef.name.Equals("SY_PoisonGrenade_WeaponDef"))
                        {
                            multiplier *= 0.2f;
                        }
                    }
                    DamageKeywordDef damageKeywordDef = _keywordMap[damageDealerDamageKeyword.DamageKeywordDef];
                    float damageKeywordValue = Mathf.Round(damageDealerDamageKeyword.Value * multiplier);

                    Logger.Debug($"      Calculated bonus damage value: {damageKeywordValue}.");

                    _appliedDamageKeywordValues.Add(damageKeywordDef, damageKeywordValue);
                    float value = damageKeywordValue;
                    DamageKeywordPair bonusKeyword = TacticalActor.GetBonusKeywords().FirstOrDefault(dkp => dkp.DamageKeywordDef == damageKeywordDef);
                    if (bonusKeyword != null)
                    {
                        value += bonusKeyword.Value;
                        TacticalActor.RemoveDamageKeywordPair(bonusKeyword);

                        Logger.Debug($"      Existing bonus keyword {bonusKeyword} with same DamageKeyword on actor found and removed, recalculated damage value for new bonus keyword: {value}.");
                    }
                    DamageKeywordPair damageKeywordPair = new DamageKeywordPair()
                    {
                        DamageKeywordDef = damageKeywordDef,
                        Value = value
                    };
                    TacticalActor.AddDamageKeywordPair(damageKeywordPair);

                    Logger.Debug($"      New bonus keyword {damageKeywordPair} added on actor.");
                }
            }
            if (TacticalActor.GetBonusKeywords().Count() > 0)
            {
                Logger.Debug($"  Resulting bonus damage keywords on actor: {TacticalActor.GetBonusKeywords().Join()}");
            }
            else
            {
                Logger.Debug($"  No bonus damage keywords set and added, probably none found in predefined array.");
            }
        }

        private void RemoveKeywords()
        {
            if (_appliedDamageKeywordValues != null && _appliedDamageKeywordValues.Count > 0)
            {
                if (TacticalActor.GetBonusKeywords().Count() > 0)
                {
                    foreach (KeyValuePair<DamageKeywordDef, float> kvp in _appliedDamageKeywordValues)
                    {
                        DamageKeywordPair bonusKeyword = TacticalActor.GetBonusKeywords().FirstOrDefault(dkp => dkp.DamageKeywordDef == kvp.Key);
                        if (bonusKeyword != null)
                        {
                            if (bonusKeyword.Value == kvp.Value)
                            {
                                TacticalActor.RemoveDamageKeywordPair(bonusKeyword);

                                Logger.Debug($"  Removed damage keyword {bonusKeyword} on actor.");
                            }
                            else
                            {
                                DamageKeywordPair newKeyword = new DamageKeywordPair()
                                {
                                    DamageKeywordDef = bonusKeyword.DamageKeywordDef,
                                    Value = bonusKeyword.Value - kvp.Value
                                };
                                TacticalActor.RemoveDamageKeywordPair(bonusKeyword);
                                TacticalActor.AddDamageKeywordPair(newKeyword);

                                Logger.Debug($"  Changed damage keyword {bonusKeyword} to {newKeyword}");
                            }
                        }
                    }
                }
                _appliedDamageKeywordValues.Clear();
            }
        }

        private DamageKeywordDef[] _dependentDamageKeywordDefs;
        private Dictionary<DamageKeywordDef, DamageKeywordDef> _keywordMap;
        private Dictionary<DamageKeywordDef, float> _appliedDamageKeywordValues;
        private TacticalAbility _trackedAbility;
    }
}
