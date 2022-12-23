using System;
using UnityEngine;

namespace Combat.Component.Body
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RigidBodyAdapter : MonoBehaviour, IBodyComponent
    {
        public void Initialize(IBody parent, Vector2 position, float rotation, float scale, Vector2 velocity,
            float angularVelocity, float weight, bool frozen = false)
        {
            _transformCache = transform;
            _rigidbody = GetComponent<Rigidbody2D>();
            if (parent != null)
                parent.AddChild(_transformCache);
            else
            {
                if ((object) _transformCache.parent != null) _transformCache.parent = null;
            }

            Parent = parent;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Weight = weight;
            _frozen = frozen;

            if (_rigidbody.bodyType != RigidbodyType2D.Static)
            {
                Velocity = velocity;
                AngularVelocity = angularVelocity;
            }
        }

        public IBody Parent
        {
            get => _parent;
            private set
            {
                if (_parent == value)
                    return;

                _rigidbody.isKinematic = value != null;
                _parent = value;
            }
        }

        public Vector2 Position
        {
            get => _cachedPosition;
            set
            {
                _cachedPosition = value;
                // We assume (dangerous) that we are always disposed upon destruction, and so cache was deleted
                if (ReferenceEquals(_transformCache, null)) return;
                var targetPos = _parent?.ChildPosition(value) ?? value;
                // Frozen objects are the only one who are realistically gonna do a lot of calls with unchanging value
                if (!_frozen || _transformCache.localPosition.x != targetPos.x ||
                    _transformCache.localPosition.y != targetPos.y)
                {
                    _transformCache.localPosition = new Vector3(targetPos.x, targetPos.y, _cachedZ);
                }
            }
        }

        public float Rotation
        {
            get => _cachedRotation;
            set
            {
                _cachedRotation = value;
                // We assume (dangerous) that we are always disposed upon destruction, and so cache was deleted
                if (ReferenceEquals(_transformCache, null)) return;
                var targetAngle = Mathf.Repeat(value, 360);
                // Frozen objects are the only one who are realistically gonna do a lot of calls with unchanging value
                if (!_frozen || _transformCache.localRotation.z != targetAngle)
                {
                    _transformCache.rotation = Quaternion.Euler(0, 0, targetAngle);
                }
            }
        }

        public float Offset { get; set; }

        public Vector2 Velocity
        {
            get => Parent == null ? _cachedVelocity : Vector2.zero;
            set
            {
                if (Parent == null)
                {
                    _cachedVelocity = value;
                    _rigidbody.velocity = value;
                }
            }
        }

        public float AngularVelocity
        {
            get => Parent == null ? _rigidbody.angularVelocity : 0f;
            set
            {
                if (Parent == null)
                    _rigidbody.angularVelocity = value;
            }
        }

        public float Weight
        {
            get => _rigidbody.mass;
            set => _rigidbody.mass = value;
        }

        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                if (!ReferenceEquals(_transformCache, null))
                {
                    _transformCache.localScale =
                        Parent == null ? new Vector3(value, value, value) : Vector3.one * (Parent.WorldScale() * value);
                }
            }
        }

        public void ApplyAcceleration(Vector2 acceleration)
        {
            if (Parent == null)
                _rigidbody.AddForce(acceleration * _rigidbody.mass, ForceMode2D.Impulse);
        }

        public void ApplyAngularAcceleration(float acceleration)
        {
            if (Parent == null)
                _rigidbody.AddTorque(acceleration * Mathf.Deg2Rad * _rigidbody.inertia, ForceMode2D.Impulse);
        }

        public void ApplyForce(Vector2 position, Vector2 force)
        {
            if (Parent == null)
                _rigidbody.AddForceAtPosition(force, position, ForceMode2D.Impulse);
        }

        public void SetVelocityLimit(float value)
        {
            _maxVelocity = value;
        }

        public void SetAngularVelocityLimit(float value)
        {
            _maxAngularVelocity = value;
        }

        public void Move(Vector2 position)
        {
            Position = position;
        }

        public void Turn(float rotation)
        {
            Rotation = rotation;
        }

        public void SetSize(float size)
        {
            Scale = size;
        }

        public void Dispose()
        {
            _transformCache = null;
        }

        public void UpdatePhysics(float elapsedTime)
        {
            if (_frozen)
            {
                Position = _cachedPosition;
                Rotation = _cachedRotation;
            }

            if(_maxVelocity > 0)
            {
                var velocity = _rigidbody.velocity;
                if (velocity.sqrMagnitude > _maxVelocity * _maxVelocity)
                {
                    velocity = velocity.normalized * _maxVelocity;
                    _rigidbody.velocity = velocity;
                }
                _cachedVelocity = velocity;
            }

            if(_maxAngularVelocity > 0)
            {
                var angularVelocity = _rigidbody.angularVelocity;
                if (Math.Abs(angularVelocity) > _maxAngularVelocity)
                {
                    _rigidbody.angularVelocity = _maxAngularVelocity * Mathf.Sign(angularVelocity);
                }
            }
        }

        public void UpdateView(float elapsedTime)
        {
            if (_frozen) return;
            if (ReferenceEquals(_transformCache, null))
            {
                _transformCache = transform;
            }

            var pos = _transformCache.localPosition;
            _cachedPosition = pos;
            _cachedZ = pos.z;
            _cachedRotation = _transformCache.localEulerAngles.z;
        }

        public void AddChild(Transform child)
        {
            child.parent = _transformCache;
        }

        //private void Update()
        //{
        //    if (Parent == null)
        //        return;

        //    transform.localPosition = this.WorldPosition();
        //    transform.localEulerAngles = new Vector3(0,0,this.WorldRotation());
        //}

        private Rigidbody2D _rigidbody;
        private Transform _transformCache;
        private Vector2 _cachedPosition;
        private float _cachedZ;
        private float _cachedRotation;
        private Vector2 _cachedVelocity;
        private float _scale;
        private IBody _parent;
        private float _maxVelocity;
        private float _maxAngularVelocity;
        private bool _frozen;
    }
}
