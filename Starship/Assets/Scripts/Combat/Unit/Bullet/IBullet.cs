using Combat.Component.Bullet.Lifetime;
using Combat.Component.Unit;

namespace Combat.Component.Bullet
{
    public interface IBullet : IUnit
    {
        ILifetime Lifetime { get; }
        void Detonate();
        bool CanBeDisarmed { get; }
    }
}
