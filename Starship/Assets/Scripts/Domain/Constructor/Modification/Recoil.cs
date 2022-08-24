using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class Recoil : IModification
    {
        public Recoil(ModificationQuality quality)
        {
            _weightMultiplier = quality.PowerMultiplier(2.5f, 2.0f, 1.5f, 0.8f, 0.5f, 0.2f);
            Quality = quality;
        }

        public string GetDescription(ILocalization localization) { return localization.GetString("$RecoilMod", Maths.Format.SignedPercent(_weightMultiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats) {}

        public void Apply(ref DeviceStats device) { }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition)
        {
            ammunition.Impulse *= _weightMultiplier;
            ammunition.Recoil *= _weightMultiplier;
        }

        public void Apply(ref WeaponStatModifier statModifier)
        {
            statModifier.WeightMultiplier *= _weightMultiplier;
        }

        public void Apply(ref DroneBayStats droneBay) { }

        private readonly float _weightMultiplier;
    }
}
