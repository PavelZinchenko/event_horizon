using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class DroneDamage : IModification
    {
        public DroneDamage(ModificationQuality quality)
        {
            _multiplier = quality.PowerMultiplier(0.5f, 0.7f, 0.8f, 1.3f, 1.8f, 2.5f);
            Quality = quality;
        }

        public string GetDescription(ILocalization localization) { return localization.GetString("$DamageMod", Maths.Format.SignedPercent(_multiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats)
        {
            if (stats.DroneDamageMultiplier.HasValue)
                stats.DroneDamageMultiplier %= _multiplier;
        }

        public void Apply(ref DeviceStats device) { }

        public void Apply(ref DroneBayStats droneBay)
        {
            droneBay.DamageMultiplier += _multiplier - 1f;
        }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
        public void Apply(ref WeaponStatModifier statModifier) {}

        private readonly float _multiplier;
    }
}
