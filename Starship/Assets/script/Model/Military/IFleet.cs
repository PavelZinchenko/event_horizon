using System.Collections.Generic;
using Constructor.Ships;

namespace Model
{
	namespace Military
	{
		public interface IFleet
		{
			IEnumerable<IShip> Ships { get; }
			int AiLevel { get; }
			float Power { get; }
		}
	}
}
