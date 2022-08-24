using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;
using UnityEngine;

namespace Constructor.Modification
{
    class LowEnergyCost : IModification
    {
        public LowEnergyCost(ModificationQuality quality)
        {
            _energyMultiplier = quality.PowerMultiplier(2.0f, 1.5f, 1.2f, 0.9f, 0.75f, 0.5f);
            Quality = quality;
        }

		public string GetDescription(ILocalization localization) { return localization.GetString("$EnergyCostMod", Maths.Format.SignedPercent(_energyMultiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats)
        {
            if (stats.EnergyRechargeRate < 0)
                stats.EnergyRechargeRate *= _energyMultiplier;
        }

        public void Apply(ref DeviceStats device)
        {
            if (device.EnergyConsumption > 0)
                device.EnergyConsumption *= _energyMultiplier;
        }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition)
        {
            if (ammunition.EnergyCost > 0)
                ammunition.EnergyCost *= _energyMultiplier;
        }

        public void Apply(ref WeaponStatModifier statModifier)
        {
            statModifier.EnergyCostMultiplier *= _energyMultiplier;
        }

        public void Apply(ref DroneBayStats droneBay)
        {
            if (droneBay.EnergyConsumption > 0)
                droneBay.EnergyConsumption *= _energyMultiplier;
        }

        private readonly float _energyMultiplier;
    }
}
