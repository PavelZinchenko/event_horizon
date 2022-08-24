using UnityEngine;

namespace Combat.Ai
{
	public class StayBehindAction : IAction
	{
		public StayBehindAction(float min, float max, float deltaAngle)
		{
			_distanceMin = min;
			_distanceMax = max;
			_deltaAngle = deltaAngle;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			var ship = context.Ship;
			var enemy = context.Enemy;

			if (controls.MovementLocked)
				return;

			var currentDir = enemy.Body.Position.Direction(ship.Body.Position).normalized;
			var distance = Helpers.Distance(ship, enemy);

			if (distance <= _distanceMax && distance >= _distanceMin)
				if (Mathf.Abs(Mathf.DeltaAngle(RotationHelpers.Angle(-currentDir), ship.Body.Rotation)) < _deltaAngle)
					return;

			var direction = Vector3.Cross(currentDir,RotationHelpers.Direction(enemy.Body.Rotation)).normalized;
			var targetDir = Vector3.Cross(currentDir, direction).normalized;
			
			var targetAngle = RotationHelpers.Angle(targetDir);			
			var delta = 30*Mathf.Sin(Time.time*Mathf.PI/4);
			if (distance > _distanceMax) targetAngle -= direction.z*(45 + Mathf.Clamp01(distance/_distanceMax - 1)*45 + delta);
			if (distance < _distanceMin) targetAngle += direction.z*(45 + Mathf.Clamp01(_distanceMin/distance - 1)*45 + delta);

			controls.Course = targetAngle;
			controls.Thrust = 1;
		}
		
		private readonly float _distanceMin;
		private readonly float _distanceMax;
		private readonly float _deltaAngle;
	}
}