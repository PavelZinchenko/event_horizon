using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class DroneSpeed : IModification
    {
        public DroneSpeed(ModificationQuality quality)
        {
            _multiplier = quality.PowerMultiplier(0.5f, 0.7f, 0.8f, 1.2f, 1.5f, 1.8f);
            Quality = quality;
        }

        public string GetDescription(ILocalization localization) { return localization.GetString("$DroneSpeedMod", Maths.Format.SignedPercent(_multiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats)
        {
            if (stats.DroneSpeedMultiplier.HasValue)
                stats.DroneSpeedMultiplier %= _multiplier;
        }

        public void Apply(ref DeviceStats device) { }
        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
        public void Apply(ref WeaponStatModifier statModifier) { }

        public void Apply(ref DroneBayStats droneBay)
        {
            droneBay.SpeedMultiplier += _multiplier - 1.0f;
        }

        private readonly float _multiplier;
    }
}
