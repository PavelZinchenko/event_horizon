using Combat.Component.Body;
using Combat.Component.Unit;
using UnityEngine;

namespace Combat.Collision.Manager
{
    public struct CollisionData
    {
        public static CollisionData FromCollision(Vector2 contactPoint, Vector2 relativeVelocity, bool isNew, float timeInterval)
        {
            return new CollisionData
            {
                Position = contactPoint,
                TimeInterval = timeInterval,
                RelativeVelocity = relativeVelocity,
                IsNew = isNew,
            };
        }

        public static CollisionData FromRaycastHit2D(RaycastHit2D hit, bool isNew, float timeInterval)
        {
            return new CollisionData
            {
                IsNew = isNew,
                Position = hit.point,
                TimeInterval = timeInterval,
                RelativeVelocity = Vector2.zero,
            };
        }

        public static CollisionData FromObjects(IUnit first, IUnit second, Vector2 position, bool isNew, float timeInterval)
        {
            return new CollisionData
            {
                Position = position,
                RelativeVelocity = second.Body.WorldVelocity() - first.Body.WorldVelocity(),
                TimeInterval = timeInterval,
                IsNew = isNew,
            };
        }

        public bool IsNew;
        public float TimeInterval;
        public Vector2 RelativeVelocity;
        public Vector2 Position;
    }
}
