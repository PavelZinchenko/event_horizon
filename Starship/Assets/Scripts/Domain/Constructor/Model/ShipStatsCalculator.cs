using System.Collections.Generic;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using UnityEngine;

namespace Constructor.Model
{
    public interface IShipStats
    {
        ColorScheme ShipColor { get; }
        ColorScheme TurretColor { get; }

        StatMultiplier DamageMultiplier { get; }
        StatMultiplier ArmorMultiplier { get; }
        StatMultiplier ShieldMultiplier { get; }

        float ArmorPoints { get; }
        float EnergyPoints { get; }
        float ShieldPoints { get; }

        float EnergyRechargeRate { get; }
        float ShieldRechargeRate { get; }
        float ArmorRepairRate { get; }

        Layout Layout { get; }
        float Weight { get; }
        float EnginePower { get; }
        float TurnRate { get; }

        StatMultiplier WeaponDamageMultiplier { get; }
        StatMultiplier WeaponFireRateMultiplier { get; }
        StatMultiplier WeaponEnergyCostMultiplier { get; }
        StatMultiplier WeaponRangeMultiplier { get; }

        StatMultiplier DroneDamageMultiplier { get; }
        StatMultiplier DroneDefenseMultiplier { get; }
        StatMultiplier DroneSpeedMultiplier { get; }
        StatMultiplier DroneRangeMultiplier { get; }

        float EnergyAbsorption { get; }
        float RammingDamage { get; }
        float RammingDamageMultiplier { get; }

        float ArmorRepairCooldown { get; }
        float EnergyRechargeCooldown { get; }
        float ShieldRechargeCooldown { get; }

        float EnergyResistance { get; }
        float KineticResistance { get; }
        float ThermalResistance { get; }

        float EnergyAbsorptionPercentage { get; }
        float KineticResistancePercentage { get; }
        float EnergyResistancePercentage { get; }
        float ThermalResistancePercentage { get; }

        bool Autopilot { get; }
        bool TargetingSystem { get; }

        float DroneBuildSpeed { get; }
        float DroneBuildTime { get; }

        SpriteId IconImage { get; }
        SpriteId ModelImage { get; }
        float ModelScale { get; }
        Color EngineColor { get; }
        ShipCategory ShipCategory { get; }
        IEnumerable<Engine> Engines { get; }
    }

    public class ShipStatsCalculator : IShipStats
    {
        public ShipStatsCalculator(Ship ship, ShipSettings settings)
        {
            _ship = ship;
            ShipSettings = settings;
        }

        public ShipSettings ShipSettings { get; }

        public ShipBaseStats BaseStats;
        public ShipEquipmentStats EquipmentStats;
        public ShipBonuses Bonuses;

        public ColorScheme ShipColor { get; set; }
        public ColorScheme TurretColor { get; set; }

        public StatMultiplier DamageMultiplier => Bonuses.DamageMultiplier;
        public StatMultiplier ArmorMultiplier => Bonuses.ArmorPointsMultiplier;
        public StatMultiplier ShieldMultiplier => Bonuses.ShieldPointsMultiplier;

        public Layout Layout => BaseStats.Layout;

        public int CellCount => BaseStats.Layout.CellCount;

        public ImmutableCollection<Device> BuiltinDevices => BaseStats.BuiltinDevices;

        public float ArmorPoints => ArmorPointsWithoutBonuses * Bonuses.ArmorPointsMultiplier.Value;

        private float ArmorPointsWithoutBonuses
        {
            get
            {
                var basePoints = (ShipSettings.BaseArmorPoints + ShipSettings.ArmorPointsPerCell * CellCount) * BaseStats.BaseArmorMultiplier.Value;
                var totalPoints = basePoints + EquipmentStats.ArmorPoints;
                return totalPoints >= 1 ? totalPoints : 0;
            }
        }

        public float EnergyPoints => (ShipSettings.BaseEnergyPoints + EquipmentStats.EnergyPoints) * Bonuses.EnergyMultiplier.Value;
        public float ShieldPoints => EquipmentStats.ShieldPoints * Bonuses.ShieldPointsMultiplier.Value;

        public float EnergyRechargeRate => EquipmentStats.EnergyRechargeRate + ShipSettings.BaseEnergyRechargeRate;
        public float ShieldRechargeRate => (EquipmentStats.ShieldRechargeRate + ShipSettings.BaseShieldRechargeRate) * Bonuses.ShieldPointsMultiplier.Value * Bonuses.ShieldRechargeMultiplier.Value;

        public float ArmorRepairRate
        {
            get
            {
                var regeneration = _ship.Regeneration ? 0.01f + BaseStats.RegenerationRate : BaseStats.RegenerationRate;
                return EquipmentStats.ArmorRepairRate * Bonuses.ArmorPointsMultiplier.Value + Mathf.Max(0, ArmorPoints * regeneration);
            }
        }

        public float EnergyResistance
        {
            get
            {
                var resistance = EquipmentStats.EnergyResistance + (_ship.EnergyResistance + Bonuses.ExtraEnergyResistance) * (ArmorPointsWithoutBonuses + EquipmentStats.EnergyResistance);
                return resistance * Bonuses.ArmorPointsMultiplier.Value * BaseStats.EnergyResistanceMultiplier.Value;
            }
        }

