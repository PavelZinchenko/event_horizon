using UnityEngine;

namespace Combat.Component.Body
{
    public class GameObjectBody : MonoBehaviour, IBodyComponent
    {
        public void Initialize(IBody parent, Vector2 position, float rotation, float scale, Vector2 velocity,
            float angularVelocity, float weight)
        {
            if (parent != null)
                parent.AddChild(transform);
            else
                transform.parent = null;

            Parent = parent;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
            Weight = weight;
        }

        public IBody Parent
        {
            get { return _parent; }
            set
            {
                if (_parent == value)
                    return;

                if (value == null)
                {
                    _position = _parent.WorldPosition();
                    _rotation = _parent.WorldRotation();
                }
                else
                {
                    _position = Vector2.zero;
                    _rotation = 0;
                }

                _parent = value;
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                if (this && transform)
                    gameObject.Move(Parent == null ? value : Parent.ChildPosition(value));
            }
        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                if (this && transform)
                    transform.localEulerAngles = new Vector3(0, 0, Mathf.Repeat(value, 360));
            }
        }

        public float Offset { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }
        public float Weight { get; set; }

        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                if (transform)
                    transform.localScale = Vector3.one * value;
            }
        }

        public void ApplyAcceleration(Vector2 acceleration) { }

        public void ApplyAngularAcceleration(float acceleration) { }

        public void ApplyForce(Vector2 position, Vector2 force)
        {
            if (Parent != null)
                Parent.ApplyForce(position, force);
        }

        public void SetVelocityLimit(float value) { }
        public void SetAngularVelocityLimit(float value) { }

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
            if (Parent != null)
                return;

            Position += Velocity * elapsedTime;
            Rotation += AngularVelocity * elapsedTime * Mathf.Rad2Deg;
        }

        public void UpdateView(float elapsedTime) { }

        public void AddChild(Transform child)
        {
            child.parent = transform;
        }

        //private void Awake()
        //{
        //    _position = transform.localPosition;
        //    _rotation = transform.localEulerAngles.z;
        //    _scale = transform.localScale.z;
        //}

        //private void Update()
        //{
        //    if (Parent == null || transform.parent != null)
        //        return;

        //    transform.localPosition = this.WorldPosition();
        //    transform.localEulerAngles = new Vector3(0, 0, this.WorldRotation());
        //}

        private Vector2 _position;
        private float _rotation;
        private float _scale;
        private IBody _parent;
    }
}
