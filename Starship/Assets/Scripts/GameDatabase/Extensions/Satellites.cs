using System.Collections.Generic;
using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Enums;

namespace GameDatabase.Extensions
{
    public static class SatelliteExtensions
    {
        public static IEnumerable<SatelliteBuild> SuitableFor(this IEnumerable<SatelliteBuild> satellites, Ship ship)
        {
            return satellites.Where(item => ship.ModelScale >= item.Satellite.ModelScale*2);
        }

        public static IEnumerable<SatelliteBuild> LimitClass(this IEnumerable<SatelliteBuild> satellites, DifficultyClass shipClass)
        {
            return satellites.Where(item => item.DifficultyClass <= shipClass);
        }
    }
}
