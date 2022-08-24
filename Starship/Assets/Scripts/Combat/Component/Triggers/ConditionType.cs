using System;

namespace Combat.Component.Triggers
{
    [Flags]
    public enum ConditionType
    {
        None         = 0,
        OnDestroy    = 1,
        OnHit        = 2,
        OnActivate   = 4,
        OnDeactivate = 8,
        OnRemainActive = 16,
        OnDischarge = 32,
    }

    public static class ConditionTypeExtensions
    {
        public static bool Contains(this ConditionType effect, ConditionType other)
        {
            return other != ConditionType.None && (effect & other) == other;
        }

        public static ConditionType Remove(this ConditionType effect, ConditionType other)
        {
            return effect & ~other;
        }
    }
}
