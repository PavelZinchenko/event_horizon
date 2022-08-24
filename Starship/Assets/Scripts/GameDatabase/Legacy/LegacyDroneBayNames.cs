using System.Collections.Generic;

using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Model;

namespace Database.Legacy
{
    public static class LegacyDroneBayNames
    {
        public static ItemId<DroneBay> GetId(string value) { int id; return new ItemId<DroneBay>(_items.TryGetValue(value, out id) ? id : -1); }

        public static string GetName(ItemId<DroneBay> id) { return _items.FirstOrDefault(item => item.Value == id.Value).Key; } // TODO: delete

        private static readonly Dictionary<string, int> _items = new Dictionary<string, int>()
        {
            { "DroneBay1", 1 },
            { "DroneBay2", 2 },
            { "DroneBay3", 3 },
            { "DroneBay4", 4 },
            { "DroneBay5", 5 },
        };
    }
}
