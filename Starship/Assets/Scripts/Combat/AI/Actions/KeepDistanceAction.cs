using UnityEngine;

namespace Combat.Ai
{
	public class KeepDistanceAction : IAction
	{
		public KeepDistanceAction(float min, float max, int spin = 0)
		{
			_distanceMin = min;
			_distanceMax = max;
			_spin = spin;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			var ship = context.Ship;
			var enemy = context.Enemy;

			if (controls.MovementLocked)
				return;

		    var minDistance = _distanceMin + ship.Body.Scale/2 + enemy.Body.Scale/2;
		    var maxDistance = minDistance - _distanceMin + _distanceMax;

		    var direction = ship.Body.Position.Direction(enemy.Body.Position);
            var alpha = RotationHelpers.Angle(direction);

            var distance = direction.magnitude;
			if (distance >= minDistance && distance <= maxDistance)
				return;

		    var delta = 0f;
		    if (distance > maxDistance)
		    {
		        var sina = minDistance / distance;
		        delta = Mathf.Asin(sina)*Mathf.Rad2Deg;
		    }
            else if (distance < minDistance)
            {
                delta = 90f + 45f*(minDistance - distance)/minDistance;
            }
		    else
		    {
		        return;
		    }

			if (_spin < 0)
				alpha += delta;
			else if (_spin > 0)
				alpha -= delta;
			else if (Mathf.DeltaAngle(ship.Body.Rotation, alpha) < 0)
				alpha += delta;
			else 
				alpha -= delta;

			if (Vector2.Dot(ship.Body.Velocity, RotationHelpers.Direction(alpha)) < ship.Engine.MaxVelocity*0.95f)
			{
				controls.Course = alpha;
				if (Mathf.Abs(Mathf.DeltaAngle(alpha, ship.Body.Rotation)) < 30)
					controls.Thrust = 1;
			}
		}

		private readonly int _spin;
		private readonly float _distanceMin;
		private readonly float _distanceMax;
	}
}
