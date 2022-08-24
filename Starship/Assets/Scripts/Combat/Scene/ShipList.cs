using System.Collections.Generic;
using Combat.Component.Ship;

namespace Combat.Scene
{
    public class ShipList : IUnitList<IShip>
    {
        public ShipList()
        {
            _lockObject = new object();
            _ships = new List<IShip>();
            _shipsReadOnly = _ships.AsReadOnly();
        }

        public IList<IShip> Items { get { return _shipsReadOnly; } }
        public object LockObject { get { return _lockObject; } }

        public void Add(IShip ship)
        {
            lock (_lockObject)
            {
                _ships.Add(ship);
            }
        }

        public void Remove(IShip ship)
        {
            lock (_lockObject)
            {
                _ships.Remove(ship);
            }
        }

        public void Clear()
        {
            lock (LockObject)
            {
                _ships.Clear();
            }
        }

        private readonly object _lockObject;
        private readonly List<IShip> _ships;
        private readonly IList<IShip> _shipsReadOnly;
    }
}
