using UnityEngine;

namespace Combat.Ai
{
	public class KamikazeAction : IAction
	{
		public KamikazeAction(int deviceId)
		{
			_deviceId = deviceId;
			_time = 1.0f; // TODO
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			var ship = context.Ship;
			var enemy = context.Enemy;
			if (ship.Stats.Armor.Value > enemy.Stats.Armor.Value + enemy.Stats.Shield.Value)
				return;

			var shipPosition = ship.Body.Position + ship.Body.Velocity * _time;
			var enemyPosition = enemy.Body.Position + enemy.Body.Velocity * _time;
			if (Vector2.Distance(shipPosition, enemyPosition) <= ship.Body.Scale/2 + enemy.Body.Scale/2)
			{
				controls.ActivateSystem(_deviceId);
				controls.Thrust = 0f;
			}
			else
			{
				Vector2 target;
				float timeInterval;

				if (!Geometry.GetTargetPosition(
					enemy.Body.Position,
					enemy.Body.Velocity,
					ship.Body.Position,
					ship.Engine.MaxVelocity,
					out target,
					out timeInterval))
				{
					return;
				}

				var direction = ship.Body.Position.Direction(target);
				var course = RotationHelpers.Angle(direction);
				controls.Course = course;

				if (Mathf.Abs(Mathf.DeltaAngle(ship.Body.Rotation, course)) < 30)
				{
					controls.Thrust = 1.0f;
				}
			}
		}
		
		private readonly int _deviceId;
		private readonly float _time;
	}
}
