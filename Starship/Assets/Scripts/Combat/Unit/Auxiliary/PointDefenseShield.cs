using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Unit;
using Combat.Unit.Auxiliary;

namespace Combat.Component.Unit
{
    public class PointDefenseShield : UnitBase, IAuxiliaryUnit
    {
        public PointDefenseShield(IShip parent, IBody body, IView view, ICollider collider, ICollisionBehaviour collisionBehaviour, float energyConsumption, float cooldown)
            : base(new UnitType(UnitClass.Drone, parent.Type.Side, parent), body, view, collider, null)
        {
            _parentShip = parent;
            _energyConsumption = energyConsumption;
            _collisionBehaviour = collisionBehaviour;
            _cooldown = cooldown;
        }

        public override ICollisionBehaviour CollisionBehaviour { get { return _collisionBehaviour; } }

        public override UnitState State { get { return _state; } }

        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData)
        {
            if (impact.Effects.Contains(CollisionEffect.Activate))
            {
                InvokeTriggers(ConditionType.OnActivate);
                UpdateCollider(false);
                _timeFromLastUse = 0f;

                if (!_parentShip.Stats.Energy.TryGet(_energyConsumption))
                    Enabled = false;
            }
        }

        public bool Active { get; set; }

        public bool Enabled { get; set; }

        public override void Vanish()
        {
            _state = UnitState.Inactive;
        }

        public void Destroy()
        {
            _state = UnitState.Destroyed;
        }

        protected override void OnUpdateView(float elapsedTime) {}

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            _timeFromLastUse += elapsedTime;
            UpdateCollider(Enabled && _timeFromLastUse > _cooldown);
        }

        protected override void OnDispose() {}

        private void UpdateCollider(bool enabled)
        {
            Collider.Enabled = enabled;
        }

        private float _timeFromLastUse;
        private UnitState _state = UnitState.Active;
        private readonly float _energyConsumption;
        private readonly IShip _parentShip;
        private readonly ICollisionBehaviour _collisionBehaviour;
        private readonly float _cooldown;
    }
}
