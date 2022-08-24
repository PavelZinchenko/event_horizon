using System.Linq;
using Combat.Component.Ship;
using Combat.Component.Systems.Weapons;
using Combat.Component.Unit;
using Combat.Scene;
using UnityEngine;

namespace Combat.Ai
{
    public class HitAndRun : StrategyBase
    {
        public static float SuitabilityLevel(IShip ship, IShip enemy, int level)
        {
            return -1f; // TODO

            /*if (level < 50)
				return 0f;
			if (2*ship.HitPoints.MaxValue > 3*enemy.HitPoints.MaxValue)
				return 0f;

			var range = Helpers.ShipMaxRange(ship);
			var enemyRange = ShipTurretRange(enemy);            
			if (range*0.7f > enemyRange)
				return 0;

			var value = 0.6f - enemy.Energy.EnergyRechargeRate / enemy.Energy.MaxValue;
			value += 0.2f*ship.MaxVelocity/enemy.MaxVelocity;

			return Mathf.Clamp01(value);*/
        }

        public HitAndRun(IShip ship, IShip enemy, int level, IScene scene)
        {
            var attackRange = Helpers.ShipMinRange(ship);
            var enemyDistance = ShipTurretRange(enemy);
            var rechargingState = new State<bool>();

            this.AvoidPlanets(scene);
            this.Kamikaze(ship, enemy);

            AddPolicy(
                new Condition.Any(new Condition.EnemyHasEnergy(0.2f), new Condition.IsFlagSet(rechargingState)),
                new KeepDistanceAction(enemyDistance, Mathf.Max(enemyDistance * 1.3f, attackRange), Helpers.AverageWeaponDirection(ship)));

            this.AttackIfInRange(ship, rechargingState, level);

            AddPolicy(
                new Condition.None(new Condition.EnemyHasEnergy(0.2f), new Condition.IsFlagSet(rechargingState)),
                new FollowAction(attackRange));

            this.LaunchDrones(ship);
            this.UseDevices(ship, enemy, rechargingState, level);
            this.UseDefenseSystems(ship, level);

            this.UseAccelerators(ship, rechargingState);
        }

        public override bool IsThreat(IShip ship, IUnit unit)
        {
            return true;
        }

        private static float ShipTurretRange(IShip ship)
        {
            float range = 0;
            foreach (var weapon in ship.Systems.All.OfType<IWeapon>())
            {
                if (weapon.Platform.AutoAimingAngle >= 160)
                    range = Mathf.Max(range, weapon.Info.Range);
            }

            return range;
        }
    }
}
