using Combat.Component.Body;
using Combat.Component.Platform;
using Combat.Factory;
using UnityEngine;

namespace Combat.Component.Systems.Weapons
{
    public enum WeaponType
    {
        Common,
        Manageable,
        Continuous,
        RequiredCharging,
    }

    public enum BulletType
    {
        Direct,
        Homing,
        Projectile,
        AreaOfEffect,
    }

    public enum BulletEffectType
    {
        Common,
        DamageOverTime,
        Repair,
        Special,
        ForDronesOnly,
    }

    public class WeaponInfo
    {
        public WeaponInfo(WeaponType weaponType, float spread, IBulletFactory bulletFactory, IWeaponPlatform platform)
        {
            _bulletFactory = bulletFactory;
            _weaponType = weaponType;
            _spread = spread;

            if (platform.Body.Parent != null && weaponType == WeaponType.Common)
                Recoil = _bulletFactory.Stats.Recoil / platform.Body.TotalWeight();
        }

        public WeaponType WeaponType => _weaponType;
        public BulletType BulletType => _bulletFactory.Stats.Type;
        public BulletEffectType BulletEffectType => _bulletFactory.Stats.EffectType;
        public float Range => _bulletFactory.Stats.BulletHitRange;
        public float Spread => _spread;
        public bool IsRelativeVelocity => !_bulletFactory.Stats.IgnoresShipSpeed;
        public float BulletSpeed => _bulletFactory.Stats.BulletSpeed;
        public float EnergyCost => _bulletFactory.Stats.EnergyCost;
        public float Recoil { get; private set; }

        private readonly float _spread;
        private readonly WeaponType _weaponType;
        private readonly IBulletFactory _bulletFactory;
    }
}
