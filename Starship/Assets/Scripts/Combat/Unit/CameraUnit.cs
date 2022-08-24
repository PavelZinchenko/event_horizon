using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using UnityEngine;

namespace Combat.Unit
{
    public class CameraUnit : UnitBase
    {
        public CameraUnit(IBody body)
            : base(new UnitType(UnitClass.Camera, UnitSide.Neutral, null), body, null, null, null)
        {
            _initialPosition = body.Position;
            _initialSize = body.Scale;
        }

        public override ICollisionBehaviour CollisionBehaviour { get { return null; } }
        public override UnitState State { get { return UnitState.Active; } }
        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData) {}
        public override void Vanish() {}

        protected override void OnUpdateView(float elapsedTime) {}

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            _time += elapsedTime;

            Body.Move(_initialPosition + RotationHelpers.Direction(_time*5)*_initialSize*0.5f);
            Body.SetSize(_initialSize + Mathf.Sin(_time/100f)*_initialSize*0.3f);
        }

        protected override void OnDispose() {}

        private float _time;
        private readonly float _initialSize;
        private readonly Vector2 _initialPosition;
    }
}
