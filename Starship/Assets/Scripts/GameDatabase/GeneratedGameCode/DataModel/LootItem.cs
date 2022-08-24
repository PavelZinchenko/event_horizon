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
	public partial class LootItem
	{
		partial void OnDataDeserialized(LootItemSerializable serializable, Database.Loader loader);

		public static LootItem Create(LootItemSerializable serializable, Database.Loader loader)
		{
			return new LootItem(serializable, loader);
		}

		private LootItem(LootItemSerializable serializable, Database.Loader loader)
		{
			Weight = UnityEngine.Mathf.Clamp(serializable.Weight, -3.402823E+38f, 3.402823E+38f);
			Loot = LootContent.Create(serializable.Loot, loader);

			OnDataDeserialized(serializable, loader);
		}

		public float Weight { get; private set; }
		public LootContent Loot { get; private set; }

		public static LootItem DefaultValue { get; private set; }
	}
}
