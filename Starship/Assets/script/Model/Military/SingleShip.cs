using System;
using System.Collections.Generic;
using System.Linq;
using Constructor;
using Constructor.Ships;

namespace Model
{
	namespace Military
	{
		public class SingleShip : IFleet
		{
			public SingleShip(IShip ship, int aiLevel = 100)
			{
				_ship = ship;
				AiLevel = aiLevel;
			}
			
			public IEnumerable<IShip> Ships { get { yield return _ship; } }
			public int AiLevel { get; private set; }			
			public float Power { get { return Maths.Threat.GetShipPower(_ship); } }

			private IShip _ship;
		}
	}
}
