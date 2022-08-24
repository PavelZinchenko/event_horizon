using System.Collections.Generic;

using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Model;

namespace Database.Legacy
{
    public static class LegacySatelliteNames
    {
        public static ItemId<Satellite> GetId(string value) { int id; return new ItemId<Satellite>(_items.TryGetValue(value, out id) ? id : -1); }

        public static string GetName(ItemId<Satellite> id) { return _items.FirstOrDefault(item => item.Value == id.Value).Key; } // TODO: delete

        private static readonly Dictionary<string, int> _items = new Dictionary<string, int>()
        {
            { "1l", 1 },
            { "1m", 2 },
            { "1s", 3 },
            { "2l", 4 },
            { "2m", 5 },
            { "2s", 6 },
            { "3l", 7 },
            { "3m", 8 },
            { "3s", 9 },
            { "4l", 10 },
            { "4m", 11 },
            { "4s", 12 },
            { "5l", 13 },
            { "5m", 14 },
            { "5s", 15 },
            { "6l", 16 },
            { "6m", 17 },
            { "6s", 18 },
            { "7l", 19 },
            { "7m", 20 },
            { "7s", 21 },
        };
    }
}
