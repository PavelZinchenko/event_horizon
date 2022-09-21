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
	public partial class ShipSettings
	{
		partial void OnDataDeserialized(ShipSettingsSerializable serializable, Database.Loader loader);

		public static ShipSettings Create(ShipSettingsSerializable serializable, Database.Loader loader)
		{
			return new ShipSettings(serializable, loader);
		}

		private ShipSettings(ShipSettingsSerializable serializable, Database.Loader loader)
		{
			DefaultWeightPerCell = UnityEngine.Mathf.Clamp(serializable.DefaultWeightPerCell, 0.01f, 3.402823E+38f);
			MinimumWeightPerCell = UnityEngine.Mathf.Clamp(serializable.MinimumWeightPerCell, 0.01f, 3.402823E+38f);
			BaseArmorPoints = UnityEngine.Mathf.Clamp(serializable.BaseArmorPoints, -3.402823E+38f, 3.402823E+38f);
			ArmorPointsPerCell = UnityEngine.Mathf.Clamp(serializable.ArmorPointsPerCell, -3.402823E+38f, 3.402823E+38f);
			ArmorRepairCooldown = UnityEngine.Mathf.Clamp(serializable.ArmorRepairCooldown, 0f, 3.402823E+38f);
			BaseEnergyPoints = UnityEngine.Mathf.Clamp(serializable.BaseEnergyPoints, -3.402823E+38f, 3.402823E+38f);
			BaseEnergyRechargeRate = UnityEngine.Mathf.Clamp(serializable.BaseEnergyRechargeRate, -3.402823E+38f, 3.402823E+38f);
			EnergyRechargeCooldown = UnityEngine.Mathf.Clamp(serializable.EnergyRechargeCooldown, 0f, 3.402823E+38f);
			BaseShieldRechargeRate = UnityEngine.Mathf.Clamp(serializable.BaseShieldRechargeRate, -3.402823E+38f, 3.402823E+38f);
			ShieldRechargeCooldown = UnityEngine.Mathf.Clamp(serializable.ShieldRechargeCooldown, 0f, 3.402823E+38f);
			BaseDroneReconstructionSpeed = UnityEngine.Mathf.Clamp(serializable.BaseDroneReconstructionSpeed, 0f, 3.402823E+38f);
			MaxVelocity = UnityEngine.Mathf.Clamp(serializable.MaxVelocity, 5f, 50f);
			MaxTurnRate = UnityEngine.Mathf.Clamp(serializable.MaxTurnRate, 5f, 50f);

			OnDataDeserialized(serializable, loader);
		}

		public float DefaultWeightPerCell { get; private set; }
		public float MinimumWeightPerCell { get; private set; }
		public float BaseArmorPoints { get; private set; }
		public float ArmorPointsPerCell { get; private set; }
		public float ArmorRepairCooldown { get; private set; }
		public float BaseEnergyPoints { get; private set; }
		public float BaseEnergyRechargeRate { get; private set; }
		public float EnergyRechargeCooldown { get; private set; }
		public float BaseShieldRechargeRate { get; private set; }
		public float ShieldRechargeCooldown { get; private set; }
		public float BaseDroneReconstructionSpeed { get; private set; }
		public float MaxVelocity { get; private set; }
		public float MaxTurnRate { get; private set; }

		public static ShipSettings DefaultValue { get; private set; }
	}
}
