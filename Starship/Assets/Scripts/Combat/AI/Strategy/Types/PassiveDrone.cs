using Combat.Component.Ship;
using Combat.Component.Unit;

namespace Combat.Ai
{
    public class PassiveDrone : StrategyBase
    {
        public PassiveDrone(IShip ship)
        {
            var random = new System.Random();
            var rechargingState = new State<bool>();
            var droneRadius = 2.5f + random.NextFloat();

            this.DroneRepair(ship, rechargingState);

            AddPolicy(
                new AlwaysTrueCondition(),
                new DroneReturnAction(droneRadius));

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
