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
	public partial class ExplorationSettings
	{
		partial void OnDataDeserialized(ExplorationSettingsSerializable serializable, Database.Loader loader);

		public static ExplorationSettings Create(ExplorationSettingsSerializable serializable, Database.Loader loader)
		{
			return new ExplorationSettings(serializable, loader);
		}

		private ExplorationSettings(ExplorationSettingsSerializable serializable, Database.Loader loader)
		{
			OutpostShip = loader.GetShip(new ItemId<Ship>(serializable.OutpostShip));
			TurretShip = loader.GetShip(new ItemId<Ship>(serializable.TurretShip));
			InfectedPlanetFaction = loader.GetFaction(new ItemId<Faction>(serializable.InfectedPlanetFaction));
			HiveShipBuild = loader.GetShipBuild(new ItemId<ShipBuild>(serializable.HiveShipBuild));

			OnDataDeserialized(serializable, loader);
		}

		public Ship OutpostShip { get; private set; }
		public Ship TurretShip { get; private set; }
		public Faction InfectedPlanetFaction { get; private set; }
		public ShipBuild HiveShipBuild { get; private set; }

		public static ExplorationSettings DefaultValue { get; private set; }
	}
}
