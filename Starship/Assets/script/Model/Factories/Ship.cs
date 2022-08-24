using System;
using System.Linq;
using Constructor.Satellites;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;

namespace Model
{
	namespace Factories
	{
		public static class Ship
		{
			public static IShip Create(ShipBuild data, int distance, Random random, IDatabase database)
			{
				var shipLevel = Maths.Distance.ToShipLevel(distance);
				var delta = Math.Min(10, shipLevel/5);
				shipLevel += random.Next(delta+1) - delta/2;
				var ship = new EnemyShip(data) { Experience = Maths.Experience.FromLevel(shipLevel) };

				if (data.Ship.ShipCategory == ShipCategory.Flagship || data.Ship.ShipCategory == ShipCategory.Starbase)
					return ship;
				
				var companionClass = Maths.Distance.CompanionClass(distance);
				
				var companions = database.SatelliteBuildList.LimitClass(companionClass).SuitableFor(data.Ship);

			    if (companions.Any())
			    {
			        if (random.Next(3) != 0)
			            ship.FirstSatellite = new CommonSatellite(companions.RandomElement(random));
			        if (random.Next(3) != 0)
			            ship.SecondSatellite = new CommonSatellite(companions.RandomElement(random));
			    }

			    return ship;
			}
		}
	}
}
