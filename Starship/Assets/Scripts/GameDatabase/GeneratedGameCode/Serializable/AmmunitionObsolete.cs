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
	public class AmmunitionObsoleteSerializable : SerializableItem
	{
		public AmmunitionClassObsolete AmmunitionClass;
		public DamageType DamageType;
		public float Impulse;
		public float Recoil;
		public float Size;
		public UnityEngine.Vector2 InitialPosition;
		public float AreaOfEffect;
		public float Damage;
		public float Range;
		public float Velocity;
		public float LifeTime;
		public int HitPoints;
		public bool IgnoresShipVelocity;
		public float EnergyCost;
		public int CoupledAmmunitionId;
		public string Color;
		public string FireSound;
		public string HitSound;
		public string HitEffectPrefab;
		public string BulletPrefab;
	}
}
