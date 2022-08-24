using System.ComponentModel;
using Combat.Component.Ship;
using Combat.Scene;

namespace Combat.Ai
{
    public enum StrategyType
    {
        CloseRange,
        HitAndRun,
        LongRange,
        Ramming,
    }

    public static class StrategyTypeExtensions
    {
        public static float Applicability(this StrategyType type, IShip ship, IShip enemy, int level)
        {
            switch (type)
            {
                case StrategyType.CloseRange:
                    return CloseRange.SuitabilityLevel(ship, enemy, level);
                case StrategyType.HitAndRun:
                    return HitAndRun.SuitabilityLevel(ship, enemy, level);
                case StrategyType.LongRange:
                    return LongRange.SuitabilityLevel(ship, enemy, level);
                case StrategyType.Ramming:
                    return Ramming.SuitabilityLevel(ship, enemy, level);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public static StrategyBase Create(this StrategyType type, IShip ship, IShip enemy, int level, IScene scene)
        {
            switch (type)
            {
                case StrategyType.CloseRange:
                    return new CloseRange(ship, enemy, level, scene);
                case StrategyType.HitAndRun:
                    return new HitAndRun(ship, enemy, level, scene);
                case StrategyType.LongRange:
                    return new LongRange(ship, enemy, level, scene);
                case StrategyType.Ramming:
                    return new Ramming(ship, enemy, level, scene);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
