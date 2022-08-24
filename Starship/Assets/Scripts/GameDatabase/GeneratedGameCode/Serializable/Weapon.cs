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
	public class WeaponSerializable : SerializableItem
	{
		public WeaponClass WeaponClass;
		public float FireRate;
		public float Spread;
		public int Magazine;
		public ActivationType ActivationType;
		public string ShotSound;
		public string ChargeSound;
		public string ShotEffectPrefab;
		public string ControlButtonIcon;
	}
}
