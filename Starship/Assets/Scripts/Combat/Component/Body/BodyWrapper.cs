using Combat.Component.Body;
using UnityEngine;

namespace Combat.Component.Bullet.Action
{
    public class BodyWrapper : IBody
    {
        public BodyWrapper(IBody body)
        {
            _body = body;
        }

        public void Dispose() { }
        public IBody Parent { get { return _body.Parent; } }
        public Vector2 Position { get { return _body.Position; } }
        public float Rotation { get { return _body.Rotation + _rotation; } }
        public float Offset { get { return _body.Offset; } }
        public Vector2 Velocity { get { return _body.Velocity; } }
        public float AngularVelocity { get { return _body.AngularVelocity; } }
        public float Weight { get { return _body.Weight; } }
        public float Scale { get { return _body.Scale; } }
        public void Move(Vector2 position) { }
        public void Turn(float rotation) { _rotation = rotation; }
        public void SetSize(float size) { }
        public void ApplyAcceleration(Vector2 acceleration) { }
        public void ApplyAngularAcceleration(float acceleration) { }
        public void ApplyForce(Vector2 position, Vector2 force) { }
        public void SetVelocityLimit(float value) { }

        public void UpdatePhysics(float elapsedTime) { }
        public void UpdateView(float elapsedTime) { }
        public void AddChild(Transform child) { }

        private float _rotation;
        private readonly IBody _body;
    }
}