        public float KineticResistance
        {
            get
            {
                var resistance = EquipmentStats.KineticResistance + (_ship.KineticResistance + Bonuses.ExtraKineticResistance) * (ArmorPointsWithoutBonuses + EquipmentStats.KineticResistance);
                return resistance * Bonuses.ArmorPointsMultiplier.Value * BaseStats.KineticResistanceMultiplier.Value;
            }
        }

        public float ThermalResistance
        {
            get
            {
                var resistance = EquipmentStats.ThermalResistance + (_ship.HeatResistance + Bonuses.ExtraHeatResistance) * (ArmorPointsWithoutBonuses + EquipmentStats.ThermalResistance);
                return resistance * Bonuses.ArmorPointsMultiplier.Value * BaseStats.HeatResistanceMultiplier.Value;
            }
        }

        public float ArmorRepairCooldown => (EquipmentStats.ArmorRepairBaseCooldown + ShipSettings.ArmorRepairCooldown) * Mathf.Max(EquipmentStats.ArmorRepairCooldownMultiplier.Value, 0);
        public float EnergyRechargeCooldown => (EquipmentStats.EnergyRechargeBaseCooldown + ShipSettings.EnergyRechargeCooldown) * Mathf.Max(EquipmentStats.EnergyRechargeCooldownMultiplier.Value, 0);
        public float ShieldRechargeCooldown => (EquipmentStats.ShieldRechargeBaseCooldown + ShipSettings.ShieldRechargeCooldown) * Mathf.Max(EquipmentStats.ShieldRechargeCooldownMultiplier.Value, 0);

        public float EnergyAbsorptionPercentage => EnergyAbsorption / (ArmorPoints + EnergyAbsorption);
        public float KineticResistancePercentage => KineticResistance / (ArmorPoints + KineticResistance);
        public float EnergyResistancePercentage => EnergyResistance / (ArmorPoints + EnergyResistance);
        public float ThermalResistancePercentage => ThermalResistance / (ArmorPoints + ThermalResistance);

        public float Weight
        {
            get
            {
                var multiplier = BaseStats.BaseWeightMultiplier.Value;
                var minWeight = multiplier * ShipSettings.MinimumWeightPerCell * CellCount;
                return Mathf.Max(minWeight, EquipmentStats.Weight + ShipSettings.DefaultWeightPerCell * CellCount * multiplier) / 1000f;
            }
        }

        public float EnginePower
        {
            get
            {
                var enginePower = Bonuses.VelocityMultiplier.Value * EquipmentStats.EnginePower;
                return Mathf.Clamp(enginePower * 50f / (Mathf.Sqrt(Weight)*CellCount), 0, ShipSettings.MaxVelocity);
            }
        }

        public float TurnRate
        {
            get
            {
                var turnRate = Bonuses.VelocityMultiplier.Value * EquipmentStats.TurnRate;
                return Mathf.Clamp(turnRate * 50f / (Mathf.Sqrt(Weight) * CellCount), 0, ShipSettings.MaxTurnRate);
            }
        }

        public bool Autopilot => EquipmentStats.Autopilot;
        public bool TargetingSystem => BaseStats.AutoTargeting;

        public StatMultiplier WeaponDamageMultiplier => EquipmentStats.WeaponDamageMultiplier * Bonuses.DamageMultiplier.Value;
        public StatMultiplier WeaponFireRateMultiplier => EquipmentStats.WeaponFireRateMultiplier;
        public StatMultiplier WeaponEnergyCostMultiplier => EquipmentStats.WeaponEnergyCostMultiplier;
        public StatMultiplier WeaponRangeMultiplier => EquipmentStats.WeaponRangeMultiplier;

        public StatMultiplier DroneDamageMultiplier => EquipmentStats.DroneDamageMultiplier;
        public StatMultiplier DroneDefenseMultiplier => EquipmentStats.DroneDefenseMultiplier;
        public StatMultiplier DroneSpeedMultiplier => EquipmentStats.DroneSpeedMultiplier;
        public StatMultiplier DroneRangeMultiplier => EquipmentStats.DroneRangeMultiplier;

        public float RammingDamage => EquipmentStats.RammingDamage;
        public float EnergyAbsorption => EquipmentStats.EnergyAbsorption * Bonuses.ArmorPointsMultiplier.Value;

        public float RammingDamageMultiplier
        {
            get
            {
                var armorPoints = ArmorPoints;
                var rammingDamage = EquipmentStats.RammingDamage;
                return EquipmentStats.RammingDamageMultiplier.Value * Bonuses.RammingDamageMultiplier.Value * (1.0f + rammingDamage / (rammingDamage + armorPoints));
            }
        }

        public float DroneBuildTime
        {
            get
            {
                var speed = ShipSettings.BaseDroneReconstructionSpeed + EquipmentStats.DroneReconstructionSpeed;
                return speed > 0 ? EquipmentStats.DroneReconstructionTimeMultiplier.Value / speed : 0f;
            }
        }

        public float DroneBuildSpeed
        {
            get
            {
                var speed = ShipSettings.BaseDroneReconstructionSpeed + EquipmentStats.DroneReconstructionSpeed;
                return speed / EquipmentStats.DroneReconstructionTimeMultiplier.Value;
            }
        }

        public float ModelScale => _ship.ModelScale;
        public SpriteId ModelImage => _ship.ModelImage;
        public SpriteId IconImage => _ship.IconImage;
        public Color EngineColor => _ship.EngineColor;
        public ShipCategory ShipCategory => _ship.ShipCategory;
        public IEnumerable<Engine> Engines => _ship.Engines;

        private readonly Ship _ship;
    }
}
