using UnityEngine;

namespace Combat.Ai
{
	public class FollowAction : IAction
	{
		public FollowAction(float distance)
		{
			_distance = distance;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			var ship = context.Ship;
			var enemy = context.Enemy;

			if (controls.MovementLocked)
				return;
			
			var direction = ship.Body.Position.Direction(enemy.Body.Position).normalized;
			if (Helpers.Distance(ship, enemy) < _distance)
				return;
			var course = RotationHelpers.Angle(direction);
			if (Vector2.Dot(ship.Body.Velocity, direction) < ship.Engine.MaxVelocity*0.95f)
			{
				controls.Course = course;
				if (Mathf.Abs(Mathf.DeltaAngle(course, ship.Body.Rotation)) < 30)
					controls.Thrust = 1;
			}
		}

		private readonly float _distance;
	}
}
