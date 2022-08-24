using System.Collections.Generic;
using System.Linq;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;

namespace Model
{
	namespace Military
	{
		public class SurvivalFleet : IFleet
		{
			public SurvivalFleet(IDatabase database, IEnumerable<ShipBuild> ships, int level, int seed)
			{
				_builds = ships;
				_seed = seed;
				_level = level;
			    _database = database;
				AiLevel = Maths.Distance.AiLevel(level);
			}
			
			public IEnumerable<IShip> Ships
			{
				get
				{
					if (_ships == null)
					{
						var level = _level;
						_ships = _builds.Select(item => Factories.Ship.Create(item, level++, new System.Random(_seed), _database)).ToList();
					}

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
			private readonly int _level;
			private readonly int _seed;
		    private readonly IDatabase _database;
		}
	}
}
