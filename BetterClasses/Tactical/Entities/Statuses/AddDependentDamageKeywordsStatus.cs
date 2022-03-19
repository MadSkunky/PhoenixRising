using Base.Entities.Statuses;
using Base.Serialization.General;
using Base.Utils.Maths;
using com.ootii.Collections;
using Harmony;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Tactical.Entities;
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

        private void OnEquipmentChanged(Equipment selectedEquipment)
        {
            try
            {
                Logger.Debug("----------------------------------------------------", false);
                Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");
                Logger.Debug($"Selected equipment: {selectedEquipment}");

                if (TacticalActor.GetBonusKeywords().Count() > 0)
                {
                    Logger.Debug($"  Already set bonus damage keywords on actor: {TacticalActor.GetBonusKeywords().Join()}");
                }
                if (_appliedDamageKeywordPairs != null && _appliedDamageKeywordPairs.Count > 0)
                {
                    if (TacticalActor.GetBonusKeywords().Count() > 0)
                    {
                        foreach (DamageKeywordPair dkp in _appliedDamageKeywordPairs)
                        {
                            if (TacticalActor.GetBonusKeywords().Contains(dkp))
                            {
                                Logger.Debug($"  Removing damage keywords on actor: {dkp}");
                                TacticalActor.RemoveDamageKeywordPair(dkp);
                            }
                        }
                    }
                    _appliedDamageKeywordPairs.Clear();
                }
                if (selectedEquipment is IDamageDealer damageDealer)
                {
                    Logger.Debug($"Equipment is IDamageDealer, entering next step ...");
                    Logger.Debug($"  Damage keywords on equipment {selectedEquipment}:");

                    foreach (DamageKeywordPair damageDealerDamageKeyword in damageDealer.GetDamagePayload().DamageKeywords)
                    {
                        Logger.Debug($"    {damageDealerDamageKeyword}");

                        if (_keywordMap.Keys.Contains(damageDealerDamageKeyword.DamageKeywordDef))
                        {
                            Logger.Debug($"      Damage keyword {damageDealerDamageKeyword} found in predefined array.");

                            float multiplier = AddDependentDamageKeywordStatusDef.BonusDamagePerc;
                            if (damageDealer.TryGetWeapon() is Weapon weapon)
                            {
                                if (weapon.WeaponDef.Tags.Contains(Repo.GetAllDefs<GameTagDef>().FirstOrDefault(gt => gt.name.Equals("ExplosiveWeapon_TagDef"))))
                                {
                                    multiplier *= 0.2f;
                                }
                            }
                            DamageKeywordPair damageKeywordPair = new DamageKeywordPair()
                            {
                                DamageKeywordDef = _keywordMap[damageDealerDamageKeyword.DamageKeywordDef],
                                Value = Mathf.Round(damageDealerDamageKeyword.Value * multiplier)
                            };
                            TacticalActor.AddDamageKeywordPair(damageKeywordPair);
                            _appliedDamageKeywordPairs.Add(damageKeywordPair);

                            Logger.Debug($"      Added keyword {damageKeywordPair}.");
                        }
                    }
                    if (TacticalActor.GetBonusKeywords().Count() > 0)
                    {
                        Logger.Debug($"  Resutling bonus damage keywords on actor: {TacticalActor.GetBonusKeywords().Join()}");
                    }
                    else
                    {
                        Logger.Debug($"  No bonus damage keywords set and added, probably none found in predifened array.");
                    }
                }

                Logger.Debug("----------------------------------------------------", false);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public override void OnApply(StatusComponent statusComponent)
        {
            Logger.Debug("----------------------------------------------------", false);
            Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");

            base.OnApply(statusComponent);
            if (TacticalActor == null)
            {
                RequestUnapply(statusComponent);
                return;
            }
            _dependentDamageKeywordDefs = AddDependentDamageKeywordStatusDef.DamageKeywordDefs;
            _keywordMap = new Dictionary<DamageKeywordDef, DamageKeywordDef>();
            string prefix = SharedSoloEffectorDamageKeywordsDataDef.Prefix;
            foreach (DamageKeywordDef dependentKeywordDef in _dependentDamageKeywordDefs)
            {
                string searchName = dependentKeywordDef.name.Replace(prefix, string.Empty);
                DamageKeywordDef keywordDef = Repo.GetAllDefs<DamageKeywordDef>().FirstOrDefault(dk => dk.name.Equals(searchName));
                _keywordMap.Add(keywordDef, dependentKeywordDef);
            }
            _appliedDamageKeywordPairs = new List<DamageKeywordPair>();
            TacticalActor.Equipments.EquipmentChangedEvent += OnEquipmentChanged;
            OnEquipmentChanged(TacticalActor.Equipments.SelectedEquipment);

            Logger.Debug("----------------------------------------------------", false);
        }

        public override void OnUnapply()
        {
            Logger.Debug("----------------------------------------------------", false);
            Logger.Debug($"'{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()' called ...");

            base.OnUnapply();
            TacticalActor.Equipments.EquipmentChangedEvent -= OnEquipmentChanged;

            Logger.Debug("----------------------------------------------------", false);
        }

        private DamageKeywordDef[] _dependentDamageKeywordDefs;
        private Dictionary<DamageKeywordDef, DamageKeywordDef> _keywordMap;
        private List<DamageKeywordPair> _appliedDamageKeywordPairs;
    }
}
