using UnityEngine;

namespace Combat.Ai.Condition
{
	public class CanCatch : ICondition
	{
		public bool IsTrue(Context context)
		{
			var ship = context.Ship;
			var enemy = context.Enemy;
			var direction = ship.Body.Position.Direction(enemy.Body.Position).normalized;
			return Vector2.Dot(enemy.Body.Velocity, direction) < ship.Engine.MaxVelocity*0.9f;
		}
	}

}
