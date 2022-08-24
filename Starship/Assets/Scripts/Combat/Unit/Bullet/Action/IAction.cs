using System;
using Combat.Collision;

namespace Combat.Component.Bullet.Action
{
    public interface IAction : IDisposable
    {
        ConditionType Condition { get; }
        CollisionEffect Invoke();
    }
}
