using System;
using UnityEngine;

namespace Combat.Component.Body
{
    public class EmptyBody : IBody
    {
        public EmptyBody(IBody parent, Vector2 position, float rotation, float scale, float weight, float offset)
        {
            Parent = parent;
            Weight = weight;
            Offset = offset;
            Move(position);
            Turn(rotation);
            SetSize(scale);
        }

        public void Dispose() { }

        public IBody Parent { get; private set; }
        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }
        public float Offset { get; private set; }

        public Vector2 Velocity => Vector2.zero;

        public float AngularVelocity => 0f;

        public float Weight { get; private set; }
        public float Scale { get; private set; }

        public void Move(Vector2 position)
        {
            Position = position;
        }

        public void Turn(float rotation)
        {
            Rotation = Mathf.Repeat(rotation, 360);
        }

        public void SetSize(float size)
        {
            Scale = size;
        }

        public void ApplyAcceleration(Vector2 acceleration) { }

        public void ApplyAngularAcceleration(float acceleration) { }

        public void ApplyForce(Vector2 position, Vector2 force)
        {
            Parent?.ApplyForce(position, force);
        }

        public void SetVelocityLimit(float value) { }

        public void SetAngularVelocityLimit(float value) { }

        public void UpdatePhysics(float elapsedTime) { }
        public void UpdateView(float elapsedTime) { }

        public void AddChild(Transform child)
        {
            throw new InvalidOperationException();
        }
    }
}
