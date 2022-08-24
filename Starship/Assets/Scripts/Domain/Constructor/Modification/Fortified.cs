using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;
using UnityEngine;

namespace Constructor.Modification
{
    class Fortified : IModification
    {
        public Fortified(ModificationQuality quality)
        {
            _defenseMultiplier = quality.PowerMultiplier(0.5f, 0.7f, 0.8f, 1.2f, 1.5f, 2.0f);
            Quality = quality;
        }

		public string GetDescription(ILocalization localization) { return localization.GetString("$DefenseMod", Maths.Format.SignedPercent(_defenseMultiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats)
        {
            if (stats.ArmorPoints > 0)
                stats.ArmorPoints *= _defenseMultiplier;
            if (stats.EnergyResistance > 0)
                stats.EnergyResistance *= _defenseMultiplier;
            if (stats.ThermalResistance > 0)
                stats.ThermalResistance *= _defenseMultiplier;
            if (stats.KineticResistance > 0)
                stats.KineticResistance *= _defenseMultiplier;
            if (stats.ShieldPoints > 0)
                stats.ShieldPoints *= _defenseMultiplier;
        }

        public void Apply(ref DeviceStats device) { }
        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
        public void Apply(ref DroneBayStats droneBay) { }
        public void Apply(ref WeaponStatModifier statModifier) { }

        private readonly float _defenseMultiplier;
    }
}
