using UnityEngine;

namespace Combat.Ai
{
	public class RammingAction : IAction
	{
		public RammingAction(State<bool> activeState, int fortificationDeviceId = -1, int accelerationDeviceId = -1)
		{
			_fortificationDeviceId = fortificationDeviceId;
			_accelerationDeviceId = accelerationDeviceId;
			_activeState = activeState;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			var ship = context.Ship;
			var enemy = context.Enemy;
			if (AttackHelpers.CantDetectTarget(ship, enemy)) return;

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
				_activeState.Value = false;
				return;
			}

			_activeState.Value = true;
			
			var direction = ship.Body.Position.Direction(target);
			var course = RotationHelpers.Angle(direction);
			controls.Course = course;
			
			if (Mathf.Abs(Mathf.DeltaAngle(ship.Body.Rotation, course)) < 30)
			{
				controls.Thrust = 1.0f;
				if (_accelerationDeviceId >= 0)
					controls.ActivateSystem(_accelerationDeviceId);
			}

			if (timeInterval < 1f && _fortificationDeviceId >= 0)
				controls.ActivateSystem(_fortificationDeviceId);
		}
		
		private readonly int _accelerationDeviceId;
		private readonly int _fortificationDeviceId;
		private readonly State<bool> _activeState;
	}
}
