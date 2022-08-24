using Combat.Component.Unit;
using UnityEngine;

namespace Combat.Ai
{
	public class AvoidObjectAction : IAction
	{
		public AvoidObjectAction(IUnit unit, float distance)
		{
			_unit = unit;
			_distance = distance;
		}

		public void Perform(Context context, ref ShipControls controls)
		{
			var ship = context.Ship;			
			var direction = ship.Body.Position.Direction(_unit.Body.Position);

			if (direction.magnitude > _distance + _unit.Body.Scale/2 + ship.Body.Scale/2)
				return;

			var directionAngle = RotationHelpers.Angle(direction);
			var angle = Mathf.DeltaAngle(directionAngle, ship.Body.Rotation);
			if (angle > 90 || angle < -90)
			{
				controls.Thrust = 1;
				if (angle > 130 || angle < -130)
					controls.Course = angle < 0 ? directionAngle - 130 : directionAngle + 130;
			}
			else
			{
				controls.Course = angle < 0 ? directionAngle - 90 : directionAngle + 90;
			}
		}

		private readonly IUnit _unit;
		private readonly float _distance;
	}
}