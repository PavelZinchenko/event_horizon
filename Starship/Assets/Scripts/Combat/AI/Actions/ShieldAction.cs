using UnityEngine;

namespace Combat.Ai
{
	public class ShieldAction : IAction
	{
		public ShieldAction(int deviceId)
		{
			_deviceId = deviceId;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
            if (context.Threats == null) return;

			var ship = context.Ship;
			
			var target = Vector2.zero;
			foreach (var threat in context.Threats.Units)
			{
				var dir = ship.Body.Position.Direction(threat.Body.Position).normalized;
				target += dir;
			}
			var course = RotationHelpers.Angle(target);
			controls.Course = course;
			if (Mathf.Abs(Mathf.DeltaAngle(course, ship.Body.Rotation)) < 90)
				controls.ActivateSystem(_deviceId);
		}
		
		private readonly int _deviceId;
	}
}
