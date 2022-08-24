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
	public class DroneBaySerializable : SerializableItem
	{
		public float EnergyConsumption;
		public float PassiveEnergyConsumption;
		public float Range;
		public float DamageMultiplier;
		public float DefenseMultiplier;
		public float SpeedMultiplier;
		public bool ImprovedAi;
		public int Capacity;
		public ActivationType ActivationType;
		public string LaunchSound;
		public string LaunchEffectPrefab;
		public string ControlButtonIcon;
	}
}
