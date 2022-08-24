using System.Collections.Generic;
using System.Linq;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;

namespace Model
{
    namespace Military
    {
        public class TestFleet : IFleet
        {
            public TestFleet(IDatabase database, IEnumerable<ShipBuild> ships, int aiLevel)
            {
                _builds = ships;
                _database = database;
                AiLevel = aiLevel;
            }

            public IEnumerable<IShip> Ships
            {
                get
                {
                    if (_ships == null)
                        _ships = _builds.Create(0, new System.Random(0), _database).ToList();

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
            private readonly IDatabase _database;
        }
    }
}
