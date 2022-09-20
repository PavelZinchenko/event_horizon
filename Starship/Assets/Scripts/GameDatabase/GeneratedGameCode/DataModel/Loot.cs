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
	public partial class LootModel
	{
		partial void OnDataDeserialized(LootSerializable serializable, Database.Loader loader);

		public static LootModel Create(LootSerializable serializable, Database.Loader loader)
		{
			return new LootModel(serializable, loader);
		}

		private LootModel(LootSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<LootModel>(serializable.Id);
			loader.AddLoot(serializable.Id, this);

			Loot = LootContent.Create(serializable.Loot, loader);

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<LootModel> Id;

		public LootContent Loot { get; private set; }

		public static LootModel DefaultValue { get; private set; }
	}
}
