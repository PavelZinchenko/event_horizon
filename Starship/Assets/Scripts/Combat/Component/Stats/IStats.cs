using System;
using Combat.Collision;
using Combat.Component.Mods;
using Combat.Unit.HitPoints;

namespace Combat.Component.Stats
{
    public interface IStats : IDisposable
    {
        bool IsAlive { get; }

        IResourcePoints Armor { get; }
        IResourcePoints Shield { get; }
        IResourcePoints Energy { get; }

        float WeaponDamageMultiplier { get; }
        float RammingDamageMultiplier { get; }
        float HitPointsMultiplier { get; }

        Resistance Resistance { get; }

        Modifications<Resistance> Modifications { get; }

        float TimeFromLastHit { get; }

        void ApplyDamage(Impact damage);
        void UpdatePhysics(float elapsedTime);
    }
}
