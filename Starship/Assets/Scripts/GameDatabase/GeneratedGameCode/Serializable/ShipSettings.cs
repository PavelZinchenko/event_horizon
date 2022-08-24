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
	public class ShipSettingsSerializable : SerializableItem
	{
		public float DefaultWeightPerCell;
		public float MinimumWeightPerCell;
		public float BaseArmorPoints;
		public float ArmorPointsPerCell;
		public float ArmorRepairCooldown;
		public float BaseEnergyPoints;
		public float BaseEnergyRechargeRate;
		public float EnergyRechargeCooldown;
		public float BaseShieldRechargeRate;
		public float ShieldRechargeCooldown;
		public float BaseDroneReconstructionSpeed;
		public float MaxVelocity;
		public float MaxTurnRate;
	}
}
