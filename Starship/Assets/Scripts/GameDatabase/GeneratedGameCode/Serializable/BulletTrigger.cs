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
	public struct BulletTriggerSerializable
	{
		public BulletTriggerCondition Condition;
		public BulletEffectType EffectType;
		public int VisualEffect;
		public string AudioClip;
		public int Ammunition;
		public string Color;
		public ColorMode ColorMode;
		public int Quantity;
		public float Size;
		public float Lifetime;
		public float Cooldown;
		public float RandomFactor;
		public float PowerMultiplier;
		public int MaxNestingLevel;
	}
}
