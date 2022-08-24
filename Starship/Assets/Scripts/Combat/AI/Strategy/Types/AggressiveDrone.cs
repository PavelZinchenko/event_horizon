using Combat.Ai.Condition;
using Combat.Component.Ship;
using Combat.Component.Unit;

namespace Combat.Ai
{
    public class AggressiveDrone : StrategyBase
    {
        public AggressiveDrone(IShip ship, IShip enemy, float range, bool improvedAi)
        {
            var random = new System.Random();
            var attackRange = Helpers.ShipMaxRange(ship);
            var rechargingState = new State<bool>();
            var droneRadius = 2.5f + random.NextFloat();

            AddPolicy(new DroneOutOfRangeCondition(range), new DroneReturnAction(droneRadius));

            if (improvedAi)
            {
                this.LaunchDrones(ship);
                this.Kamikaze(ship, enemy);
                this.UseDevices(ship, enemy, rechargingState, 100);
            }

            this.AttackIfInRange(ship, rechargingState, 100);

            if (improvedAi)
            {
                this.UseDefenseSystems(ship, 100);
                this.AvoidThreats();
                this.UseAccelerators(ship, rechargingState);
            }

            AddPolicy(
                new DroneTargetOutOfRangeCondition(range + attackRange*0.8f),
                new DroneReturnAction(droneRadius),
                new FollowAction(attackRange*0.9f));

            AddPolicy(new None(new DroneTargetOutOfRangeCondition(range + attackRange*0.8f), new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)),
                new KeepDistanceAction(attackRange*0.3f, attackRange*0.9f, random.NextFloat() > 0.5f ? 1 : -1));

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
