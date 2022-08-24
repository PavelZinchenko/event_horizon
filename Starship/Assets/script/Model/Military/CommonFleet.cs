using System.Collections.Generic;
using System.Linq;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;

namespace Model
{
	namespace Military
	{
		public class CommonFleet : IFleet
		{
			public CommonFleet(IDatabase database, IEnumerable<ShipBuild> ships, int distance, int seed, int aiLevel = -1)
			{
				_builds = ships;
				_seed = seed;
				_distance = distance;
			    _database = database;
				AiLevel = aiLevel < 0 ? Maths.Distance.AiLevel(distance) : aiLevel;
			}

			public IEnumerable<IShip> Ships
			{
				get
				{
					if (_ships == null)
						_ships = _builds.Create(_distance, new System.Random(_seed), _database).ToList();

					return _ships;
				}
			}

			public int AiLevel { get; private set; }

			public float Power
			{
				get
				{
					if (_power < 0)
						_power = Maths.Threat.GetShipsPower(Ships);
					return _power;
				}
			}

			private float _power = -1;
			private List<IShip> _ships;
			private readonly IEnumerable<ShipBuild> _builds;
			private readonly int _distance;
			private readonly int _seed;
		    private readonly IDatabase _database;
		}
	}
}
