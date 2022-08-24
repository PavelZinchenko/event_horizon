using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.View;

namespace Combat.Unit.Object
{
    public class StaticPlatform : UnitBase
    {
        public StaticPlatform(UnitSide side, IBody body, IView view, ICollider collider)
            : base(new UnitType(UnitClass.Platform, side, null), body, view, collider, null)
        {
            AddResource(_collisionBehaviour = new DefaultCollisionBehaviour(1));
        }

        public override ICollisionBehaviour CollisionBehaviour { get { return _collisionBehaviour; } }

        public override UnitState State { get { return _state; } }

        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData)
        {
        }

        public override void Vanish()
        {
            _state = UnitState.Inactive;
        }

        protected override void OnUpdateView(float elapsedTime)
        {
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
        }

        protected override void OnDispose()
        {
        }

        //private void Destroy()
        //{
        //    InvokeTriggers(ConditionType.OnDestroy);
        //    _state = UnitState.Destroyed;
        //}

        private UnitState _state = UnitState.Active;
        private readonly ICollisionBehaviour _collisionBehaviour;
    }
}
