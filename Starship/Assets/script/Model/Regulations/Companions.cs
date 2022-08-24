using GameDatabase.DataModel;

namespace Model
{
	namespace Regulations
	{
		public static class Companions
		{
			public static bool IsSuitable(Satellite satellite, Ship ship)
			{
				return ship.ModelScale >= satellite.ModelScale*2;
			}
		}
	}
}
