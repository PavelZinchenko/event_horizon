using Constructor.Ships;

namespace GameModel
{
	public struct ExperienceData
	{
		public ExperienceData(IShip ship, long gained)
		{
			Ship = ship;
			ShipName = ship.Name;
			ExperienceBefore = (long)ship.Experience;
			ExperienceAfter = ship.Experience + gained;
		}

        public ExperienceData(long before, long gained)
        {
            Ship = null;
            ShipName = string.Empty;
            ExperienceBefore = before;
            ExperienceAfter = before + gained;
        }

        public readonly IShip Ship;
		public readonly string ShipName;
		public readonly ObscuredLong ExperienceAfter;
		public readonly ObscuredLong ExperienceBefore;

        public static readonly ExperienceData Empty = new ExperienceData(0,0);
	}
}

