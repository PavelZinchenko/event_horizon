//-------------------------------------------------------------------------------
//                                                                               
//    This code was automatically generated.                                     
//    Changes to this file may cause incorrect behavior and will be lost if      
//    the code is regenerated.                                                   
//                                                                               
//-------------------------------------------------------------------------------

using System;
using GameDatabase.Enums;
using GameDatabase.Model;

namespace GameDatabase.Serializable
{
	[Serializable]
	public class ComponentStatsSerializable : SerializableItem
	{
		public ComponentStatsType Type;
		public float ArmorPoints;
		public float ArmorRepairRate;
		public float ArmorRepairCooldownModifier;
		public float EnergyPoints;
		public float EnergyRechargeRate;
		public float EnergyRechargeCooldownModifier;
		public float ShieldPoints;
		public float ShieldRechargeRate;
		public float ShieldRechargeCooldownModifier;
		public float Weight;
		public float RammingDamage;
		public float EnergyAbsorption;
		public float KineticResistance;
		public float EnergyResistance;
		public float ThermalResistance;
		public float EnginePower;
		public float TurnRate;
		public bool Autopilot;
		public float DroneRangeModifier;
		public float DroneDamageModifier;
		public float DroneDefenseModifier;
		public float DroneSpeedModifier;
		public float DronesBuiltPerSecond;
		public float DroneBuildTimeModifier;
		public float WeaponFireRateModifier;
		public float WeaponDamageModifier;
		public float WeaponRangeModifier;
		public float WeaponEnergyCostModifier;
		public PlatformType AlterWeaponPlatform;
	}
}
