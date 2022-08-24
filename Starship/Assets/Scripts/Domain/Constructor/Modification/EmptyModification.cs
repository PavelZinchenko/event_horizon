using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;
using UnityEngine;

namespace Constructor.Modification
{
    class EmptyModification : IModification
    {
        public string GetDescription(ILocalization localization) { return string.Empty; }
        public ModificationQuality Quality { get { return ModificationQuality.N3; } }
        public void Apply(ref ShipEquipmentStats stats) { }
        public void Apply(ref DeviceStats device) { }
        public void Apply(ref DroneBayStats droneBay) { }
        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
        public void Apply(ref WeaponStatModifier statModifier) { }

        public static readonly EmptyModification Instance = new EmptyModification();
    }
}
