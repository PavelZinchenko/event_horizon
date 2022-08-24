using System;
using Combat.Collision;

namespace Combat.Component.Stats
{
    public interface IDamageIndicator : IDisposable
    {
        void ApplyDamage(Impact damage);
        void Update(float elapsedTime);
    }
}
