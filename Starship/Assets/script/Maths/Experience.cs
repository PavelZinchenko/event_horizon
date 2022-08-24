using System;
using Constructor.Ships;

namespace Maths
{
	public struct Experience
	{
		public Experience(long value, bool limitMaxLevel = true)
		{
		    if (value < 0)
		        value = 0;
		    else if (limitMaxLevel && value > MaxExperience)
		        value = MaxExperience;

			_value = value;
		}

		public int Level
		{
			get
			{
				var exp = Value;
				var level = (int)Math.Pow(exp/100, 1.0/3.0);

				if (LevelToExp(level+1) <= exp)
					return level+1;

				return level;
			}
		}

		public float PowerMultiplier { get { return LevelToPowerMultiplier(Level); } }

		public long ExpFromLastLevel { get { return Value - LevelToExp(Level); } }

	    public long NextLevelCost
		{
			get
			{
				var level = Level;
				return LevelToExp(level+1) - LevelToExp(level);
			}
		}

		public static Experience FromLevel(int level)
		{
			return new Experience(LevelToExp(level));
		}

		public static float LevelToPowerMultiplier(int level)
		{
			return (float)Math.Pow(11, (0.01*level));
		}

        public static long TotalExpForShip(IShip ship)
        {
            return (1L + (long)Math.Pow(ship.Experience.Level + 1, 1.2)) * (long)Math.Pow(ship.Model.Layout.CellCount, 1.2) * (1 + (int)ship.ExtraThreatLevel);
        }

        public static implicit operator Experience(long value)
		{
			return new Experience(value);
		}
		
		public static implicit operator long(Experience data)
		{
			return data.Value;
		}
		
		public override string ToString ()
		{
			return Value.ToString();
		}

		private long Value { get { return _value; } }

		private static long LevelToExp(int level) { return 100L*level*level*level; }

		private readonly ObscuredLong _value;

		public const int MaxRank = 300;
        public const long MaxExperience = 100L * MaxRank * MaxRank * MaxRank;
	    public const int MaxPlayerRank = 100;
	    public const int MaxPlayerRank2 = 200;
	    public const long MaxPlayerExperience = 100L * MaxPlayerRank * MaxPlayerRank * MaxPlayerRank;
	    public const long MaxPlayerExperience2 = 100L * MaxPlayerRank2 * MaxPlayerRank2 * MaxPlayerRank2;
    }
}
