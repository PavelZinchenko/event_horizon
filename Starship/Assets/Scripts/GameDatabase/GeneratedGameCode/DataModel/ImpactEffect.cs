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
	public partial class ImpactEffect
	{
		partial void OnDataDeserialized(ImpactEffectSerializable serializable, Database.Loader loader);

		public static ImpactEffect Create(ImpactEffectSerializable serializable, Database.Loader loader)
		{
			return new ImpactEffect(serializable, loader);
		}

		private ImpactEffect(ImpactEffectSerializable serializable, Database.Loader loader)
		{
			Type = serializable.Type;
			DamageType = serializable.DamageType;
			Power = UnityEngine.Mathf.Clamp(serializable.Power, 0f, 1E+09f);
			Factor = UnityEngine.Mathf.Clamp(serializable.Factor, 0f, 1f);

			OnDataDeserialized(serializable, loader);
		}

		public ImpactEffectType Type { get; private set; }
		public DamageType DamageType { get; private set; }
		public float Power { get; private set; }
		public float Factor { get; private set; }

		public static ImpactEffect DefaultValue { get; private set; }
	}
}
