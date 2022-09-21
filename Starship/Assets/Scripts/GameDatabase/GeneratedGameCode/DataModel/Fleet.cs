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
	public partial class Fleet
	{
		partial void OnDataDeserialized(FleetSerializable serializable, Database.Loader loader);

		public static Fleet Create(FleetSerializable serializable, Database.Loader loader)
		{
			return new Fleet(serializable, loader);
		}

		private Fleet(FleetSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<Fleet>(serializable.Id);
			loader.AddFleet(serializable.Id, this);

			Factions = RequiredFactions.Create(serializable.Factions, loader);
			LevelBonus = UnityEngine.Mathf.Clamp(serializable.LevelBonus, -10000, 10000);
			NoRandomShips = serializable.NoRandomShips;
			CombatTimeLimit = UnityEngine.Mathf.Clamp(serializable.CombatTimeLimit, 0, 999);
			LootCondition = serializable.LootCondition;
			ExpCondition = serializable.ExpCondition;
			SpecificShips = new ImmutableCollection<ShipBuild>(serializable.SpecificShips?.Select(item => loader.GetShipBuild(new ItemId<ShipBuild>(item), true)));

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<Fleet> Id;

		public RequiredFactions Factions { get; private set; }
		public int LevelBonus { get; private set; }
		public bool NoRandomShips { get; private set; }
		public int CombatTimeLimit { get; private set; }
		public RewardCondition LootCondition { get; private set; }
		public RewardCondition ExpCondition { get; private set; }
		public ImmutableCollection<ShipBuild> SpecificShips { get; private set; }

		public static Fleet DefaultValue { get; private set; }
	}
}
