namespace GameModel
{
    namespace Upgrades
	{
		public enum UpgradeType
		{
			// Common
			Experience = 1,
			Attack     = 2,
			Defense    = 3,

			// Mothership
			FuelTankCapacity  = 4,

			// Planetary
			PlanetaryAttack   = 5,
			PlanetaryDefense  = 6,
			RadarRange = 7,
			PlanetarySpeed  = 8,
			PlanetaryEnergy = 9,

			PlanetaryMissile = 10,
		}
	}
}