using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Controls;
using Combat.Component.Engine;
using Combat.Component.Features;
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
    public class Decoy : UnitBase, IShip
    {
        public Decoy(IShip parent, IBody body, IView view, ICollider collider, float hitPoints, float lifetime)
            : base(new UnitType(UnitClass.Decoy, parent.Type.Side, parent), body, view, collider, null)
        {
            _lifetime = lifetime;
            _hitPoints = hitPoints;
            _features = new Features.Features(TargetPriority.High, Color.white);
        }

        public override ICollisionBehaviour CollisionBehaviour { get { return null; } }

        public override UnitState State
        {
            get { return _elapsedTime < _lifetime ? UnitState.Active : UnitState.Destroyed; }
        }

        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData)
        {
            Affect(impact);
        }

        protected override void OnUpdateView(float elapsedTime)
        {
            View.Life = Mathf.Clamp01(1f - _elapsedTime/_lifetime);
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            _elapsedTime += elapsedTime;
            if (_elapsedTime >= _lifetime)
            {
                Destroy();
                return;
            }

            Body.ApplyAcceleration(-Body.Velocity*elapsedTime);
            Body.ApplyAngularAcceleration(-Body.AngularVelocity * 0.1f * elapsedTime);
        }

        protected override void OnDispose() {}
        public IControls Controls { get; set; }
        public IStats Stats { get { return null; } }
        public IEngine Engine { get { return null; } }
        public IFeatures Features { get { return _features; } }
        public IShipSystems Systems { get { return null; } }
        public IShipEffects Effects { get { return null; } }
        public IShipSpecification Specification { get { return null; } }

        public void Affect(Impact impact)
        {
            impact.ApplyImpulse(Body);
            _hitPoints -= impact.GetTotalDamage(Resistance.Empty);
            if (_hitPoints < 0)
                impact.Effects |= CollisionEffect.Destroy;

            if (impact.Effects.Contains(CollisionEffect.Destroy))
                Destroy();
        }

        public void AddPlatform(IWeaponPlatform platform) {}
        public void AddSystem(ISystem system) {}
        public void AddEffect(IShipEffect shipEffect) {}

        public override void Vanish()
        {
            _elapsedTime = _lifetime;
        }

        private void Destroy()
        {
            _elapsedTime = _lifetime;
            InvokeTriggers(ConditionType.OnDestroy);
        }
    
        private float _elapsedTime;
        private float _hitPoints;
        private readonly float _lifetime;
        private readonly IFeatures _features;
    }
}
