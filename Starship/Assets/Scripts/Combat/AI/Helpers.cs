using System.Linq;
using Combat.Component.Body;
using Combat.Component.Platform;
using Combat.Component.Ship;
using Combat.Component.Systems.Weapons;
using Combat.Component.Unit;
using UnityEngine;

namespace Combat.Ai
{
	public static class Helpers
	{
		public static float Distance(IUnit first, IUnit second)
		{
		    return Mathf.Max(0.001f, Vector2.Distance(first.Body.Position, second.Body.Position) - first.Body.Scale/2 - second.Body.Scale/2);
		}

		public static float TargetCourse(IUnit ship, Vector2 target, IWeaponPlatform platform)
		{
			return RotationHelpers.Angle(platform.Body.WorldPosition().Direction(target)) + ship.Body.Rotation - platform.FixedRotation;
		}

		public static float ShipMinRange(IShip ship)
		{
			float range = 0;
			foreach (var weapon in ship.Systems.All.OfType<IWeapon>())
			{
                if (weapon.Info.BulletEffectType == BulletEffectType.Special)
                    continue;

                range = range > 0 ? Mathf.Min(range, weapon.Info.Range) : weapon.Info.Range;
			}
			
			return range;
		}
		
		public static float ShipMaxRange(IShip ship)
		{
			float range = 0;
			foreach (var weapon in ship.Systems.All.OfType<IWeapon>())
			{
                if (weapon.Info.BulletEffectType == BulletEffectType.Special)
                    continue;

                range = Mathf.Max(range, weapon.Info.Range);
			}
			
			return range;
		}

		public static float ShipAvgRange(IShip ship)
		{
			float range = 0;
			int count = 0;
			foreach (var weapon in ship.Systems.All.OfType<IWeapon>())
			{
                if (weapon.Info.BulletEffectType == BulletEffectType.Special)
                    continue;

                range += weapon.Info.Range;
				count++;
			}
			
			return count > 0 ? range/count : 0f;
		}

		public static int AverageWeaponDirection(IShip ship)
		{
			float angle = 0;
			var shipRotation = ship.Body.Rotation;
			
			foreach (var weapon in ship.Systems.All.OfType<IWeapon>())
			{
				var rotation = Mathf.DeltaAngle(shipRotation, weapon.Platform.FixedRotation);
				
				if (rotation > 90)
					rotation = 180 - rotation;
				else if (rotation < -90)
					rotation = -180 - rotation;
				
				angle += rotation;
			}
			
			return Mathf.RoundToInt(angle);
		}

		public static float StealthAccuracy = 10;
	}
}
