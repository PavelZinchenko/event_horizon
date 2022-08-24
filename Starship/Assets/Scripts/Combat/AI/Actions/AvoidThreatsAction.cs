using UnityEngine;

namespace Combat.Ai
{
	public class AvoidThreatsAction : IAction
	{
		public void Perform(Context context, ref ShipControls controls)
		{
		    if (context.Threats == null) return;

			var ship = context.Ship;

			var target = Vector2.zero;
			var sqrMaxVelocity = context.Ship.Engine.MaxVelocity*3; sqrMaxVelocity *= sqrMaxVelocity;
			int count = 0;

			foreach (var threat in context.Threats.Units)
			{
				if (threat.Body.Velocity.sqrMagnitude > sqrMaxVelocity)
					continue;

				var dir = ship.Body.Position.Direction(threat.Body.Position).normalized;
                target += dir;
				count++;
            }

			if (count == 0)
				return;
            
			var delta = 20*Mathf.Clamp01(Vector2.Dot(ship.Body.Velocity, RotationHelpers.Direction(ship.Body.Rotation)) / ship.Engine.MaxVelocity);
			if (Mathf.DeltaAngle(RotationHelpers.Angle(target), ship.Body.Rotation) < 0)
				delta = -delta;

			controls.Thrust = 1;
			controls.Course = ship.Body.Rotation + delta;
        }
	}
}
