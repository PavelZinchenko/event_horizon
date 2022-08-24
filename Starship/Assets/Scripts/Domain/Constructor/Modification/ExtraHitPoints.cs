using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;
using UnityEngine;

namespace Constructor.Modification
{
    class ExtraHitPoints : IModification
    {
        public ExtraHitPoints(ModificationQuality quality)
        {
            _hitPoints = quality.PowerMultiplier(-3, -2, -1, 1, 3, 5);
            Quality = quality;
        }

		public string GetDescription(ILocalization localization) { return localization.GetString("$HitPointsMod", Maths.Format.SignedFloat(_hitPoints)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats)
        {
            stats.ArmorPoints += _hitPoints;
        }

        public void Apply(ref DeviceStats device) { }
        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
        public void Apply(ref DroneBayStats droneBay) { }
        public void Apply(ref WeaponStatModifier statModifier) { }

        private readonly float _hitPoints;
    }
}
