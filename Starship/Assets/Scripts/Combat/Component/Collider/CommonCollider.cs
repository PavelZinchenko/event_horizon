using System.Collections.Generic;
using System.Linq;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using UnityEngine;
using Zenject;

namespace Combat.Component.Collider
{
    public class CommonCollider : MonoBehaviour, ICollider
    {
        [Inject] private readonly ICollisionManager _collisionManager;

        [SerializeField] private bool _isStatic;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled == value)
                    return;

                _enabled = value;

                foreach (var item in AllColliders)
                    if (item)
                        item.enabled = value;
            }
        }

        public IUnit Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                _unitSide = _unit != null ? _unit.Type.Side : UnitSide.Undefined;

                if (this)
                    gameObject.layer = _unit != null ? (int)_unit.Type.CollisionLayer : (int)Layer.Default;
            }
        }

        public float MaxRange { get; set; }

        //public bool IsTrigger
        //{
        //    get
        //    {
        //        return AllColliders.All(item => item.isTrigger);
        //    }
        //    set
        //    {
        //        foreach (var item in AllColliders)
        //            item.isTrigger = value;
        //    }
        //}

        public IUnit ActiveCollision { get { return _activeCollision ?? (_activeCollision = _activeCollisions.FirstOrDefault().Key); } }
        public Vector2 LastContactPoint { get; private set; }

        public void UpdatePhysics(float elapsedTime)
        {
            _frameId++;
            _timeInterval = elapsedTime;

            if (_unit != null && _unitSide != _unit.Type.Side && this)
            {
                _unitSide = _unit.Type.Side;
                gameObject.layer = (int)_unit.Type.CollisionLayer;
            }
        }

        public void Dispose()
        {
            Unit = null;
            _activeCollision = null;
            _recentTrigger = null;
            Enabled = true;
            _activeCollisions.Clear();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (_collisionManager == null || _unit == null || !collider)
                return;

            var other = collider.gameObject.GetComponent<ICollider>();
            if (other == null || other.Unit == null || other.Unit.Body == null)
            {
                Debug.LogError("invalid collider: " + other);
                //Debug.Break();
                return;
            }

            //if (!TryAddActiveCollision(other.Unit))
            //    return;

            var isNew = _recentTrigger != other.Unit;
            _recentTrigger = other.Unit;

            LastContactPoint = Unit.Body.WorldPosition();// + RotationHelpers.Direction(Unit.Body.WorldRotation())*Unit.Body.Scale/2;
            var hitPoint = LastContactPoint + Unit.Body.WorldVelocity().normalized*Unit.Body.Scale/2;
            _collisionManager.OnCollision(Unit, other.Unit, CollisionData.FromObjects(Unit, other.Unit, hitPoint, isNew, _timeInterval));
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (_collisionManager == null || _unit == null || !collider)
                return;

            var other = collider.gameObject.GetComponent<ICollider>();
            if (other == null || other.Unit == null || other.Unit.Body == null)
            {
                Debug.LogError("invalid collider: " + other);
                //Debug.Break();
                return;
            }

            //if (!TryUpdateActiveCollision(other.Unit))
            //    return;

            LastContactPoint = Unit.Body.WorldPosition();// + RotationHelpers.Direction(Unit.Body.WorldRotation())*Unit.Body.Scale/2;
            var hitPoint = LastContactPoint + Unit.Body.WorldVelocity().normalized * Unit.Body.Scale/2;
            _collisionManager.OnCollision(Unit, other.Unit, CollisionData.FromObjects(Unit, other.Unit, hitPoint, false, _timeInterval));
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            //var other = collider.gameObject.GetComponent<ICollider>();
            //if (other == null || other.Unit == null)
            //{
            //    UnityEngine.Debug.Log("invalid collider: " + other);
            //    //Debug.Break();
            //    return;
            //}

            //RemoveActiveCollision(collider.GetComponent<ICollider>().Unit);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_unit == null)
                return;

            var other = collision.collider.gameObject.GetComponent<ICollider>();
            if (other == null || other.Unit == null)
            {
                Debug.LogError("invalid collider: " + other);
                //Debug.Break();
                return;
            }

            if (!TryAddActiveCollision(other.Unit))
                return;

            var contacts = collision.contacts;
            if (contacts.Length > 0)
                LastContactPoint = contacts[0].point;
            else
                LastContactPoint = _unit.Body.WorldPosition();

            _collisionManager.OnCollision(Unit, other.Unit, CollisionData.FromCollision(LastContactPoint, collision.relativeVelocity, true, _timeInterval));
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var other = collision.collider.gameObject.GetComponent<ICollider>();
            if (other == null || other.Unit == null)
            {
                Debug.LogError("invalid collider: " + other);
                return;
            }

            RemoveActiveCollision(collision.gameObject.GetComponent<ICollider>().Unit);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_unit == null)
                return;

            var other = collision.collider.gameObject.GetComponent<ICollider>();
            if (other == null || other.Unit == null)
            {
                Debug.LogError("invalid collider: " + other);
                //Debug.Break();
                return;
            }

            if (!TryUpdateActiveCollision(other.Unit))
                return;

            if (_activeCollisionFrameCount*Time.fixedDeltaTime > 1.0f)
            {
                var weight = _unit.Body.TotalWeight();
                var otherWeight = other.Unit.Body.TotalWeight();
                if (!_isStatic)
                {
                    var body = _unit.Body.Owner();
                    var position = body.Position;
                    var otherPosition = (Vector2)collision.transform.position;
                    var direction = (otherPosition - position).normalized;
                    body.Move(position - direction*(_activeCollisionFrameCount*0.05f*Time.fixedDeltaTime*otherWeight/(0.00001f + weight + otherWeight)));
                }
            }

            var contacts = collision.contacts;
            if (contacts.Length > 0)
                LastContactPoint = contacts[0].point;
            else
                LastContactPoint = _unit.Body.WorldPosition();

            _collisionManager.OnCollision(Unit, other.Unit, CollisionData.FromCollision(LastContactPoint, collision.relativeVelocity, false, _timeInterval));
        }

        private bool TryAddActiveCollision(IUnit unit)
        {
            if (unit == null || unit == _unit.Type.Owner || _activeCollisions.ContainsKey(unit))
                return false;

            _activeCollision = unit;
            _activeCollisionFrameCount = 0;
            _activeCollisions[unit] = _frameId;
            return true;
        }

        private bool TryUpdateActiveCollision(IUnit unit)
        {
            if (unit == _unit.Type.Owner)
                return false;

            int frameId;
            if (_activeCollisions.TryGetValue(unit, out frameId) && frameId == _frameId)
                return false;

            if (_activeCollision == unit)
            {
                _activeCollisionFrameCount++;
            }
            else
            {
                _activeCollisionFrameCount = 0;
                _activeCollision = unit;
            }

            _activeCollisions[unit] = _frameId;
            return true;
        }

        private void RemoveActiveCollision(IUnit unit)
        {
            if (unit == null)
                return;

            _activeCollisions.Remove(unit);
            if (unit == _activeCollision)
                _activeCollision = null;
        }

        private Collider2D[] AllColliders { get { return _cachedColliders ?? (_cachedColliders = GetComponents<Collider2D>()); } }

        //private readonly ContactPoint2D[] _contacts = new ContactPoint2D[1];
        private IUnit _recentTrigger;
        private float _timeInterval;
        private int _frameId;
        private int _activeCollisionFrameCount;
        private bool _enabled = true;
        private IUnit _unit;
        private IUnit _activeCollision;
        private UnitSide _unitSide = UnitSide.Undefined;
        private Collider2D[] _cachedColliders;

        private readonly Dictionary<IUnit, int> _activeCollisions = new Dictionary<IUnit, int>();
    }
}
