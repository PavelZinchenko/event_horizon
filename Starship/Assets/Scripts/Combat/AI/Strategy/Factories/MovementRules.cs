using Combat.Ai.Condition;
using Combat.Component.Ship;
using Combat.Component.Systems.Devices;
using Combat.Scene;

namespace Combat.Ai
{
	public static class MovementRules
	{
		public static void AvoidPlanets(this StrategyBase strategy, IScene scene)
		{
            //foreach (var unit in scene..Where(unit => unit is Planet))
            //    strategy.AddPolicy(new AlwaysTrueCondition(), new AvoidObjectAction(unit, 10f));
		}

		public static void UseAccelerators(this StrategyBase strategy, IShip ship, State<bool> rechargingState)
        {
            for (var i = 0; i < ship.Systems.All.Count; i++)
            {
                var device = ship.Systems.All.Device(i);
                if (device == null)
                    continue;

                if (device is AcceleratorDevice)
                    strategy.AddPolicy(new All(new HasEnergyCondition(0.5f), new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)), new BoostAction(i));
                else if (device is TeleporterDevice)
                	strategy.AddPolicy(new All(new HasEnergyCondition(0.5f), new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)), new BoostWithTeleportAction(i));
            }
        }
	}
}
