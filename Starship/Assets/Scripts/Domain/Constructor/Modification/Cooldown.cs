using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class Cooldown : IModification
    {
        public Cooldown(ModificationQuality quality)
        {
            _multiplier = quality.PowerMultiplier(2.0f, 1.5f, 1.2f, 0.9f, 0.75f, 0.5f);
            Quality = quality;
        }

		public string GetDescription(ILocalization localization) { return localization.GetString("$CooldownMod", Maths.Format.SignedPercent(_multiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats) { }

        public void Apply(ref DeviceStats device)
        {
            device.Cooldown *= _multiplier;
        }

        public void Apply(ref DroneBayStats droneBay) { }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition)
        {
            weapon.FireRate /= _multiplier;
        }

        public void Apply(ref WeaponStatModifier statModifier)
        {
            statModifier.FireRateMultiplier *= 1f/_multiplier;
        }

        private readonly float _multiplier;
    }
}
