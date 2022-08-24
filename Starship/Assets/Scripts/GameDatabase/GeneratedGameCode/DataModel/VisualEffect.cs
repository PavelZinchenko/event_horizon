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
	public partial class VisualEffect
	{
		partial void OnDataDeserialized(VisualEffectSerializable serializable, Database.Loader loader);

		public static VisualEffect Create(VisualEffectSerializable serializable, Database.Loader loader)
		{
			return new VisualEffect(serializable, loader);
		}

		private VisualEffect(VisualEffectSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<VisualEffect>(serializable.Id);
			loader.AddVisualEffect(serializable.Id, this);

			Elements = new ImmutableCollection<VisualEffectElement>(serializable.Elements?.Select(item => VisualEffectElement.Create(item, loader)));

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<VisualEffect> Id;

		public ImmutableCollection<VisualEffectElement> Elements { get; private set; }

		public static VisualEffect DefaultValue { get; private set; }
	}
}
