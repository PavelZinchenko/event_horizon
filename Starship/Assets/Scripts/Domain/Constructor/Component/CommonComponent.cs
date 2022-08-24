using System.Linq;
using System.Collections.Generic;
using Constructor.Model;
using Constructor.Modification;
using Constructor.Ships;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Model;

namespace Constructor.Component
{
    public class CommonComponent : IComponent
    {
        public CommonComponent(GameDatabase.DataModel.Component data, int shipSize)
        {
            _shipSize = shipSize;
            _component = data;
        }

        public void UpdateStats(ref ShipEquipmentStats shipStats)
        {
            var stats = ShipEquipmentStats.FromComponent(_component.Stats, _component.Layout.CellCount);

            if (UpgradeLevel > 0)
            {
                var multiplier = 1f + 0.1f * UpgradeLevel;
                stats.ArmorPoints *= multiplier;
                stats.ShieldPoints *= multiplier;
                stats.EnergyResistance *= multiplier;
                stats.KineticResistance *= multiplier;
                stats.ThermalResistance *= multiplier;
            }

            if (_component.Device != null)
                stats.EnergyRechargeRate -= _component.Device.Stats.PassiveEnergyConsumption*_shipSize*0.05f;

            if (_component.DroneBay != null)
                stats.EnergyRechargeRate -= _component.DroneBay.Stats.PassiveEnergyConsumption*(0.5f + _shipSize*0.005f);

            if (Modification != null)
                Modification.Apply(ref stats);

            shipStats += stats;
        }

        public bool IsSuitable(IShipModel ship)
        {
            return CompatibilityChecker.IsCompatibleComponent(_component, ship);
        }

        public void UpdateWeaponPlatform(IWeaponPlatformStats stats)
        {
            var type = _component.Stats.AlterWeaponPlatform;
            if (type != PlatformType.Common)
                stats.ChangePlatformType(type);
        }

        public IEnumerable<WeaponData> Weapons
        {
            get
            {
                if (_component.Weapon != null && _component.Ammunition != null)
                {
                    var statModifiers = new WeaponStatModifier();

                    if (UpgradeLevel > 0)
                        statModifiers.DamageMultiplier *= 1f + 0.1f * UpgradeLevel;

                    if (Modification != null)
                        Modification.Apply(ref statModifiers);

                    yield return new WeaponData { Weapon = _component.Weapon, Ammunition = _component.Ammunition, StatModifier = statModifiers };
                }
            }
        }

        public IEnumerable<KeyValuePair<WeaponStats, AmmunitionObsoleteStats>> WeaponsObsolete
        {
            get
            {
                if (_component.Weapon != null && _component.AmmunitionObsolete != null)
                {
                    var weaponStats = _component.Weapon.Stats;
                    var ammoStats = _component.AmmunitionObsolete.Stats;

                    if (UpgradeLevel > 0)
                        ammoStats.Damage *= 1f + 0.1f * UpgradeLevel;

                    if (Modification != null)
                        Modification.Apply(ref weaponStats, ref ammoStats);

                    yield return new KeyValuePair<WeaponStats, AmmunitionObsoleteStats>(weaponStats, ammoStats);
                }
            }
        }

        public IEnumerable<DeviceStats> Devices
        {
            get
            {
                if (_component.Device != null)
                {
                    var stats = _component.Device.Stats;

                    stats.EnergyConsumption *= _shipSize*0.05f;

                    if (Modification != null)
                        Modification.Apply(ref stats);

                    yield return stats;
                }
            }
        }

        public IEnumerable<KeyValuePair<DroneBayStats, ShipBuild>> DroneBays
        {
            get
            {
                if (_component.DroneBay != null && _component.Drone != null)
                {
                    var stats = _component.DroneBay.Stats;

                    if (UpgradeLevel > 0)
                        stats.DamageMultiplier *= 1f + 0.1f*UpgradeLevel;

                    if (Modification != null)
                        Modification.Apply(ref stats);

                    yield return new KeyValuePair<DroneBayStats, ShipBuild>(stats, _component.Drone);
                }
            }
        }

        public ActivationType ActivationType
        {
            get
            {
                if (_component.Weapon != null)
                    return _component.Weapon.Stats.ActivationType;
                if (_component.Device != null)
                    return _component.Device.Stats.ActivationType;
                if (_component.DroneBay != null)
                    return _component.DroneBay.Stats.ActivationType;

                return ActivationType.None;
            }
        }

        public IModification Modification { get; set; }
        public int UpgradeLevel { get; set; }

        public IEnumerable<ComponentModType> SuitableModifications { get { return _component.PossibleModifications.Select(item => item.Type); } }

        private readonly int _shipSize;
        private readonly GameDatabase.DataModel.Component _component;
    }
}
