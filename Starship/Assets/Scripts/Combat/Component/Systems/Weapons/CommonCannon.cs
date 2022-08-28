using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Weapons
{
    public class CommonCannon : SystemBase, IWeapon
    {
        public CommonCannon(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding)
            : base(keyBinding, weaponStats.ControlButtonIcon)
        {
            MaxCooldown = weaponStats.FireRate > 0 ? 1f/weaponStats.FireRate : 0f;

            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost;
            _spread = weaponStats.Spread;
            _rotation = weaponStats.Rotation;
            _initialPosition = weaponStats.InitialPosition;

            Info = new WeaponInfo(WeaponType.Common, _spread, bulletFactory, platform, _rotation, _initialPosition);
        }

        public override float ActivationCost { get { return _energyConsumption; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && _platform.IsReady && _platform.EnergyPoints.Value >= _energyConsumption; } }
        public override float Cooldown { get { return Mathf.Max(_platform.Cooldown, base.Cooldown); } }

        public WeaponInfo Info { get; private set; }
        public IWeaponPlatform Platform { get { return _platform; } }
        public float PowerLevel { get { return 1.0f; } }
        public IBullet ActiveBullet { get { return null; } }

        protected override void OnUpdateView(float elapsedTime) {}

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && _platform.EnergyPoints.TryGet(_energyConsumption))
            {
                Shot();
                TimeFromLastUse = 0;
                InvokeTriggers(ConditionType.OnActivate);
            }
        }

        protected override void OnDispose() {}

        private void Shot()
        {
            _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
            _platform.OnShot();
            _bulletFactory.Create(_platform, _spread, _rotation, 0, _initialPosition);
        }

        private readonly Vector2 _initialPosition;
        private readonly float _spread;
        private readonly float _rotation;
        private readonly float _energyConsumption;
        private readonly IWeaponPlatform _platform;
        private readonly Factory.IBulletFactory _bulletFactory;
    }
}
