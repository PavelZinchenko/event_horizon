using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Component.Ship;
using Combat.Scene;

namespace Combat.Ai
{
	public static class StrategySelector
	{
		//public static IStrategy BestAvailable(IShipCombatModel ship, IShipCombatModel enemy, int level, ISceneObsolete scene)
		//{
		//	return AllSuitable(ship, enemy, level).MinValue(item => -item.Value).Key.Create(ship, enemy, level, scene);
		//}

		public static IStrategy Random(IShip ship, IShip enemy, int level, System.Random random, IScene scene)
		{
		    const float variation = 0.3f;
		    return AllSuitable(ship, enemy, level).MinValue(item => random.NextFloat()*variation - item.Value).Key.Create(ship, enemy, level, scene);
		}

        private static IEnumerable<KeyValuePair<StrategyType, float>> AllSuitable(IShip ship, IShip enemy, int level)
        {
            foreach (var type in Enum.GetValues(typeof(StrategyType)).Cast<StrategyType>())
            {
                var applicability = type.Applicability(ship, enemy, level);
                if (applicability > 0)
                    yield return new KeyValuePair<StrategyType, float>(type, applicability);
            }
        }
    }
}
