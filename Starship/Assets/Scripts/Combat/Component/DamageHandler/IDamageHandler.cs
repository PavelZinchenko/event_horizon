using System;
using Combat.Collision;

namespace Combat.Component.DamageHandler
{
    public interface IDamageHandler : IDisposable
    {
        CollisionEffect ApplyDamage(Impact impact);
    }
}
