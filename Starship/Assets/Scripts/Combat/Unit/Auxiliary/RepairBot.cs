using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Controller;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Unit;
using Combat.Unit.Auxiliary;

namespace Combat.Component.Unit
{
    public class RepairBot : UnitBase, IAuxiliaryUnit
    {
        public RepairBot(IShip parent, IBody body, IView view, ICollider collider, float hitPoints)
            : base(new UnitType(UnitClass.Drone, parent.Type.Side, parent), body, view, collider, null)
        {
            _hitPoints = hitPoints;
        }

        public override ICollisionBehaviour CollisionBehaviour { get { return null; } }

        public override UnitState State { get { return _state; } }

        public IController Controller { get; set; }

        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData)
        {
            impact.ApplyImpulse(Body);

            var damage = impact.GetTotalDamage(Resistance.Empty);
            if (impact.Effects.Contains(CollisionEffect.Destroy) || damage > _hitPoints)
                Destroy();
        }

        public bool Active
        {
            get { return _isActive; }
            set
            {
                if (_isActive == value)
                    return;

                _isActive = value;
                InvokeTriggers(_isActive ? ConditionType.OnActivate : ConditionType.OnDeactivate);
            }
        }

        public bool Enabled { get; set; }

        public override void Vanish()
        {
            _state = UnitState.Inactive;
        }

        public void Destroy()
        {
            InvokeTriggers(ConditionType.OnDestroy);
            _state = UnitState.Destroyed;
        }

        protected override void OnUpdateView(float elapsedTime)
        {
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

        private bool _isActive;
        private UnitState _state = UnitState.Active;
        private readonly float _hitPoints;
    }
}
