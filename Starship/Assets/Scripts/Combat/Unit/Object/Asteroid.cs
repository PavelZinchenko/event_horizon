using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Physics;
using Combat.Component.Triggers;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.View;

namespace Combat.Unit.Object
{
    public class Asteroid : UnitBase
    {
        public Asteroid(UnitType type, IBody body, IView view, ICollider collider, PhysicsManager physics, float hitPoints, float damageMultiplier) 
            : base(type, body, view, collider, physics)
        {
            _hitPoints = hitPoints;
            AddResource(_collisionBehaviour = new DefaultCollisionBehaviour(damageMultiplier));
            AddResource(_noDamageCollisionBehaviour = new DefaultCollisionBehaviour(0));
            CombatMode = true;
        }

        public bool CombatMode { get; set; }

        public override ICollisionBehaviour CollisionBehaviour { get { return CombatMode ? _collisionBehaviour : _noDamageCollisionBehaviour; } }

        public override UnitState State { get { return _state; } }

        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData)
        {
            impact.ApplyImpulse(Body);
            _hitPoints -= impact.GetTotalDamage(Resistance.Empty);
            if (_hitPoints < 0 || impact.Effects.Contains(CollisionEffect.Destroy))
                Destroy();
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

        private void Destroy()
        {
            InvokeTriggers(ConditionType.OnDestroy);
            _state = UnitState.Destroyed;
        }

        private UnitState _state = UnitState.Active;
        private float _hitPoints;
        private readonly ICollisionBehaviour _collisionBehaviour;
        private readonly ICollisionBehaviour _noDamageCollisionBehaviour;
    }
}
