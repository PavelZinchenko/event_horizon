using UnityEngine;

namespace Combat.Component.Body
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RigidBodyAdapter : MonoBehaviour, IBodyComponent
    {
        public void Initialize(IBody parent, Vector2 position, float rotation, float scale, Vector2 velocity, float angularVelocity, float weight)
        {
            if (parent != null)
                parent.AddChild(transform);
            else
                transform.parent = null;

            Parent = parent;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Weight = weight;

            if (_rigidbody.bodyType != RigidbodyType2D.Static)
            {
                Velocity = velocity;
                AngularVelocity = angularVelocity;
            }
        }

        public IBody Parent
        {
            get { return _parent; }
            private set
            {
                if (_parent == value)
                    return;

                GetComponent<Rigidbody2D>().isKinematic = value != null;
                _parent = value;
            }
        }

        public Vector2 Position
        {
			get { return _cachedPosition; }
            set
            {
                _cachedPosition = value;
                if (this && transform)
                    gameObject.Move(_parent == null ? value : _parent.ChildPosition(value));
            }
        }

        public float Rotation
        {
			get { return _cachedRotation; }
            set
            {
                _cachedRotation = value;
                if (this && transform)
                    transform.localEulerAngles = new Vector3(0, 0, Mathf.Repeat(value, 360));
            }
        }
        
        public float Offset { get; set; }

        public Vector2 Velocity
        {
            get { return Parent == null ? _cachedVelocity : Vector2.zero; }
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
            get { return Parent == null ? _rigidbody.angularVelocity : 0f; }
            set
            {
                if (Parent == null && _rigidbody)
                    _rigidbody.angularVelocity = value; 
            }
        }

        public float Weight
        {
            get { return _rigidbody.mass; }
            set { _rigidbody.mass = value; }
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                if (transform)
                    transform.localScale = Parent == null ? Vector3.one * value : Vector3.one * Parent.WorldScale() * value;
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

        public void Dispose() { }

        public void UpdatePhysics(float elapsedTime)
        {
            var velocity = _rigidbody.velocity;
            if (_maxVelocity > 0 && velocity.magnitude > _maxVelocity)
            {
                velocity = velocity.normalized * _maxVelocity;
                _rigidbody.velocity = velocity;
            }

            _cachedVelocity = velocity;
        }

        public void UpdateView(float elapsedTime)
        {
            _cachedPosition = transform.localPosition;
            _cachedRotation = transform.localEulerAngles.z;
        }

        public void AddChild(Transform child)
        {
            child.parent = transform;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        //private void Update()
        //{
        //    if (Parent == null)
        //        return;

        //    transform.localPosition = this.WorldPosition();
        //    transform.localEulerAngles = new Vector3(0,0,this.WorldRotation());
        //}

        private Rigidbody2D _rigidbody;
        private Vector2 _cachedPosition;
        private float _cachedRotation;
        private Vector2 _cachedVelocity;
        private float _scale;
        private IBody _parent;
        private float _maxVelocity;
    }
}
