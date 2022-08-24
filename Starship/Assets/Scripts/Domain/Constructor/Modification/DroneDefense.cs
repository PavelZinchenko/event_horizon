using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;
using UnityEngine;

namespace Constructor.Modification
{
    class DroneDefense : IModification
    {
        public DroneDefense(ModificationQuality quality)
        {
            _multiplier = quality.PowerMultiplier(0.5f, 0.7f, 0.8f, 1.3f, 1.8f, 2.5f);
            Quality = quality;
        }

        public string GetDescription(ILocalization localization) { return localization.GetString("$DefenseMod", Maths.Format.SignedPercent(_multiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats)
        {
            if (stats.DroneDefenseMultiplier.HasValue)
                stats.DroneDefenseMultiplier %= _multiplier;
        }

        public void Apply(ref DeviceStats device) { }
        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
        public void Apply(ref WeaponStatModifier statModifier) { }

        public void Apply(ref DroneBayStats droneBay)
        {
            droneBay.DefenseMultiplier += _multiplier - 1f;
        }

        private readonly float _multiplier;
    }
}
