using System;

namespace GameModel.Skills
{
    public struct Experience
    {
        public Experience(long value)
        {
            if (value < 0)
                value = 0;
            else if (value > MaxExperience)
                value = MaxExperience;

            _value = value;
        }

        public int Level
        {
            get
            {
                var exp = Value;
                var level = (int)Math.Sqrt(exp/100);
                return level;
            }
        }

        public static long ConvertCombatExperience(long experience, int currentLevel)
        {
            return experience > 0 ? 50 + experience/(25 + currentLevel) : 0;
        }

        public long ExpFromLastLevel { get { return Value - LevelToExp(Level); } }

        public long NextLevelCost
        {
            get
            {
                var level = Level;
                return LevelToExp(level + 1) - LevelToExp(level);
            }
        }

        public static Experience FromLevel(int level)
        {
            return new Experience(LevelToExp(level));
        }

        public static implicit operator Experience(long value)
        {
            return new Experience(value);
        }

        public static implicit operator long(Experience data)
        {
            return data.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        private long Value { get { return _value; } }

        private static long LevelToExp(int level) { return 100L * level * level; }

        private readonly ObscuredLong _value;

        public static long MaxExperience = int.MaxValue;
    }
}
