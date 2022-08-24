using System.Collections.Generic;
using Constructor.Ships;

namespace Maths
{
	public static class Threat
	{
		public enum Level { VeryEasy = 0, Easy = 1, Average = 2, Hard = 3, VeryHard = 4 }

		public static float GetShipPower(IShip ship)
		{
			var size = ship.Model.Layout.CellCount;
		    if (ship.FirstSatellite != null)
		        size += ship.FirstSatellite.Information.Layout.CellCount;
            if (ship.SecondSatellite != null)
                size += ship.SecondSatellite.Information.Layout.CellCount;
			
			return ship.Experience.PowerMultiplier * size * (1f + 0.5f*(int)ship.ExtraThreatLevel);
		}
		
		public static float GetShipsPower(IEnumerable<IShip> ships)
		{
			var power = 0f;
			foreach (var ship in ships)
				power += GetShipPower(ship);

			return power;
		}

		public static Level GetLevel(float playerPower, float enemyPower)
		{
			var threat = enemyPower/(playerPower + enemyPower);

			if (threat < 0.2f)
				return Level.VeryEasy;
			else if (threat < 0.4f)
				return Level.Easy;
			else if (threat < 0.6f)
				return Level.Average;
			else if (threat < 0.8f)
				return Level.Hard;
			else
				return Level.VeryHard;
		}
	}
}