using System.Collections.Generic;
using GameDatabase.DataModel;
using GameDatabase.Model;

namespace Database.Legacy
{
    public static class LegacySatelliteBuildNames
    {
        public static ItemId<Satellite> GetId(string value) { int id; return new ItemId<Satellite>(_items.TryGetValue(value, out id) ? id : -1); }

        private static readonly Dictionary<string, int> _items = new Dictionary<string, int>()
        {
            { "1l_1", 1 },
            { "1m_1", 2 },
            { "1s_1", 3 },
            { "2l_1", 4 },
            { "2l_2", 5 },
            { "2l_3", 6 },
            { "2m_1", 7 },
            { "2s_1", 8 },
            { "3l_1", 9 },
            { "3l_2", 10 },
            { "3m_1", 11 },
            { "3s_1", 12 },
            { "4l_1", 13 },
            { "4l_2", 14 },
            { "4m_1", 15 },
            { "4s_1", 16 },
            { "5l_1", 17 },
            { "5l_2", 18 },
            { "5m_1", 19 },
            { "5s_1", 20 },
            { "6l_1", 21 },
            { "6l_2", 22 },
            { "6m_1", 23 },
            { "6m_2", 24 },
            { "6s_1", 25 },
            { "7l_1", 26 },
            { "7m_1", 27 },
            { "7s_1", 28 },
        };
    }
}
