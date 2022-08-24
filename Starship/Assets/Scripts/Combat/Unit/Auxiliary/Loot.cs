using System.Runtime.InteropServices;
using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Triggers;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Scene;
using Combat.Unit;
using UnityEngine;

namespace Combat.Component.Unit
{
    public class Loot : UnitBase
    {
        public Loot(IBody body, IView view, ICollider collider, float lifetime, IScene scene, float magnetRadius)
            : base(new UnitType(UnitClass.Loot, UnitSide.Neutral, null), body, view, collider, null)
        {
            _lifetime = lifetime;
            _scene = scene;
            _magnetRadius = magnetRadius;
            _collisionBehaviour = new LootCollisionBehaviour();
        }

        public override ICollisionBehaviour CollisionBehaviour { get { return _collisionBehaviour; } }

        public override UnitState State
        {
            get { return _elapsedTime < _lifetime ? UnitState.Active : UnitState.Destroyed; }
        }

        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData)
        {
            if (impact.Effects.Contains(CollisionEffect.Activate))
            {
                InvokeTriggers(ConditionType.OnActivate);
                _elapsedTime = _lifetime;
            }
            else
            {
                Affect(impact);
            }
        }

        protected override void OnUpdateView(float elapsedTime)
        {
            View.Life = Mathf.Clamp01(1f - _elapsedTime / _lifetime);
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            _elapsedTime += elapsedTime;
            if (_elapsedTime >= _lifetime)
                Destroy();

            var player = _scene.PlayerShip;
            if (!player.IsActive())
                return;

            var direction = player.Body.Position - Body.Position;

            if (direction.magnitude < _magnetRadius + player.Body.Scale / 2 + Body.Scale / 2)
                Body.ApplyAcceleration((direction.normalized*10 + player.Body.Velocity - Body.Velocity)*elapsedTime*10);
            else
                Body.ApplyAcceleration(-Body.Velocity*elapsedTime);

            Body.ApplyAngularAcceleration(-Body.AngularVelocity * 0.1f * elapsedTime);
        }

        protected override void OnDispose() { }

        public void Affect(Impact impact)
        {
            impact.ApplyImpulse(Body);

            if (impact.Effects.Contains(CollisionEffect.Destroy))
                Destroy();
        }
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
        private readonly float _lifetime;
        private readonly ICollisionBehaviour _collisionBehaviour;
        private readonly IScene _scene;
        private readonly float _magnetRadius;
    }
}
