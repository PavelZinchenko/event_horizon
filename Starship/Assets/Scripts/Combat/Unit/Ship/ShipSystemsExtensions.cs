using System.Collections.Generic;
using System.Linq;
using Combat.Component.Systems;
using Combat.Component.Systems.Devices;
using Combat.Component.Systems.DroneBays;
using Combat.Component.Systems.Weapons;

namespace Combat.Component.Ship
{
    public static class ShipSystemsExtensions
    {
        public static List<List<int>> GetKeyBindings(this IList<ISystem> shipSystems)
        {
            var keys = new Dictionary<int, List<int>>();
            for (var i = 0; i < shipSystems.Count; ++i)
            {
                var system = shipSystems[i];
                if (system.KeyBinding < 0)
                    continue;

                List<int> list;
                if (!keys.TryGetValue(system.KeyBinding, out list))
                {
                    list = new List<int>();
                    keys.Add(system.KeyBinding, list);
                }
                list.Add(i);
            }

            return keys.OrderBy(item => item.Key).Select(item => item.Value).ToList();
        }

        public static IEnumerable<int> GetDroneBayIndices(this IList<ISystem> shipSystems)
        {
            for (var i = 0; i < shipSystems.Count; ++i)
            {
                var system = shipSystems.DroneBay(i);
                if (system == null || system.KeyBinding < 0)
                    continue;
                yield return i;
            }

            yield break;
        }

        public static IWeapon Weapon(this IList<ISystem> shipSystems, int id)
        {
            return shipSystems[id] as IWeapon;
        }

        public static IDroneBay DroneBay(this IList<ISystem> shipSystems, int id)
        {
            return shipSystems[id] as IDroneBay;
        }

        public static IDevice Device(this IList<ISystem> shipSystems, int id)
        {
            return shipSystems[id] as IDevice;
        }
    }
}
