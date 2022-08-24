using System.Collections.Generic;
using Constructor.Ships;
using Database.Legacy;
using GameDatabase;

namespace Model
{
	namespace Military
	{
		public class PlayerFleet : IFleet
		{
		    public PlayerFleet(IDatabase database, GameServices.Player.PlayerFleet fleet)
		    {
		        _database = database;
		        _fleet = fleet;
		    }

            public IEnumerable<IShip> Ships 
			{
				get
				{
					int count = 0;
					foreach (var ship in _fleet.ActiveShipGroup.Ships)
					{
						count++;
						yield return ship;
					}

					if (count == 0)
						yield return new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("mothership_1")));
				}
			}

			public int AiLevel { get { return 100; } }			
			public float Power { get { return _fleet.Power; } }

		    private readonly IDatabase _database;
		    private readonly GameServices.Player.PlayerFleet _fleet;
		}
	}
}
