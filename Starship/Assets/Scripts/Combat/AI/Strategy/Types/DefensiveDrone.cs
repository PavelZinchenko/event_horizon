using Combat.Ai.Condition;
using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Scene;

namespace Combat.Ai
{
    public class DefensiveDrone : StrategyBase
    {
        public DefensiveDrone(IShip ship, IShip enemy, float range, bool improvedAi)
        {
            var random = new System.Random();
            var attackRange = Helpers.ShipMaxRange(ship);
            var rechargingState = new State<bool>();
            var droneRadius = 2.5f + random.NextFloat();

            AddPolicy(new DroneOutOfRangeCondition(droneRadius), new DroneReturnAction(droneRadius));

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
                new AnyNot(new DroneTargetOutOfRangeCondition(droneRadius + attackRange*0.8f), new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)), 
                new DroneReturnAction(droneRadius), 
                new FollowAction(attackRange));

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
