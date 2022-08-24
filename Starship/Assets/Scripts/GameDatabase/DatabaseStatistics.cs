using System.Collections.Generic;
using GameDatabase.DataModel;
using UnityEngine;

namespace GameDatabase
{
    public class DatabaseStatistics
    {
        public DatabaseStatistics(GameDatabaseLoadedSignal dataLoadedSignal, IDatabase database)
        {
            _database = database;
            _dataLoadedSignal = dataLoadedSignal;
            _dataLoadedSignal.Event += OnDatabaseLoaded;

            UpdateStatistics();
        }

        public static int SmallestShipSize(Faction faction)
        {
            int size;
            return _smallestFactionShips.TryGetValue(faction, out size) ? size : 0;
        }

        private void OnDatabaseLoaded()
        {
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            _smallestFactionShips.Clear();
            foreach (var ship in _database.ShipList)
            {
                var faction = ship.Faction;
                var shipSize = ship.Layout.CellCount;

                int size;
                if (_smallestFactionShips.TryGetValue(faction, out size) && size < shipSize)
                    continue;

                _smallestFactionShips[faction] = shipSize;
            }
        }

        private readonly GameDatabaseLoadedSignal _dataLoadedSignal;
        private readonly IDatabase _database;

        private static readonly Dictionary<Faction, int> _smallestFactionShips = new Dictionary<Faction, int>();
    }
}
