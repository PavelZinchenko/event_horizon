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
	public partial class SatelliteBuild
	{
		partial void OnDataDeserialized(SatelliteBuildSerializable serializable, Database.Loader loader);

		public static SatelliteBuild Create(SatelliteBuildSerializable serializable, Database.Loader loader)
		{
			return new SatelliteBuild(serializable, loader);
		}

		private SatelliteBuild(SatelliteBuildSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<SatelliteBuild>(serializable.Id);
			loader.AddSatelliteBuild(serializable.Id, this);

			Satellite = loader.GetSatellite(new ItemId<Satellite>(serializable.SatelliteId));
			if (Satellite == null)
			    throw new DatabaseException(this.GetType().Name + ".Satellite cannot be null - " + serializable.SatelliteId);
			NotAvailableInGame = serializable.NotAvailableInGame;
			DifficultyClass = serializable.DifficultyClass;
			Components = new ImmutableCollection<InstalledComponent>(serializable.Components?.Select(item => InstalledComponent.Create(item, loader)));

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<SatelliteBuild> Id;

		public Satellite Satellite { get; private set; }
		public bool NotAvailableInGame { get; private set; }
		public DifficultyClass DifficultyClass { get; private set; }
		public ImmutableCollection<InstalledComponent> Components { get; private set; }

		public static SatelliteBuild DefaultValue { get; private set; }
	}
}
