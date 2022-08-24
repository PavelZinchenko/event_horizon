using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class Damage : IModification
    {
        public Damage(ModificationQuality quality)
        {
            _multiplier = quality.PowerMultiplier(0.5f, 0.7f, 0.8f, 1.2f, 1.5f, 2.0f);
            Quality = quality;
        }

		public string GetDescription(ILocalization localization) { return localization.GetString("$DamageMod", Maths.Format.SignedPercent(_multiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats) { }
        public void Apply(ref DeviceStats device) { }
        public void Apply(ref DroneBayStats droneBay) { }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition)
        {
            ammunition.Damage *= _multiplier;
        }

        public void Apply(ref WeaponStatModifier statModifier)
        {
            statModifier.DamageMultiplier *= _multiplier;
        }

        private readonly float _multiplier;
    }
}
