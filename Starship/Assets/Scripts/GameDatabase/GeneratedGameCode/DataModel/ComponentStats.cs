//-------------------------------------------------------------------------------
//                                                                               
//    This code was automatically generated.                                     
//    Changes to this file may cause incorrect behavior and will be lost if      
//    the code is regenerated.                                                   
//                                                                               
//-------------------------------------------------------------------------------

using System.Linq;
using GameDatabase.Enums;
using GameDatabase.Serializable;
using GameDatabase.Model;

namespace GameDatabase.DataModel
{
	public partial class ComponentStats
	{
		partial void OnDataDeserialized(ComponentStatsSerializable serializable, Database.Loader loader);

		public static ComponentStats Create(ComponentStatsSerializable serializable, Database.Loader loader)
		{
			return new ComponentStats(serializable, loader);
		}

		private ComponentStats(ComponentStatsSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<ComponentStats>(serializable.Id);
			loader.AddComponentStats(serializable.Id, this);

			Type = serializable.Type;
			ArmorPoints = UnityEngine.Mathf.Clamp(serializable.ArmorPoints, -3.402823E+38f, 3.402823E+38f);
			ArmorRepairRate = UnityEngine.Mathf.Clamp(serializable.ArmorRepairRate, -3.402823E+38f, 3.402823E+38f);
			ArmorRepairCooldownModifier = UnityEngine.Mathf.Clamp(serializable.ArmorRepairCooldownModifier, -3.402823E+38f, 3.402823E+38f);
			EnergyPoints = UnityEngine.Mathf.Clamp(serializable.EnergyPoints, -3.402823E+38f, 3.402823E+38f);
			EnergyRechargeRate = UnityEngine.Mathf.Clamp(serializable.EnergyRechargeRate, -3.402823E+38f, 3.402823E+38f);
			EnergyRechargeCooldownModifier = UnityEngine.Mathf.Clamp(serializable.EnergyRechargeCooldownModifier, -3.402823E+38f, 3.402823E+38f);
			ShieldPoints = UnityEngine.Mathf.Clamp(serializable.ShieldPoints, -3.402823E+38f, 3.402823E+38f);
			ShieldRechargeRate = UnityEngine.Mathf.Clamp(serializable.ShieldRechargeRate, -3.402823E+38f, 3.402823E+38f);
			ShieldRechargeCooldownModifier = UnityEngine.Mathf.Clamp(serializable.ShieldRechargeCooldownModifier, -3.402823E+38f, 3.402823E+38f);
			Weight = UnityEngine.Mathf.Clamp(serializable.Weight, -3.402823E+38f, 3.402823E+38f);
			RammingDamage = UnityEngine.Mathf.Clamp(serializable.RammingDamage, -3.402823E+38f, 3.402823E+38f);
			EnergyAbsorption = UnityEngine.Mathf.Clamp(serializable.EnergyAbsorption, -3.402823E+38f, 3.402823E+38f);
			KineticResistance = UnityEngine.Mathf.Clamp(serializable.KineticResistance, -3.402823E+38f, 3.402823E+38f);
			EnergyResistance = UnityEngine.Mathf.Clamp(serializable.EnergyResistance, -3.402823E+38f, 3.402823E+38f);
			ThermalResistance = UnityEngine.Mathf.Clamp(serializable.ThermalResistance, -3.402823E+38f, 3.402823E+38f);
			EnginePower = UnityEngine.Mathf.Clamp(serializable.EnginePower, -3.402823E+38f, 3.402823E+38f);
			TurnRate = UnityEngine.Mathf.Clamp(serializable.TurnRate, -3.402823E+38f, 3.402823E+38f);
			Autopilot = serializable.Autopilot;
			DroneRangeModifier = UnityEngine.Mathf.Clamp(serializable.DroneRangeModifier, -3.402823E+38f, 3.402823E+38f);
			DroneDamageModifier = UnityEngine.Mathf.Clamp(serializable.DroneDamageModifier, -3.402823E+38f, 3.402823E+38f);
			DroneDefenseModifier = UnityEngine.Mathf.Clamp(serializable.DroneDefenseModifier, -3.402823E+38f, 3.402823E+38f);
			DroneSpeedModifier = UnityEngine.Mathf.Clamp(serializable.DroneSpeedModifier, -3.402823E+38f, 3.402823E+38f);
			DronesBuiltPerSecond = UnityEngine.Mathf.Clamp(serializable.DronesBuiltPerSecond, -3.402823E+38f, 3.402823E+38f);
			DroneBuildTimeModifier = UnityEngine.Mathf.Clamp(serializable.DroneBuildTimeModifier, -3.402823E+38f, 3.402823E+38f);
			WeaponFireRateModifier = UnityEngine.Mathf.Clamp(serializable.WeaponFireRateModifier, -3.402823E+38f, 3.402823E+38f);
			WeaponDamageModifier = UnityEngine.Mathf.Clamp(serializable.WeaponDamageModifier, -3.402823E+38f, 3.402823E+38f);
			WeaponRangeModifier = UnityEngine.Mathf.Clamp(serializable.WeaponRangeModifier, -3.402823E+38f, 3.402823E+38f);
			WeaponEnergyCostModifier = UnityEngine.Mathf.Clamp(serializable.WeaponEnergyCostModifier, -3.402823E+38f, 3.402823E+38f);
			AlterWeaponPlatform = serializable.AlterWeaponPlatform;

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<ComponentStats> Id;

		public ComponentStatsType Type { get; private set; }
		public float ArmorPoints { get; private set; }
		public float ArmorRepairRate { get; private set; }
		public float ArmorRepairCooldownModifier { get; private set; }
		public float EnergyPoints { get; private set; }
		public float EnergyRechargeRate { get; private set; }
		public float EnergyRechargeCooldownModifier { get; private set; }
		public float ShieldPoints { get; private set; }
		public float ShieldRechargeRate { get; private set; }
		public float ShieldRechargeCooldownModifier { get; private set; }
		public float Weight { get; private set; }
		public float RammingDamage { get; private set; }
		public float EnergyAbsorption { get; private set; }
		public float KineticResistance { get; private set; }
		public float EnergyResistance { get; private set; }
		public float ThermalResistance { get; private set; }
		public float EnginePower { get; private set; }
		public float TurnRate { get; private set; }
		public bool Autopilot { get; private set; }
		public float DroneRangeModifier { get; private set; }
		public float DroneDamageModifier { get; private set; }
		public float DroneDefenseModifier { get; private set; }
		public float DroneSpeedModifier { get; private set; }
		public float DronesBuiltPerSecond { get; private set; }
		public float DroneBuildTimeModifier { get; private set; }
		public float WeaponFireRateModifier { get; private set; }
		public float WeaponDamageModifier { get; private set; }
		public float WeaponRangeModifier { get; private set; }
		public float WeaponEnergyCostModifier { get; private set; }
		public PlatformType AlterWeaponPlatform { get; private set; }

		public static ComponentStats DefaultValue { get; private set; }
	}
}
