using Combat.Component.Body;
using Combat.Component.Bullet;
using Combat.Component.Physics.Joint;
using Combat.Component.Unit;
using Combat.Unit;
using UnityEngine;

namespace Combat.Component.Controller
{
    public class TractorBeamController : IController
    {
        public TractorBeamController(IBullet bullet, float maxDistance)
        {
            _bullet = bullet;
            _maxDistance = maxDistance;
            _defaultRotation = bullet.Body.Parent == null ? bullet.Body.Rotation : 0;
        }

        public void Dispose()
        {
            if (_joint != null)
                _joint.Dispose();
            _joint = null;
        }

        public void UpdatePhysics(float elapsedTime)
        {
            if (!_target.IsActive())
            {
                _target = _bullet.Collider.ActiveCollision;

                if (_joint != null)
                {
                    _joint.Dispose();
                    _joint = null;
                }

                _joint = CreateJoint();
            }

            if (!_target.IsActive() || _joint == null || !_joint.IsActive)
            {
                _bullet.Body.Turn(_defaultRotation);
                return;
            }

            var delta = RotationHelpers.Angle(_bullet.Body.WorldPosition().Direction(_target.Body.WorldPosition()));
            if (_bullet.Body.Parent != null)
                _bullet.Body.Turn(Mathf.DeltaAngle(_bullet.Body.Parent.WorldRotation(), delta));
            else
                _bullet.Body.Turn(delta);
        }

        private IJoint CreateJoint()
        {
            if (!_target.IsActive())
                return null;

            var owner = _bullet.Type.Owner;
            if (!owner.IsActive())
                return null;

            return owner.Physics.CreateDistanceJoint(_target.Physics, _maxDistance);
        }

        private IJoint _joint;
        private IUnit _target;
        private readonly IBullet _bullet;
        private readonly float _maxDistance;
        private readonly float _defaultRotation;
    }
}
