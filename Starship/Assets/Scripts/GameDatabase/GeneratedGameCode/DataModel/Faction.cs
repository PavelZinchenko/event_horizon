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
	public partial class Faction
	{
		partial void OnDataDeserialized(FactionSerializable serializable, Database.Loader loader);

		public static Faction Create(FactionSerializable serializable, Database.Loader loader)
		{
			return new Faction(serializable, loader);
		}

		private Faction(FactionSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<Faction>(serializable.Id);
			loader.AddFaction(serializable.Id, this);

			Name = serializable.Name;
			Color = new ColorData(serializable.Color);
			HomeStarDistance = UnityEngine.Mathf.Clamp(serializable.HomeStarDistance, 0, 1000);
			WanderingShipsDistance = UnityEngine.Mathf.Clamp(serializable.WanderingShipsDistance, 0, 1000);
			Hidden = serializable.Hidden;
			Hostile = serializable.Hostile;

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<Faction> Id;

		public string Name { get; private set; }
		public ColorData Color { get; private set; }
		public int HomeStarDistance { get; private set; }
		public int WanderingShipsDistance { get; private set; }
		public bool Hidden { get; private set; }
		public bool Hostile { get; private set; }

		public static Faction DefaultValue { get; private set; }
	}
}
