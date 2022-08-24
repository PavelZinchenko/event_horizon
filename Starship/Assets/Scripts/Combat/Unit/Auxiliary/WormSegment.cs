using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Controls;
using Combat.Component.Engine;
using Combat.Component.Features;
using Combat.Component.Physics;
using Combat.Component.Physics.Joint;
using Combat.Component.Platform;
using Combat.Component.Ship;
using Combat.Component.Ship.Effects;
using Combat.Component.Stats;
using Combat.Component.Systems;
using Combat.Component.Triggers;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Unit;
using Constructor;
using UnityEngine;

namespace Combat.Component.Unit
{
    public class WormSegment : UnitBase, IShip
    {
        public WormSegment(IShip owner, IBody body, IView view, ICollider collider, PhysicsManager physics, float hitPoints)
            : base(new UnitType(UnitClass.Limb, owner.Type.Side, owner), body, view, collider, physics)
        {
            _hitPoints = hitPoints;
            _state = UnitState.Active;
            _features = new Features.Features(TargetPriority.High, Color.white);
            body.SetVelocityLimit(30f); // TODO
        }

        public void Connect(IUnit parent, float parentOffset, float offset, float maxAngle)
        {
            Body.Move(parent.Body.Position - RotationHelpers.Direction(parent.Body.Rotation) * parent.Body.Scale);
            Body.Turn(parent.Body.Rotation);

            _parent = parent;
            _offset = offset;
            _maxAngle = maxAngle;
            _parentOffset = parentOffset;
            _joint = parent.Physics.CreateHingeJoint(Physics, parentOffset, offset, -maxAngle, maxAngle);
        }

        public bool Enabled { get { return _isActive; } }

        public void SetDamageIndicator(IDamageIndicator indicator, bool isOwner)
        {
            _damageIndicator = indicator;
            _ownDamageIndicator = isOwner;

            if (isOwner)
                AddResource(indicator);
        }

        public override ICollisionBehaviour CollisionBehaviour { get { return null; } }

        public override UnitState State { get { return _state; } }

        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData)
        {
            Affect(impact);
        }

        protected override void OnUpdateView(float elapsedTime)
        {
            View.Life = _isActive ? 1.0f : 0.0f;
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Type.Owner.State == UnitState.Inactive)
            {
                Vanish();
                return;
            }
            if (Type.Owner.State == UnitState.Destroyed)
            {
                Destroy();
                return;
            }

            if (!EnsurePositionValid())
                return;

            if (_ownDamageIndicator)
                _damageIndicator.Update(elapsedTime);

            var velocity = Body.Velocity;
            var direction = RotationHelpers.Direction(Body.Rotation);
            var sideDirection = new Vector2(direction.y, -direction.x);
            var sideVelocity = Vector2.Dot(velocity, sideDirection);
            Body.ApplyAcceleration(-5f*sideVelocity*elapsedTime*sideDirection);
        }

        private bool EnsurePositionValid()
        {
            if (_parent.Body.Position.SqrDistance(Body.Position) < Body.Scale*5 && Mathf.Abs(Mathf.DeltaAngle(_parent.Body.Rotation, Body.Rotation)) < 90f)
                return true;

            _joint.Dispose();
            Connect(_parent, _parentOffset, _offset, _maxAngle);

            Body.ApplyAcceleration(_parent.Body.Velocity - Body.Velocity);
            Body.ApplyAngularAcceleration(-Body.AngularVelocity);

            return false;
        }

        protected override void OnDispose()
        {
            _features.Dispose();
        }

        public IControls Controls { get; set; }
        public IStats Stats { get { return null; } }
        public IEngine Engine { get { return null; } }
        public IFeatures Features { get { return _features; } }
        public IShipSystems Systems { get { return null; } }
        public IShipEffects Effects { get { return null; } }
        public IShipSpecification Specification { get { return null; } }

        public void AddPlatform(IWeaponPlatform platform) { }
        public void AddSystem(ISystem system) { }
        public void AddEffect(IShipEffect shipEffect) { }

        public void Affect(Impact impact)
        {
            if (!_isActive)
                return;

            impact.ApplyImpulse(Body);
            var damage = impact.GetTotalDamage(Resistance.Empty);

            if (_damageIndicator != null)
                _damageIndicator.ApplyDamage(new Impact { ShieldDamage = damage });

            _hitPoints -= damage;
            if (_hitPoints < 0)
                impact.Effects |= CollisionEffect.Destroy;

            if (impact.Effects.Contains(CollisionEffect.Destroy))
                Blow();
        }

        public override void Vanish()
        {
            _state = UnitState.Inactive;
        }

        private void Destroy()
        {
            _state = UnitState.Destroyed;
            InvokeTriggers(ConditionType.OnDestroy);
        }

        private void Blow()
        {
            _isActive = false;
            Collider.Enabled = false;
            InvokeTriggers(ConditionType.OnDestroy);
        }

        private IDamageIndicator _damageIndicator;
        private bool _ownDamageIndicator;

        private bool _isActive = true;
        private IJoint _joint;
        private UnitState _state;
        private float _hitPoints;
        private float _maxAngle;
        private float _offset;
        private float _parentOffset;
        private IUnit _parent;
        private readonly IFeatures _features;
    }
}
