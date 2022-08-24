using UnityEngine;

namespace Combat.Ai
{
    public class FlyAroundAction : IAction
    {
        public FlyAroundAction(Vector2 position, float distance, int spin = 0)
        {
            _distance = distance;
            _position = position;
            _spin = spin;
        }

        public void Perform(Context context, ref ShipControls controls)
        {
            var ship = context.Ship;

            if (controls.MovementLocked)
                return;

            var direction = ship.Body.Position.Direction(_position);
            var alpha = RotationHelpers.Angle(direction);

            var distance = direction.magnitude;
            //if (distance >= minDistance && distance <= maxDistance)
            //    return;

            var delta = 0f;
            if (distance > _distance)
            {
                var sina = _distance / distance;
                delta = Mathf.Asin(sina) * Mathf.Rad2Deg;
            }
            else
            {
                delta = 90f + 45f * (_distance - distance) / _distance;
            }

            if (_spin > 0)
                alpha += delta;
            else if (_spin < 0)
                alpha -= delta;
            else if (Mathf.DeltaAngle(ship.Body.Rotation, alpha) < 0)
                alpha += delta;
            else
                alpha -= delta;

            if (Vector2.Dot(ship.Body.Velocity, RotationHelpers.Direction(alpha)) < ship.Engine.MaxVelocity * 0.75f)
            {
                controls.Course = alpha;
                if (Mathf.Abs(Mathf.DeltaAngle(alpha, ship.Body.Rotation)) < 30)
                    controls.Thrust = 1;
            }
        }

        private readonly int _spin;
        private readonly float _distance;
        private readonly Vector2 _position;
    }
}
