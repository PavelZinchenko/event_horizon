using Combat.Component.Unit;
using UnityEngine;

namespace Combat.Component.Controller
{
    public class RocketController : IController
    {
        public RocketController(IUnit unit, float maxVelocity, float acceleration)
        {
            _unit = unit;
            _maxVelocity = maxVelocity;
            _acceleration = acceleration;
        }

        public void Dispose() { }

        public void UpdatePhysics(float elapsedTime)
        {
            if (_unit.Body.Parent != null)
                return;

            UpdateVelocity(elapsedTime);
        }

        private void UpdateVelocity(float deltaTime)
        {
            var forward = RotationHelpers.Direction(_unit.Body.Rotation);
            var velocity = _unit.Body.Velocity;
            var forwardVelocity = Vector2.Dot(velocity, forward);

            var requiredVelocity = Mathf.Max(forwardVelocity, _maxVelocity) * forward;
            var dir = (requiredVelocity - velocity).normalized;

            _unit.Body.ApplyAcceleration(dir * _acceleration * deltaTime);
        }

        private readonly IUnit _unit;
        private readonly float _maxVelocity;
        private readonly float _acceleration;
    }
}
