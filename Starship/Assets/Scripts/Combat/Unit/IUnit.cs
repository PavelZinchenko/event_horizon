using System;
using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Physics;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Unit;

namespace Combat.Component.Unit
{
    public interface IUnit : IDisposable
    {
        UnitType Type { get; }
        IBody Body { get; }
        IView View { get; }
        ICollider Collider { get; }
        ICollisionBehaviour CollisionBehaviour { get; }

        float DefenseMultiplier { get; }

        PhysicsManager Physics { get; }

        UnitState State { get; }

        void OnCollision(Impact impact, IUnit target, CollisionData collisionData);
        void UpdatePhysics(float elapsedTime);
        void UpdateView(float elapsedTime);

        void Vanish();
    }
}
