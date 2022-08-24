using System.Collections.Generic;
using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Enums;

namespace GameDatabase.Extensions
{
    public static class ComponentExtensions
    {
        public static IEnumerable<Component> Common(this IEnumerable<Component> components)
        {
            return components.Where(item => item.Availability == Availability.Common);
        }

        public static IEnumerable<Component> CommonAndRare(this IEnumerable<Component> components)
        {
            return components.Where(item => item.Availability == Availability.Common || item.Availability == Availability.Rare);
        }

        public static IEnumerable<Component> OfFaction(this IEnumerable<Component> components, Faction faction)
        {
            if (faction == Faction.Neutral || faction == Faction.Undefined)
                return components.Where(item => !item.Faction.Hidden);

            return components.Where(item => item.Faction == faction || item.Faction == Faction.Neutral || item.Faction == Faction.Undefined);
        }

        public static IEnumerable<Component> Available(this IEnumerable<Component> components)
        {
            return components.CommonAndRare();
        }
        public static IEnumerable<Component> LevelLessOrEqual(this IEnumerable<Component> components, int level)
        {
            if (level < 0) level = 0;
            return components.Where(item => item.Level <= level);
        }
    }
}
