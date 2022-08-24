using Combat.Component.Body;
using Combat.Component.Unit;
using UnityEngine;

namespace Combat.Component.Controller
{
    public class LookAtTargetController : IController
    {
        public LookAtTargetController(IUnit unit, float maxAngle, float defaultSpread)
        {
            _unit = unit;
            _maxAngle = maxAngle;
            _defaultRotation = unit.Body.Parent == null ? unit.Body.Rotation : 0;
            _defaultSpread = defaultSpread;
        }

        public void Dispose() { }

        public void UpdatePhysics(float elapsedTime)
        {
            var target = _unit.Collider.ActiveCollision;
            if (target == null)
            {
                _unit.Body.Turn(_defaultRotation + (2f*Random.value - 1f)*_defaultSpread);
                return;
            }

            var delta = RotationHelpers.Angle(_unit.Body.WorldPositionNoOffset().Direction(target.Body.WorldPosition()));
            if (_unit.Body.Parent != null)
                _unit.Body.Turn(Mathf.Clamp(Mathf.DeltaAngle(_unit.Body.Parent.WorldRotation(), delta), -_maxAngle, _maxAngle));
            else
                _unit.Body.Turn(delta);
        }

        private readonly float _defaultRotation;
        private readonly float _defaultSpread;
        private readonly IUnit _unit;
        private readonly float _maxAngle;
    }
}
