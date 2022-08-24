using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Scene;

namespace Combat.Ai
{
	public class CloseRange : StrategyBase
	{
		public static float SuitabilityLevel(IShip ship, IShip enemy, int level)
		{
			var range = Helpers.ShipMaxRange(ship);
			var enemyRange = Helpers.ShipMaxRange(enemy);

			var value = enemyRange / (range + enemyRange);
			value *= enemy.Engine.MaxVelocity / (ship.Engine.MaxVelocity + enemy.Engine.MaxVelocity);

			return value;
		}

		public CloseRange(IShip ship, IShip enemy, int level, IScene scene)
		{
			var attackRange = Helpers.ShipMinRange(ship);
			var enemyDistance = Helpers.ShipMaxRange(enemy);
			var rechargingState = new State<bool>();
			
			if (level >= 40)
				this.AttackIfTooClose(ship, rechargingState, enemyDistance, level);
			
			this.AvoidPlanets(scene);
			this.LaunchDrones(ship);
			this.Kamikaze(ship, enemy);
			this.UseDevices(ship, enemy, rechargingState, level);
			this.AttackIfInRange(ship, rechargingState, level);

			if (level >= 20)
				this.UseDefenseSystems(ship, level);

			if (level >= 50)
				this.AvoidThreats();
				
			if (level >= 75)
			{
				AddPolicy(
					new Condition.All(new Condition.CanCatch(), new Condition.IsFlagNotSet(rechargingState)),
					new FollowAction(attackRange),
					new EscapeAction(2*enemyDistance));
			}

			AddPolicy(
				new AlwaysTrueCondition(),
				new FollowAction(attackRange));

			this.UseAccelerators(ship, rechargingState);
		}

		public override bool IsThreat(IShip ship, IUnit unit)
		{
			return true;
		}
    }
}
