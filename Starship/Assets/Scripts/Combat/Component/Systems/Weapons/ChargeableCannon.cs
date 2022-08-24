using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using Combat.Unit;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Weapons
{
    public class ChargeableCannon : SystemBase, IWeapon
    {
        public ChargeableCannon(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding)
            : base(keyBinding, weaponStats.ControlButtonIcon)
        {
            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost;
            _spread = weaponStats.Spread;
            _chargeTotalTime = 1.0f / weaponStats.FireRate;

            Info = new WeaponInfo(WeaponType.RequiredCharging, _spread, bulletFactory, platform);
        }

        public override bool CanBeActivated { get { return _chargeTime > 0 || (_platform.IsReady && _platform.EnergyPoints.Value >= _energyConsumption*0.5f); } }
        public override float Cooldown { get { return _platform.Cooldown; } }

        public WeaponInfo Info { get; private set; }
        public IWeaponPlatform Platform { get { return _platform; } }
        public float PowerLevel { get { return Mathf.Clamp01(_chargeTime / _chargeTotalTime); } }
        public IBullet ActiveBullet { get { return null; } }

        protected override void OnUpdateView(float elapsedTime) {}

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && _chargeTime > 0 && (_chargeTime > _chargeTotalTime || _platform.EnergyPoints.TryGet(_energyConsumption*elapsedTime / _chargeTotalTime)))
            {
                _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
                _chargeTime += elapsedTime;
                UpdatePower();
            }
            else if (_chargeTime > 0)
            {
                if (_chargeTime > 0.1f) Shot();
                _chargeTime = 0;
                InvokeTriggers(ConditionType.OnDeactivate);
            }
            else if (Active && CanBeActivated)
            {
                InvokeTriggers(ConditionType.OnActivate);
                _chargeTime += elapsedTime;
                UpdatePower();
            }
            else if (HasActiveBullet && Info.BulletType == BulletType.Direct)
            {
                _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
            }
        }

        protected override void OnDispose() {}

        private void Shot()
        {
            _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
            _platform.OnShot();
            _activeBullet = _bulletFactory.Create(_platform, _spread, 0, 0);

            InvokeTriggers(ConditionType.OnDischarge);
        }

        private void UpdatePower()
        {
            _bulletFactory.Stats.PowerLevel = PowerLevel;
        }

        private bool HasActiveBullet { get { return _activeBullet.IsActive(); } }

        private float _chargeTime;

        private IBullet _activeBullet;
        private readonly float _chargeTotalTime;
        private readonly float _spread;
        private readonly float _energyConsumption;
        private readonly IWeaponPlatform _platform;
        private readonly Factory.IBulletFactory _bulletFactory;
    }
}
