using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameModel
{
	public static class RegionLayout
	{
		public static int PositionToId(int x, int y)
		{
			return 1 + StarLayout.PositionToId(x,y);
		}
		
		public static void IdToPosition(int id, out int x, out int y)
		{
			if (id <= 0)
			{
				x = 0;
				y = 0;
			}
			else
			{
				StarLayout.IdToPosition(id-1, out x, out y);
			}
		}
		
		public static int GetRegionHomeStar(int regionId)
		{
			if (regionId < 0)
				return 0;
			
			int x,y;
			IdToPosition(regionId, out x, out y);
			x *= 3*RegionFourthSize;
			y *= 2*RegionFourthSize;
			
			return StarLayout.PositionToId(x,y);
		}

		public const int RegionFourthSize = 2;
	}
}
