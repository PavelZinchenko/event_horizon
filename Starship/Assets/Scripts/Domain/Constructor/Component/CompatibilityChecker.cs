using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Enums;

namespace Constructor.Component
{
    public static class CompatibilityChecker
    {
        public static bool IsCompatibleComponent(GameDatabase.DataModel.Component component, Ships.IShipModel ship)
        {
            if (component == null) return false;

            if (!component.Restrictions.ShipSizes.IsEmpty && !component.Restrictions.ShipSizes.Contains(ship.SizeClass)) return false;
            if (component.Restrictions.NotForMechanicShips && !ship.IsBionic) return false;
            if (component.Restrictions.NotForOrganicShips && ship.IsBionic) return false;

            if (component.Device != null && !IsCompatibleDevice(component.Device, ship)) return false;
            if (component.Weapon != null && !IsCompatibleWeapon(component, ship)) return false;

            return true;
        }

        public static bool IsCompatibleDevice(Device device, Ships.IShipModel ship)
        {
            //if (ship.Info.ShipCategory == ShipCategory.Starbase)
            //    return false;

            var type = device.Stats.DeviceClass;

            switch (type)
            {
                case DeviceClass.RepairBot:
                    return !ship.IsBionic;
            }

            var count = ship.Stats.BuiltinDevices.Count;
            for (var i = 0; i < count; ++i)
                if (ship.Stats.BuiltinDevices[i].Stats.DeviceClass == type)
                    return false;

            return true;
        }

        public static bool IsCompatibleWeapon(GameDatabase.DataModel.Component component, Ships.IShipModel ship)
        {
            //if (ship.Info.ShipCategory == ShipCategory.Starbase && component.Layout.CellCount < 4)
            //    return false;

            return true;
        }
    }
}
