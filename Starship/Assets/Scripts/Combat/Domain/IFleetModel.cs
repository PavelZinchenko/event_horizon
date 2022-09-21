using System.Collections.Generic;

namespace Combat.Domain
{
    public interface IFleetModel
    {
        IList<IShipInfo> Ships { get; }
        int Level { get; }
    }

    public static class FleetModelExtensions
    {
        public static bool TryGetInfo(this IFleetModel fleet, Combat.Component.Ship.IShip unit, out IShipInfo info)
        {
            info = fleet.GetInfo(unit);
            return info != null;
        }

        public static int CountStatus(this IFleetModel fleet, ShipStatus status)
        {
            var count = 0;
            foreach (var fleetShip in fleet.Ships)
            {
                if (fleetShip.Status == status) count++;
            }

            return count;
        }

        public static IShipInfo GetInfo(this IFleetModel fleet, Combat.Component.Ship.IShip unit)
        {
            var count = fleet.Ships.Count;
            for (var i = 0; i < count; ++i)
            {
                var ship = fleet.Ships[i];
                if (ship.ShipUnit == unit)
                    return ship;
            }

            return null;
        }

        public static bool IsAnyShipLeft(this IFleetModel fleet)
        {
            return fleet.AnyAvailableShip() != null;
        }

        public static bool IsAnyShipAlive(this IFleetModel fleet)
        {
            var count = fleet.Ships.Count;
            for (var i = 0; i < count; ++i)
            {
                var ship = fleet.Ships[i];
                if (ship.Status != ShipStatus.Destroyed)
                    return true;
            }

            return false;
        }

        public static IShipInfo AnyAvailableShip(this IFleetModel fleet)
        {
            var count = fleet.Ships.Count;
            for (var i = 0; i < count; ++i)
            {
                var ship = fleet.Ships[i];
                if (ship.Status == ShipStatus.Ready)
                    return ship;
            }

            return null;
        }

        public static IShipInfo LastActivated(this IFleetModel fleet)
        {
            var lastActivationTime = 0f;
            IShipInfo lastActivatedShip = null;

            var count = fleet.Ships.Count;
            for (var i = 0; i < count; ++i)
            {
                var ship = fleet.Ships[i];
                if (ship.ActivationTime > lastActivationTime)
                {
                    lastActivationTime = ship.ActivationTime;
                    lastActivatedShip = ship;
                }
            }

            return lastActivatedShip;
        }

        public static void DestroyAllShips(this IFleetModel fleet)
        {
            foreach (var ship in fleet.Ships)
                ship.Destroy();
        }
    }
}
