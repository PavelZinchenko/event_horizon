using Combat.Component.Body;
using Combat.Component.Unit;
using UnityEngine;

namespace Combat.Component.Bullet
{
    public static class BulletExtensions
    {
        public static Vector2 GetHitPoint(this IUnit bullet)
        {
            if (bullet.Collider.ActiveCollision != null)
                return bullet.Collider.LastContactPoint;

            return bullet.Body.WorldPosition();// + RotationHelpers.Direction(bullet.Body.WorldRotation())*bullet.Body.Scale;
        }
    }
}
