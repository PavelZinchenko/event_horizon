using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameModel
{
	public static class StarLayout
	{
		public static int PositionToId(int x, int y)
		{
			var distance = Math.Max(Math.Abs(x), Math.Abs(y));
			var length = distance*2 + 1;
			var id = length*length - 1;
			
			if (y == -distance)
				id -= distance + x;
			else if (y == distance)
				id -= distance + length + x;
			else if (x == -distance)
				id -= 2*length + (y + distance - 1)*2;
			else if (x == distance)
				id -= 2*length + (y + distance - 1)*2 + 1;
			
			return id;
		}
		
		public static void IdToPosition(int id, out int x, out int y)
		{
			var length = (int)Math.Floor(Math.Sqrt(id)) + 1;
			if (length % 2 == 0) length++;
			var distance = (length - 1)/2;
			
			id = length*length - 1 - id;
			
			if (id < length)
			{
				y = -distance;
				x = id - distance;
			}
			else if (id < 2*length)
			{
				y = distance;
				x = id - distance - length;
			}
			else
			{
				x = (id - 2*length) % 2 == 0 ? -distance : distance;
				y = (id - 2*length) / 2 - distance + 1;
			}
		}

		public static Vector2 GetStarPosition(int id, int gameseed)
		{
			int x,y;
			StarLayout.IdToPosition(id, out x, out y);
			var offset = y % 2 == 0 ? 0.0f : 0.5f;
			return new Vector2((x+offset)*_stepX, y*_stepY) + GetStarOffset(id, gameseed);
		}

        public static int GetStarLevel(int id, int gameseed)
        {
            var position = GetStarPosition(id, gameseed);
            return Mathf.RoundToInt(position.magnitude);
        }

        /*public static IEnumerable<int> GetAdjacentStars(int id)
		{
			int x, y;
			StarLayout.IdToPosition(id, out x, out y);
			bool odd = y % 2 == 0;

			if (odd)
			{
				yield return PositionToId(x - 1, y - 1);
				yield return PositionToId(x, y - 1);
			}
			else
			{
				yield return PositionToId(x, y - 1);
				yield return PositionToId(x + 1, y - 1);
			}

			yield return PositionToId(x + 1, y);

			if (odd)
			{
				yield return PositionToId(x, y + 1);
				yield return PositionToId(x - 1, y + 1);
			}
			else
			{
				yield return PositionToId(x + 1, y + 1);
				yield return PositionToId(x, y + 1);
			}

			yield return PositionToId(x - 1, y);
		}*/

        public static IEnumerable<int> GetAdjacentStars(int id, int distance = 1)
		{
            IdToPosition(id, out var x0, out var y0);
			var offset = y0 % 2 == 0 ? 0 : 1;
			var odd = distance % 2 == 0;

			yield return PositionToId(x0 - distance, y0);

			for (int i = 1; i < distance; ++i)
			{
				var x = -distance + (i + offset)/2;
				yield return PositionToId(x0+x, y0-i);
			}

			int start = odd ? -distance/2 : offset-1-distance/2;
			for (int i = 0; i <= distance; ++i)
				yield return PositionToId(x0 + start + i, y0-distance);

			for (int i = 1; i < distance; ++i)
			{
				var x = distance - (i + 1 - offset)/2;
				yield return PositionToId(x0+x, y0 + i);
			}

			yield return PositionToId(x0 + distance, y0);

			for (int i = 1; i < distance; ++i)
			{
				var x = distance - (distance - i + 1 - offset)/2;
				yield return PositionToId(x0+x, y0 - distance + i);
			}

			start = odd ? -distance/2 : offset-1-distance/2;
			for (int i = distance; i >= 0; --i)
				yield return PositionToId(x0 + start + i, y0+distance);

			for (int i = 1; i < distance; ++i)
			{
				var x = -distance + (distance - i + offset)/2;
				yield return PositionToId(x0+x, y0+distance-i);
			}
		}

        public static int Distance(int star1, int star2)
        {
            IdToPosition(star1, out var x1, out var y1);
            IdToPosition(star2, out var x2, out var y2);
            return Distance(x1, y1, x2, y2);
        }

		public static int Distance(int x0, int y0, int x1, int y1)
		{
			var dy = Math.Abs(y0-y1);
			var odd = y0 % 2 == 0;
			var dx = x1 >= x0 ? x1-x0 + (odd ? dy + 1 : dy)/2 : x0-x1 + (odd ? dy : dy + 1)/2;

			return Math.Max(dx,dy);
		}

		public static float DistanceX { get { return _stepX; } }
		public static float DistanceY { get { return _stepY; } }

		private static Vector2 GetStarOffset(int id, int gameseed)
		{
            UnityEngine.Random.InitState(gameseed + id);
            return UnityEngine.Random.insideUnitCircle * _offset;
        }

	    private const float _offset = 0.3f;
	    private const float _stepX = 1.0f;
	    private static readonly float _stepY = Mathf.Sqrt(3)/2;
	}
}