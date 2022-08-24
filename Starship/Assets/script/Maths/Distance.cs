using System;
using GameDatabase.Enums;

namespace Maths
{
	public static class Distance
	{
		public static int ToShipLevel(int distance)
		{
			var level = 3*distance/5 - 5;
			return UnityEngine.Mathf.Clamp(level, 0, Experience.MaxRank);
		}

		public static int ToShipSize(int distance)
		{
			return 25 + 2*distance;
		}

		public static int FromShipLevel(int level)
		{
			return 5*(level + 5)/3;
		}

		public static int FleetSize(int distance, System.Random random)
		{
			var max = 3 + random.Next(100)*random.Next(100)/2000;
			return 1 + System.Math.Min(max, distance/3);
		}
		
		public static int CombatTime(int distance) { return Math.Max(40, 100 - distance); }
		public static int AiLevel(int distance) { return distance*2; }
		public static int ComponentLevel(int distance) { return Math.Max(distance, 1); }
        public static int Credits(int distance) { return 100 + Math.Max(distance, 1) * 2; }
        public static DifficultyClass MaxShipClass(int distance) { return (DifficultyClass)(distance/25); }
		public static DifficultyClass MinShipClass(int distance) { return distance < 50 ? DifficultyClass.Default : DifficultyClass.Class1; }
		public static DifficultyClass CompanionClass(int distance) { return (DifficultyClass)((distance+10)/20); }
	}
}