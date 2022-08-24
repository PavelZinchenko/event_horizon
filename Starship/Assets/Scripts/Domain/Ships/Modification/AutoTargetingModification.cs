using System.Linq;
using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class AutoTargetingModification : IShipModification
    {
        public static bool IsSuitable(Ship ship)
        {
            return /*ship.Info.SizeClass <= SizeClass.Destroyer &&*/ ship.Barrels.Any(item => item.PlatformType == PlatformType.Common);
        }

        public ModificationType Type => ModificationType.AutoTargeting;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_AutoTargeting");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.AutoTargeting = true;
        }

        public int Seed => 0;
    }
}
