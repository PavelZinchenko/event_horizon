using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Controller;
using Combat.Component.Triggers;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Unit;

namespace Combat.Component.Satellite
{
    public class Satellite : UnitBase, ISatellite
    {
        public Satellite(UnitType type, IBody body, IView view, ICollider collider) 
            : base(type, body, view, collider, null)
        {
        }

        public IController Controller { get; set; }

        public override ICollisionBehaviour CollisionBehaviour { get { return null; } }

        public override UnitState State { get { return _state; } }

        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData) {}

        public void Destroy()
        {
            InvokeTriggers(ConditionType.OnDestroy);
            _state = UnitState.Destroyed;
        }

        public override void Vanish()
        {
            _state = UnitState.Inactive;
        }

        protected override void OnUpdateView(float elapsedTime)
        {
            if (Type.Owner != null)
                View.Color = Type.Owner.Features.Color;
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Controller != null)
                Controller.UpdatePhysics(elapsedTime);
        }

        protected override void OnDispose()
        {
            if (Controller != null)
                Controller.Dispose();
        }

        private UnitState _state = UnitState.Active;
    }
}
