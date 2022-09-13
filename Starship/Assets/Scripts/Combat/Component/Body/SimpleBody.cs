using UnityEngine;

namespace Combat.Component.Body
{
    public class SimpleBody : MonoBehaviour, IBody
    {
        public static SimpleBody Create(IBody parent, Vector2 position, float rotation, float scale, float weight, float offset)
        {
            var gameobject = new GameObject("Body");
            var transform = gameobject.transform;
            parent?.AddChild(transform);

            var body = gameobject.AddComponent<SimpleBody>();
            body.Initialize(parent, position, rotation, scale, weight, offset);
            return body;
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }

        public IBody Parent { get; private set; }
        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }
        public float Offset { get; private set; }
        public Vector2 Velocity { get { return Vector2.zero; } }
        public float AngularVelocity { get { return 0f; } }
        public float Weight { get; private set; }
        public float Scale { get; private set; }

        public void Move(Vector2 position)
        {
            Position = position;
            transform.localPosition = Parent?.ChildPosition(Position) ?? Position;
        }

        public void Turn(float rotation)
        {
            Rotation = Mathf.Repeat(rotation, 360);
            transform.localEulerAngles = new Vector3(0, 0, Rotation);
        }

        public void SetSize(float size)
        {
            Scale = size;
            transform.localScale = Scale * Vector3.one;
        }

        public void ApplyAcceleration(Vector2 acceleration)
        {
            //if (_parent != null)
            //    _parent.ApplyAcceleration(acceleration);
        }

        public void ApplyAngularAcceleration(float acceleration)
        {
            //if (_parent != null)
            //    _parent.ApplyAngularAcceleration(acceleration);
        }

        public void ApplyForce(Vector2 position, Vector2 force)
        {
            Parent?.ApplyForce(position, force);
        }

        public void SetVelocityLimit(float value) {}
        public void SetAngularVelocityLimit(float value) { }

        public void UpdatePhysics(float elapsedTime) {}
        public void UpdateView(float elapsedTime) {}

        public void AddChild(Transform child)
        {
            child.parent = transform;
        }

        private void Initialize(IBody parent, Vector2 position, float rotation, float scale, float weight, float offset)
        {
            Parent = parent;
            Weight = weight;
            Offset = offset;
            Move(position);
            Turn(rotation);
            SetSize(scale);
        }
    }
}
