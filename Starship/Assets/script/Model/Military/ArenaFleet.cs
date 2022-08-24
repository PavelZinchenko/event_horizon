using System.Collections.Generic;
using System.Linq;
using Constructor.Ships;

namespace Model
{
    namespace Military
    {
        public class ArenaFleet : IFleet
        {
            public ArenaFleet(IEnumerable<IShip> ships, float powerMultiplier = 1.0f)
            {
                _ships = new List<IShip>(ships.Select<IShip,IShip>(item => new ArenaShip(item, powerMultiplier)));
            }

            public IEnumerable<IShip> Ships { get { return _ships; } }

            public int AiLevel { get { return 100; } }

            public float Power { get { return Maths.Threat.GetShipsPower(Ships); } }

            private readonly List<IShip> _ships;
        }
    }
}
