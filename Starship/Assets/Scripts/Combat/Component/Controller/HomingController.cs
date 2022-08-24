using Combat.Component.Body;
using Combat.Component.Unit;
using Combat.Scene;
using Combat.Unit;
using UnityEngine;

namespace Combat.Component.Controller
{
    public class HomingController : IController
    {
        public HomingController(IUnit unit, float maxVelocity, float maxAngularVelocity, float acceleration, float maxRange, IScene scene)
        {
            _unit = unit;
            _scene = scene;
            _maxVelocity = maxVelocity;
            _maxAngularVelocity = maxAngularVelocity;
            _acceleration = acceleration;
            _maxRange = maxRange;
        }

        public void Dispose() {}

        public void UpdatePhysics(float elapsedTime)
        {
            if (_unit.Body.Parent != null)
                return;

            _timeFromLastUpdate += elapsedTime;

            if (!_target.IsActive() || _timeFromLastUpdate > _targetUpdateCooldown)
            {
                _target = _scene.Ships.GetEnemy(_unit, 0f, _maxRange*1.3f, 15f, false, false);
                _timeFromLastUpdate = 0;
            }

            var requiredAngularVelocity = 0f;
            if (_target.IsActive())
            {
                var direction = _unit.Body.WorldPosition().Direction(_target.Body.WorldPosition());
                var target = RotationHelpers.Angle(direction);
                var rotation = _unit.Body.WorldRotation();
                var delta = Mathf.DeltaAngle(rotation, target);
                requiredAngularVelocity = delta > 5 ? _maxAngularVelocity : delta < -5 ? -_maxAngularVelocity : 0f;
            }
            _unit.Body.ApplyAngularAcceleration(requiredAngularVelocity - _unit.Body.AngularVelocity);

            UpdateVelocity(elapsedTime);
        }

        private void UpdateVelocity(float deltaTime)
        {
            var forward = RotationHelpers.Direction(_unit.Body.Rotation);
            var velocity = _unit.Body.Velocity;
            var forwardVelocity = Vector2.Dot(velocity, forward);
            if (forwardVelocity >= _maxVelocity)
                return;

            var requiredVelocity = Mathf.Max(forwardVelocity, _maxVelocity) * forward;
            var dir = (requiredVelocity - velocity).normalized;

            _unit.Body.ApplyAcceleration(dir * _acceleration * deltaTime);
        }

        private float _timeFromLastUpdate;
        private IUnit _target;
        private readonly IUnit _unit;
        private readonly IScene _scene;
        private readonly float _maxVelocity;
        private readonly float _maxAngularVelocity;
        private readonly float _acceleration;
        private readonly float _maxRange;
        private const float _targetUpdateCooldown = 1.0f;
    }
}
