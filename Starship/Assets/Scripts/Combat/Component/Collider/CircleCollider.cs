using System.Collections.Generic;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Unit;
using UnityEngine;
using Zenject;

namespace Combat.Component.Collider
{
    public class CircleCollider : MonoBehaviour, ICollider
    {
        [Inject]
        private readonly ICollisionManager _collisionManager;

        public bool Enabled { get { return _enabled; } set { _enabled = value; } }

        public IUnit Unit { get; set; }

        public float MaxRange { get; set; }

        public IUnit ActiveCollision { get; private set; }
        public Vector2 LastContactPoint { get; private set; }

        public void UpdatePhysics(float elapsedTime)
        {
            if (Unit == null || MaxRange <= 0 || !Enabled)
            {
                _activeCollisions.Clear();
                ActiveCollision = null;
                return;
            }

            Generic.Swap(ref _lastActiveCollisions, ref _activeCollisions);
            _activeCollisions.Clear();

            var position = Unit.Body.WorldPosition();
            var count = Physics2D.OverlapCircleNonAlloc(position, MaxRange, _buffer, Unit.Type.CollisionMask);
            if (count > _buffer.Length)
                count = _buffer.Length;

            for (var i = 0; i < count; ++i)
            {
                var collider = _buffer[i];
                var other = collider.GetComponent<ICollider>();

                var target = other.Unit;
                ActiveCollision = target;
                LastContactPoint = target.Body.WorldPosition();
                _activeCollisions.Add(target);

                _collisionManager.OnCollision(Unit, target, CollisionData.FromObjects(Unit, target, LastContactPoint, !_lastActiveCollisions.Contains(target), elapsedTime));
            }
        }

        public void Dispose()
        {
            Unit = null;
            ActiveCollision = null;
            _activeCollisions.Clear();
            MaxRange = 0;
            _enabled = true;
        }

        private bool _enabled = true;
        private readonly Collider2D[] _buffer = new Collider2D[32];
        private HashSet<IUnit> _activeCollisions = new HashSet<IUnit>();
        private HashSet<IUnit> _lastActiveCollisions = new HashSet<IUnit>();
    }
}
