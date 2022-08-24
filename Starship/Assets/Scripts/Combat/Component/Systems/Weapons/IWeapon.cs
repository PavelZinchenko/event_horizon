using Combat.Component.Bullet;
using Combat.Component.Platform;

namespace Combat.Component.Systems.Weapons
{
    public interface IWeapon : ISystem
    {
        WeaponInfo Info { get; }
        IWeaponPlatform Platform { get; }
        float PowerLevel { get; }
        IBullet ActiveBullet { get; }
    }
}
