using System;

namespace Combat.Component.Bullet.Action
{
    [Flags]
    public enum ConditionType
    {
        None        = 0,
        OnCollide   = 1,
        OnDetonate  = 2,
        OnExpire    = 4,
        OnDisarm    = 8,
        OnDestroy   = 16,
    }

    public static class ConditionTypeExtensions
    {
        public static bool Contains(this ConditionType effect, ConditionType other)
        {
            return (effect & other) == other;
        }

        public static ConditionType Remove(this ConditionType effect, ConditionType other)
        {
            return effect & ~other;
        }
    }
}
