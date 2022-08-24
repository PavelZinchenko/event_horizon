using Combat.Ai.Condition;
using Combat.Component.Ship;
using Combat.Component.Systems.Weapons;
using UnityEngine;

namespace Combat.Ai
{
	public static class AttackRules
	{
		public static void AttackIfInRange(this StrategyBase strategy, IShip ship, State<bool> rechargingState, int level)
		{
			var wakeTime = 10 + 3*level;
			strategy.AddPolicy(new AlwaysTrueCondition(), new TrackTargetAction());

			strategy.AddPolicy(new All(
				new AwakeCondition(wakeTime),
				new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)),
				new CommonWeaponsAttackAction(ship, level < 100, false));

			for (var i = 0; i < ship.Systems.All.Count; i++)
			{
				var weapon = ship.Systems.All.Weapon(i);
                if (weapon == null)
                    continue;

                if (weapon.Info.WeaponType == WeaponType.Manageable)
                    strategy.AddPolicy(new AlwaysTrueCondition(), new ControlledWeaponAction(i, false));

			    if (weapon.Info.BulletEffectType == BulletEffectType.Repair)
			        strategy.AddPolicy(new EnergyRechargedCondition(0.1f, 0.9f, rechargingState), new DroneRepairAction(i));

                if (weapon.Info.BulletEffectType == BulletEffectType.Special || weapon.Info.BulletEffectType == BulletEffectType.Repair)
                {
                    strategy.AddPolicy(new All(
                        new AwakeCondition(wakeTime),
                        new HasEnergyCondition(0.5f),
                        new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)
                        ), ActionFactory.CreateAttackAction(ship, i, level));
                }

                if (weapon.Info.WeaponType == WeaponType.RequiredCharging)
                    strategy.AddPolicy(new AlwaysTrueCondition(), new ChargedWeaponAction(i));
            }

            strategy.AddPolicy(new All(
                new AwakeCondition(wakeTime),
                new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)),
                new CommonWeaponsAttackAction(ship, level < 100, true));

            for (var i = 0; i < ship.Systems.All.Count; i++)
            {
                var weapon = ship.Systems.All.Weapon(i);
                if (weapon == null)
                    continue;

                if (weapon.Info.WeaponType == WeaponType.Manageable)
                    strategy.AddPolicy(new AlwaysTrueCondition(), new ControlledWeaponAction(i, true));
            }

            if (wakeTime < 100)
                strategy.AddPolicy(new NotCondition(new AwakeCondition(wakeTime)), new TrackTargetAction(true));
        }

        public static void DroneRepair(this StrategyBase strategy, IShip ship, State<bool> rechargingState)
        {
            for (var i = 0; i < ship.Systems.All.Count; i++)
            {
                var weapon = ship.Systems.All.Weapon(i);
                if (weapon == null)
                    continue;

                if (weapon.Info.BulletEffectType == BulletEffectType.Repair)
                    strategy.AddPolicy(new EnergyRechargedCondition(0.1f, 0.9f, rechargingState), new DroneRepairAction(i));
            }
        }

        public static void AttackIfTooClose(this StrategyBase strategy, IShip ship, State<bool> rechargingState, float minDistance, int level)
		{
			var wakeTime = 10 + 3*level;
			strategy.AddPolicy(new AlwaysTrueCondition(), new TrackTargetAction());

			strategy.AddPolicy(new All(
				new AwakeCondition(wakeTime),
				new DistanceCondition(0, minDistance),
				new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)),
                new CommonWeaponsAttackAction(ship, level < 100, false));

            for (var i = 0; i < ship.Systems.All.Count; i++)
            {
                var weapon = ship.Systems.All.Weapon(i);
                if (weapon == null)
                    continue;

                if (weapon.Info.WeaponType == WeaponType.Manageable)
                    strategy.AddPolicy(new AlwaysTrueCondition(), new ControlledWeaponAction(i, false));

                if (weapon.Info.BulletEffectType == BulletEffectType.Special || weapon.Info.BulletEffectType == BulletEffectType.Repair)
                {
                    strategy.AddPolicy(new All(
                        new AwakeCondition(wakeTime),
                        new HasEnergyCondition(0.5f),
                        new DistanceCondition(0, Mathf.Min(weapon.Info.Range, minDistance)),
                        new EnergyRechargedCondition(0.1f, 0.9f, rechargingState)
                        ), ActionFactory.CreateAttackAction(ship, i, level));
                }
            }
        }
	}
}
