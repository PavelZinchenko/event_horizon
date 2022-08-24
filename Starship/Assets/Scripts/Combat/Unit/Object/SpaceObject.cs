using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Triggers;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.View;

namespace Combat.Unit.Object
{
    public class SpaceObject : UnitBase
    {
        public SpaceObject(UnitType type, IBody body, IView view, ICollider collider, ICollisionBehaviour collisionBehaviour = null)
            : base(type, body, view, collider, null)
        {
            CollisionBehaviour = collisionBehaviour;
        }

        public override ICollisionBehaviour CollisionBehaviour { get; }
        public override UnitState State => _state;

        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData)
        {
            if (impact.Effects.Contains(CollisionEffect.Activate))
                InvokeTriggers(ConditionType.OnActivate);
        }

        public override void Vanish() { _state = UnitState.Inactive; }
        protected override void OnUpdateView(float elapsedTime) {}
        protected override void OnUpdatePhysics(float elapsedTime) {}
        protected override void OnDispose() {}

        private UnitState _state = UnitState.Active;
    }
}
