using UnityEngine;

namespace Combat.Ai
{
	public class BoostAction : IAction
	{
		public BoostAction(int deviceId)
		{
			_deviceId = deviceId;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			var ship = context.Ship;
			if (controls.Thrust < 0.9f)
				return;
			if (Vector2.Dot(ship.Body.Velocity, RotationHelpers.Direction(ship.Body.Rotation)) > 10)
				return;

			controls.ActivateSystem(_deviceId);
		}

		private readonly int _deviceId;
	}

	public class BoostWithTeleportAction : IAction
	{
		public BoostWithTeleportAction(int deviceId)
		{
			_deviceId = deviceId;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			var ship = context.Ship;
			if (controls.Thrust < 0.9f)
				return;

			var course = ship.Controls.Course;
			if (!course.HasValue || Mathf.Abs(Mathf.DeltaAngle(course.Value, ship.Body.Rotation)) < 10)
				controls.ActivateSystem(_deviceId);
		}
		
		private readonly int _deviceId;
	}
}
