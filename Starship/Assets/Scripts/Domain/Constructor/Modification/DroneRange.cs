using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class DroneRange : IModification
    {
        public DroneRange(ModificationQuality quality)
        {
            _multiplier = quality.PowerMultiplier(0.5f, 0.7f, 0.8f, 1.2f, 1.5f, 1.8f);
            Quality = quality;
        }

        public string GetDescription(ILocalization localization) { return localization.GetString("$RangeMod", Maths.Format.SignedPercent(_multiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats)
        {
            if (stats.DroneRangeMultiplier.HasValue)
                stats.DroneRangeMultiplier %= _multiplier;
        }

        public void Apply(ref DeviceStats device) { }
        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
        public void Apply(ref WeaponStatModifier statModifier) { }

        public void Apply(ref DroneBayStats droneBay)
        {
            droneBay.Range *= _multiplier;
        }

        private readonly float _multiplier;
    }
}
