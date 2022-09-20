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
	public class AmmunitionSerializable : SerializableItem
	{
		public BulletBodySerializable Body;
		public BulletTriggerSerializable[] Triggers;
		public BulletImpactType ImpactType;
		public ImpactEffectSerializable[] Effects;
	}
}
