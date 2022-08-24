using System.Linq;
using Combat.Ai.Condition;
using Combat.Component.Ship;
using Combat.Component.Systems.Devices;
using Combat.Component.Unit;
using Combat.Scene;
using UnityEngine;

namespace Combat.Ai
{
    public class Ramming : StrategyBase
    {
        public static float SuitabilityLevel(IShip ship, IShip enemy, int level)
        {
            var hasAccelerator = ship.Systems.All.Any(device => device is AcceleratorDevice);
            if (ship.Engine.MaxVelocity < 0.75f * enemy.Engine.MaxVelocity && !hasAccelerator)
                return 0f;

            var hasFortification = ship.Systems.All.Any(device => device is FortificationDevice);
            if (ship.Stats.Resistance.Kinetic <= enemy.Stats.Resistance.Kinetic && !hasFortification)
                return 0f;

            var value = 0f;
            if (hasAccelerator)
                value += 0.5f;
            else
                value += 0.5f * ship.Engine.MaxVelocity / (enemy.Engine.MaxVelocity + ship.Engine.MaxVelocity);

            if (hasFortification)
                value += 0.5f;
            else
                value += 0.5f * ship.Stats.Resistance.Kinetic / (enemy.Stats.Resistance.Kinetic + ship.Stats.Resistance.Kinetic);

            return value;
        }

        public Ramming(IShip ship, IShip enemy, int level, IScene scene)
        {
            var attackRange = Helpers.ShipMinRange(ship);
            var enemyDistance = Helpers.ShipMaxRange(enemy);
            var rechargingState = new State<bool>();
            var rammingState = new State<bool>();

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

            int fortificationDeviceId = -1;
            int accelerationDeviceId = -1;
            for (var i = 0; i < ship.Systems.All.Count; ++i)
            {
                var device = ship.Systems.All.Device(i);
                if (device == null)
                    continue;

                if (device is FortificationDevice)
                    fortificationDeviceId = i;
                else if (device is AcceleratorDevice)
                    accelerationDeviceId = i;
            }

            AddPolicy(
                new EnergyRechargedCondition(0.25f, 0.75f, rechargingState),
                new RammingAction(rammingState, fortificationDeviceId, accelerationDeviceId));

            AddPolicy(
                new Any(new IsFalseCondition(rammingState), new IsTrueCondition(rechargingState)),
                new KeepDistanceAction(enemyDistance, Mathf.Max(enemyDistance * 1.3f, attackRange), Helpers.AverageWeaponDirection(ship)));

            this.UseAccelerators(ship, rechargingState);
        }

        public override bool IsThreat(IShip ship, IUnit unit)
        {
            return true;
        }
    }
}
