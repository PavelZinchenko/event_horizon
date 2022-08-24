using Combat.Ai.Condition;
using Combat.Component.Ship;
using Combat.Component.Unit;

namespace Combat.Ai
{
    public class AggressiveClone : StrategyBase
    {
        public AggressiveClone(IShip ship, IShip enemy)
        {
            var random = new System.Random();
            var attackRange = Helpers.ShipMaxRange(ship);
            var enemyDistance = Helpers.ShipMaxRange(enemy);
            var rechargingState = new State<bool>();

            this.LaunchDrones(ship);
            this.Kamikaze(ship, enemy);
            this.UseDevices(ship, enemy, rechargingState, 100);
            this.AttackIfInRange(ship, rechargingState, 100);

            //this.UseDefenseSystems(ship, 100);
            //this.AvoidThreats();
            this.UseAccelerators(ship, rechargingState);

            AddPolicy(
                new Condition.All(new Condition.CanCatch(), new Condition.IsFlagNotSet(rechargingState)),
                new FollowAction(attackRange),
                new EscapeAction(2 * enemyDistance));

            AddPolicy(
                new AlwaysTrueCondition(),
                new FollowAction(attackRange));

            this.UseAccelerators(ship, rechargingState);

            AddPolicy(
                new MothershipRetreatedCondition(),
                new VanishAction());
        }

        public override bool IsThreat(IShip ship, IUnit unit)
        {
            return true;
        }
    }
}