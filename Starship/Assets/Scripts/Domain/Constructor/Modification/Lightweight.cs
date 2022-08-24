using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class Lightweight : IModification
    {
        public Lightweight(ModificationQuality quality)
        {
            _weightMultiplier = quality.PowerMultiplier(2.0f, 1.5f, 1.2f, 0.8f, 0.6f, 0.5f);
            Quality = quality;
        }

		public string GetDescription(ILocalization localization) { return localization.GetString("$WeightMod", Maths.Format.SignedPercent(_weightMultiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats)
        {
            if (stats.Weight > 0)
                stats.Weight *= _weightMultiplier;
        }

        public void Apply(ref DeviceStats device) { }
        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
        public void Apply(ref DroneBayStats droneBay) { }
        public void Apply(ref WeaponStatModifier statModifier) { }

        private readonly float _weightMultiplier;
    }
}
