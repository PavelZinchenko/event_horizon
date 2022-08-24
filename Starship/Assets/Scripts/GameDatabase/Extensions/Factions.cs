using System.Collections.Generic;
using System.Linq;
using GameDatabase.DataModel;

namespace GameDatabase.Extensions
{
    public static class FactionExtensions
    {
        public static IEnumerable<Faction> Visible(this IEnumerable<Faction> factions)
        {
            return factions.Where(item => !item.Hidden);
        }

        public static IEnumerable<Faction> AtDistance(this IEnumerable<Faction> factions, int distance)
        {
            return factions.Where(item => distance < 0 || item.HomeStarDistance <= distance);
        }
    }
}
