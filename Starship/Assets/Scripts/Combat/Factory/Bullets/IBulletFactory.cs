using Combat.Component.Bullet;
using Combat.Component.Platform;

namespace Combat.Factory
{
    public interface IBulletFactory
    {
        IBullet Create(IWeaponPlatform parent, float spread, float rotation, float offset);
        IBulletStats Stats { get; }
    }
}
