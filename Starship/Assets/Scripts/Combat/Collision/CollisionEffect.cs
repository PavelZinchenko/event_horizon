using System;

namespace Combat.Collision
{
    [Flags]
    public enum CollisionEffect : uint
    {
        None = 0,
        Destroy = 1,
        Disarm = 2,
        Activate = 4,
    }

    public static class CollisionEffectExtensions
    {
        public static bool Contains(this CollisionEffect effect, CollisionEffect other)
        {
            return (effect & other) == other;
        }

        public static CollisionEffect Remove(this CollisionEffect effect, CollisionEffect other)
        {
            return effect & ~other;
        }
    }
}
