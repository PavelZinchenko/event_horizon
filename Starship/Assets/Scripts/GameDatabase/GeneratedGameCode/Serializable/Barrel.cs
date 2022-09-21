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
	public struct BarrelSerializable
	{
		public UnityEngine.Vector2 Position;
		public float Rotation;
		public float Offset;
		public PlatformType PlatformType;
		public float AutoAimingArc;
		public float RotationSpeed;
		public string WeaponClass;
		public string Image;
		public float Size;
	}
}
