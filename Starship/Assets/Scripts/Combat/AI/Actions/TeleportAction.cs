namespace Combat.Ai
{
	public class TeleportAction : IAction
	{
		public TeleportAction(int deviceId, float attackDistance, float enemyAttackDistance)
		{
			_attackRange = attackDistance;
			_deviceId = deviceId;
			_enemyAttackDistance = enemyAttackDistance;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			var ship = context.Ship;
			var enemy = context.Enemy;

			//var device = ship.Systems[_deviceId];
			//var range = device.Range;
			//var direction = ship.Position.Direction(enemy.Position);
			//if (direction.magnitude < _attackRange)
			//	return;
			//if (direction.magnitude < range && Vector2.Dot(enemy.Velocity - ship.Velocity, direction) < 0)
			//	return;
			//if (direction.magnitude > _attackRange + range && direction.magnitude > _enemyAttackDistance)
			//	return;

			//var course = RotationHelpers.Angle(direction);
			//controls.Course = course;
			
			//if (Mathf.Abs(Mathf.DeltaAngle(course, ship.Rotation)) < 10)
			//	controls.ActivateDevice(_deviceId);
		}
		
		private readonly float _attackRange;
		private readonly float _enemyAttackDistance;
		private readonly int _deviceId;
		private bool _inAttackRange;
	}
}
