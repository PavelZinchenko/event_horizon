using System.Linq;
using Combat.Ai.Condition;
using Combat.Component.Ship;
using Combat.Component.Systems.Devices;
using Combat.Component.Systems.DroneBays;
using Combat.Component.Systems.Weapons;
using Combat.Component.Unit.Classification;
using UnityEngine;

namespace Combat.Ai
{
	public static class SpecialRules
	{
		public static void LaunchDrones(this StrategyBase strategy, IShip ship)
		{
		    for (var i = 0; i < ship.Systems.All.Count; i++)
            {
                var system = ship.Systems.All[i];
                if (system is IDroneBay)
		            strategy.AddPolicy(new AlwaysTrueCondition(), new DroneAction(i));
                if (system is ClonningDevice)
                    strategy.AddPolicy(new AlwaysTrueCondition(), new ActivateDeviceAction(i));
            }
		}

		public static void Kamikaze(this StrategyBase strategy, IShip ship, IShip enemy)
		{
            if (enemy.Type.Class == UnitClass.Drone)
                return;

            for (var i = 0; i < ship.Systems.All.Count; i++)
            {
                var device = ship.Systems.All.Device(i);
                if (device == null)
                    continue;

                if (device is DetonatorDevice)
                    strategy.AddPolicy(new HitPointsCondition(0f, 0.25f), new KamikazeAction(i));
            }
        }

		public static void UseDevices(this StrategyBase strategy, IShip ship, IShip enemy, State<bool> rechargingState, int level)
		{
            float shipAttackRange = Helpers.ShipMaxRange(ship);

            for (var i = 0; i < ship.Systems.All.Count; i++)
            {
                var device = ship.Systems.All.Device(i);
                if (device == null)
                    continue;

                if (device is TeleporterDevice && enemy != null)
                {
                    if (level < 30) continue;

                    var enemyAttackRange = Helpers.ShipMaxRange(enemy);
                    strategy.AddPolicy(
                        new All(new HasEnergyCondition(0.5f), new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)),
                        new TeleportAction(i, shipAttackRange, enemyAttackRange));
                }
                else if (device is RepairSystem)
                {
                    strategy.AddPolicy(new All(new DistanceCondition(shipAttackRange * 1.1f, float.MaxValue), new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)), new RepairAction(i));
                }
                else if (device is DecoyDevice && enemy != null)
                {
                    if (level < 30) continue;

                    float distance = 0;
                    if (enemy.Type.Class == UnitClass.Drone)
                        distance = 20f;

                    foreach (var droneBay in enemy.Systems.All.OfType<IDroneBay>())
                        distance = Mathf.Max(distance, droneBay.Range);

                    foreach (var weapon in enemy.Systems.All.OfType<IWeapon>())
                        if (weapon.Platform.AutoAimingAngle > 5 || weapon.Info.BulletType == BulletType.Homing)
                            distance = Mathf.Max(distance, weapon.Info.Range);

                    if (distance > 0)
                        strategy.AddPolicy(
                            new All(new DistanceCondition(0, distance), new HasEnergyCondition(0.5f)),
                            new ActivateDeviceAction(i));
                }
                else if (device is StealthDevice)
                {
                    strategy.AddPolicy(
                        new AlwaysTrueCondition(),
                        new ActivateDeviceAction(i));
                }
                else if (device is GravityGenerator)
                {
                    var distance = Helpers.ShipMinRange(ship);
                    strategy.AddPolicy(
                        new All(
                            new DistanceCondition(distance, float.MaxValue),
                            new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)),
                        new ActivateDeviceAction(i));
                }
            }
        }
	}
}
