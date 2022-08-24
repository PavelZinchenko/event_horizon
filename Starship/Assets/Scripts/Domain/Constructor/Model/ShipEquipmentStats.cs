using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;

namespace Constructor.Model
{
    public struct ShipEquipmentStats
    {
        public float ArmorPoints;
        public float ArmorRepairRate;
        public StatMultiplier ArmorRepairCooldownMultiplier;

        public float EnergyPoints;
        public float EnergyRechargeRate;
        public StatMultiplier EnergyRechargeCooldownMultiplier;

        public float ShieldPoints;
        public float ShieldRechargeRate;
        public StatMultiplier ShieldRechargeCooldownMultiplier;

        public float ArmorRepairBaseCooldown;
        public float HullRepairBaseCooldown;
        public float EnergyRechargeBaseCooldown;
        public float ShieldRechargeBaseCooldown;

        public float Weight;

        public float EnergyAbsorption;
        public float RammingDamage;
        public StatMultiplier RammingDamageMultiplier;

        public float KineticResistance;
        public float EnergyResistance;
        public float ThermalResistance;

        public float EnginePower;
        public float TurnRate;

        public bool Autopilot;

        public StatMultiplier DroneRangeMultiplier;
        public StatMultiplier DroneDamageMultiplier;
        public StatMultiplier DroneDefenseMultiplier;
        public StatMultiplier DroneSpeedMultiplier;
        public float DroneReconstructionSpeed;
        public StatMultiplier DroneReconstructionTimeMultiplier;

        public StatMultiplier WeaponFireRateMultiplier;
        public StatMultiplier WeaponDamageMultiplier;
        public StatMultiplier WeaponRangeMultiplier;
        public StatMultiplier WeaponEnergyCostMultiplier;

        public static ShipEquipmentStats FromComponent(ComponentStats component, int cellCount)
        {
            var stats = new ShipEquipmentStats();

            var multiplier = component.Type == ComponentStatsType.PerOneCell ? cellCount : 1.0f;

            stats.ArmorPoints = component.ArmorPoints * multiplier;
            stats.ArmorRepairRate = component.ArmorRepairRate * multiplier;
            stats.ArmorRepairCooldownMultiplier = new StatMultiplier(component.ArmorRepairCooldownModifier * multiplier);

            stats.EnergyPoints = component.EnergyPoints * multiplier;
            stats.EnergyRechargeRate = component.EnergyRechargeRate * multiplier;
            stats.EnergyRechargeCooldownMultiplier = new StatMultiplier(component.EnergyRechargeCooldownModifier * multiplier);

            stats.ShieldPoints = component.ShieldPoints * multiplier;
            stats.ShieldRechargeRate = component.ShieldRechargeRate * multiplier;
            stats.ShieldRechargeCooldownMultiplier = new StatMultiplier(component.ShieldRechargeCooldownModifier * multiplier);

            stats.Weight = multiplier * component.Weight;

            stats.RammingDamage = component.RammingDamage * multiplier;
            stats.EnergyAbsorption = component.EnergyAbsorption * multiplier;

            stats.KineticResistance = component.KineticResistance * multiplier;
            stats.EnergyResistance = component.EnergyResistance * multiplier;
            stats.ThermalResistance = component.ThermalResistance * multiplier;

            stats.EnginePower = component.EnginePower * multiplier;
            stats.TurnRate = component.TurnRate * multiplier;

            stats.Autopilot = component.Autopilot;

            stats.DroneRangeMultiplier = new StatMultiplier(component.DroneRangeModifier * multiplier);
            stats.DroneDamageMultiplier = new StatMultiplier(component.DroneDamageModifier * multiplier);
            stats.DroneDefenseMultiplier = new StatMultiplier(component.DroneDefenseModifier * multiplier);
            stats.DroneSpeedMultiplier = new StatMultiplier(component.DroneSpeedModifier * multiplier);
            stats.DroneReconstructionTimeMultiplier = new StatMultiplier(component.DroneBuildTimeModifier * multiplier);
            stats.DroneReconstructionSpeed = component.DronesBuiltPerSecond * multiplier;

            stats.WeaponFireRateMultiplier = new StatMultiplier(component.WeaponFireRateModifier * multiplier);
            stats.WeaponDamageMultiplier = new StatMultiplier(component.WeaponDamageModifier * multiplier);
            stats.WeaponRangeMultiplier = new StatMultiplier(component.WeaponRangeModifier * multiplier);
            stats.WeaponEnergyCostMultiplier = new StatMultiplier(component.WeaponEnergyCostModifier * multiplier);

            return stats;
        }

        public static ShipEquipmentStats operator +(ShipEquipmentStats first, ShipEquipmentStats second)
        {
            first.ArmorPoints += second.ArmorPoints;
            first.ArmorRepairRate += second.ArmorRepairRate;
            first.ArmorRepairCooldownMultiplier += second.ArmorRepairCooldownMultiplier;

            first.EnergyPoints += second.EnergyPoints;
            first.EnergyRechargeRate += second.EnergyRechargeRate;
            first.EnergyRechargeCooldownMultiplier += second.EnergyRechargeCooldownMultiplier;

            first.ShieldPoints += second.ShieldPoints;
            first.ShieldRechargeRate += second.ShieldRechargeRate;
            first.ShieldRechargeCooldownMultiplier += second.ShieldRechargeCooldownMultiplier;

            first.Weight += second.Weight;

            first.EnergyAbsorption += second.EnergyAbsorption;
            first.RammingDamage += second.RammingDamage;
            first.RammingDamageMultiplier += second.RammingDamageMultiplier;

            first.KineticResistance += second.KineticResistance;
            first.EnergyResistance += second.EnergyResistance;
            first.ThermalResistance += second.ThermalResistance;

            first.EnginePower += second.EnginePower;
            first.TurnRate += second.TurnRate;

            first.Autopilot |= second.Autopilot;

            first.DroneRangeMultiplier += second.DroneRangeMultiplier;
            first.DroneDamageMultiplier += second.DroneDamageMultiplier;
            first.DroneDefenseMultiplier += second.DroneDefenseMultiplier;
            first.DroneSpeedMultiplier += second.DroneSpeedMultiplier;
            first.DroneReconstructionSpeed += second.DroneReconstructionSpeed;
            first.DroneReconstructionTimeMultiplier += second.DroneReconstructionTimeMultiplier;

            first.WeaponFireRateMultiplier += second.WeaponFireRateMultiplier;
            first.WeaponDamageMultiplier += second.WeaponDamageMultiplier;
            first.WeaponRangeMultiplier += second.WeaponRangeMultiplier;
            first.WeaponEnergyCostMultiplier += second.WeaponEnergyCostMultiplier;

            return first;
        }
    }
}
