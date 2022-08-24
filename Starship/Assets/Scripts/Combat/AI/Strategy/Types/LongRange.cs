using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Scene;
using UnityEngine;

namespace Combat.Ai
{
    public class LongRange : StrategyBase
    {
        public static float SuitabilityLevel(IShip ship, IShip enemy, int level)
        {
            if (level == 104) // TODO
                return -1f;
            if (level < 20)
                return 0f;

            var attackRange = Helpers.ShipMaxRange(ship);
            var enemyRange = Helpers.ShipMaxRange(enemy);

            float value = attackRange / (attackRange + enemyRange);

            if (Mathf.Abs(Helpers.AverageWeaponDirection(ship)) > 75)
                value *= 1.2f;

            return Mathf.Clamp01(value);
        }

        public LongRange(IShip ship, IShip enemy, int level, IScene scene)
        {
            var attackRange = Helpers.ShipMaxRange(ship);
            var enemyRange = Helpers.ShipMaxRange(enemy);
            var rechargingState = new State<bool>();

            if (level >= 40)
                this.AttackIfTooClose(ship, rechargingState, enemyRange, level);

            if (level >= 50)
                this.AvoidThreats();

            this.AvoidThreats();
            this.AvoidPlanets(scene);
            this.Kamikaze(ship, enemy);
            this.AttackIfInRange(ship, rechargingState, level);

            AddPolicy(
                new AlwaysTrueCondition(),
                new KeepDistanceAction(attackRange * 0.7f, attackRange * 0.9f, Helpers.AverageWeaponDirection(ship)));

            this.LaunchDrones(ship);
            this.UseDevices(ship, enemy, rechargingState, level);

            if (level >= 20)
                this.UseDefenseSystems(ship, level);

            this.UseAccelerators(ship, rechargingState);
        }

        public override bool IsThreat(IShip ship, IUnit unit)
        {
            return true;
        }
    }
}
