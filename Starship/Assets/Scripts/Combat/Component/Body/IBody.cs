using System;
using Combat.Component.Systems.Devices;
using UnityEngine;

namespace Combat.Component.Body
{
    public interface IBody : IDisposable
    {
        IBody Parent { get; }

        Vector2 Position { get; }
        float Rotation { get; }
        float Offset { get; }

        Vector2 Velocity { get; }
        float AngularVelocity { get; }

        float Weight { get; }
        float Scale { get; }

        void Move(Vector2 position);
        void Turn(float rotation);
        void SetSize(float size);
        void ApplyAcceleration(Vector2 acceleration);
        void ApplyAngularAcceleration(float acceleration);
        void ApplyForce(Vector2 position, Vector2 force);
        void SetVelocityLimit(float value);
        void SetAngularVelocityLimit(float value);

        void UpdatePhysics(float elapsedTime);
        void UpdateView(float elapsedTime);

        void AddChild(Transform child);
    }

    public static class BodyExtensions
    {
        public static Vector2 WorldPosition(this IBody body)
        {
            var position = body.Position;
            if (body.Offset != 0)
            {
                position += RotationHelpers.Direction(body.Rotation) * body.Offset;
            }

            if (body.Parent == null)
                return position;

            return body.Parent.WorldPosition() + RotationHelpers.Transform(position, body.Parent.WorldRotation())*body.Parent.WorldScale();
        }

        public static Vector2 ChildPosition(this IBody body, Vector2 position)
        {
            return new Vector2(body.Offset/body.Scale + position.x, position.y);
        }

        public static Vector2 WorldPositionNoOffset(this IBody body)
        {
            var position = body.Position;

            if (body.Parent == null)
                return position;

            return body.Parent.WorldPosition() + RotationHelpers.Transform(position, body.Parent.WorldRotation())*body.Parent.WorldScale();
        }

        public static float WorldRotation(this IBody body)
        {
            if (body.Parent == null)
                return body.Rotation;

            return body.Rotation + body.Parent.WorldRotation();
        }

        public static Vector2 WorldVelocity(this IBody body)
        {
            if (body.Parent == null)
                return body.Velocity;

            return body.Parent.Velocity + RotationHelpers.Transform(body.Velocity, body.Parent.WorldRotation());
        }

        public static float WorldAngularVelocity(this IBody body)
        {
            if (body.Parent == null)
                return body.AngularVelocity;

            return body.AngularVelocity + body.Parent.WorldAngularVelocity();
        }

        public static float WorldScale(this IBody body)
        {
            if (body.Parent == null)
                return body.Scale;

            return body.Scale * body.Parent.WorldScale();
        }

        public static float TotalWeight(this IBody body)
        {
            if (body.Parent == null)
                return body.Weight;

            return body.Weight + body.Parent.TotalWeight();
        }

        public static IBody Owner(this IBody body)
        {
            var parent = body;
            while (parent.Parent != null)
                parent = parent.Parent;

            return parent;
        }

        /// <summary>
        /// Shifts a body and all its connections
        /// </summary>
        /// <param name="body">Body to shift</param>
        /// <param name="offset">OffsetShift offset</param>
        public static void ShiftWithDependants(this IBody body, Vector2 offset)
        {
            body.Move(body.Position + offset);
            if (!WormTailDevice.Dependencies.TryGetValue(body, out var connections)) return;
            for (var i = 0; i < connections.Count; i++)
            {
                var element = connections[i];
                if (!element.IsAlive)
                {
                    connections.RemoveAt(i);
                    i--;
                    continue;
                }
                element.Target.Move(element.Target.Position + offset);
            }
        }

        /// <summary>
        /// Moves a body to the new position, and shifts all of its dependencies according to a difference between old
        /// and new position
        /// </summary>
        /// <param name="body">Body to move</param>
        /// <param name="newPosition">New position</param>
        public static void MoveWithDependants(this IBody body, Vector2 newPosition)
        {
            ShiftWithDependants(body, newPosition - body.Position);
        }
    }
}
