using System;
using System.Collections.Generic;
using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Physics;
using Combat.Component.Triggers;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Unit;

namespace Combat.Component.Unit
{
    public abstract class UnitBase : IUnit
    {
        protected UnitBase(UnitType type, IBody body, IView view, ICollider collider, PhysicsManager physics)
        {
            _type = type;

            if (collider != null)
                collider.Unit = this;

            AddResource(_body = body);
            AddResource(_view = view);
            /*AddResource(*/_collider = collider/*)*/;
            AddResource(_physics = physics);
            AddResource(_triggers = new UnitTriggers());
        }

        public UnitType Type { get { return _type; } }
        public IBody Body { get { return _body; } }
        public IView View { get { return _view; } }
        public ICollider Collider { get { return _collider; } }
        public PhysicsManager Physics { get { return _physics; } }
        public abstract ICollisionBehaviour CollisionBehaviour { get; }
        public virtual float DefenseMultiplier => Type?.Owner?.DefenseMultiplier ?? 1.0f;

        public abstract UnitState State { get; }

        public abstract void OnCollision(Impact impact, IUnit target, CollisionData collisionData);

        public abstract void Vanish();

        public void AddTrigger(IUnitEffect effect)
        {
            _triggers.Add(effect);
        }

        public void AddTrigger(IUnitAction action)
        {
            _triggers.Add(action);
        }

        public void AddResource(IDisposable item)
        {
            if (item != null)
                _resources.Add(item);
        }

        public void UpdatePhysics(float elapsedTime)
        {
            if (State != UnitState.Active)
                return;

            OnUpdatePhysics(elapsedTime);

            Body.UpdatePhysics(elapsedTime);

            if (Collider != null)
                Collider.UpdatePhysics(elapsedTime);

            _triggers.UpdatePhysics(elapsedTime);
        }

        public void UpdateView(float elapsedTime)
        {
            if (State != UnitState.Active)
                return;

            OnUpdateView(elapsedTime);

            Body.UpdateView(elapsedTime);

            if (View != null)
                View.UpdateView(elapsedTime);

            _triggers.UpdateView(elapsedTime);
        }

        public void Dispose()
        {
            foreach (var item in _resources)
                item.Dispose();

            _resources.Clear();

            OnDispose();

            if (_collider != null)
                _collider.Dispose();
        }

        protected void InvokeTriggers(ConditionType condition)
        {
            _triggers.Invoke(condition);
        }

        protected abstract void OnUpdateView(float elapsedTime);
        protected abstract void OnUpdatePhysics(float elapsedTime);
        protected abstract void OnDispose();

        private readonly UnitType _type;
        private readonly IBody _body;
        private readonly IView _view;
        private readonly ICollider _collider;
        private readonly PhysicsManager _physics;

        private readonly UnitTriggers _triggers;
        private readonly List<IDisposable> _resources = new List<IDisposable>();
    }
}
