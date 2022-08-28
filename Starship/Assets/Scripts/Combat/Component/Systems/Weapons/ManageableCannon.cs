using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using Combat.Unit;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Weapons
{
    public class ManageableCannon : SystemBase, IWeapon
    {
        public ManageableCannon(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding)
            : base(keyBinding, weaponStats.ControlButtonIcon)
        {
            MaxCooldown = weaponStats.FireRate > 0 ? 1f / weaponStats.FireRate : 0f;

            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost;
            _spread = weaponStats.Spread;
            _rotation = weaponStats.Rotation;
            _initialPosition = weaponStats.InitialPosition;

            Info = new WeaponInfo(WeaponType.Manageable, _spread, bulletFactory, platform, 0, _initialPosition);
        }

        public override float ActivationCost { get { return _energyConsumption; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && _platform.IsReady && (HasActiveBullet || _platform.EnergyPoints.Value >= _energyConsumption); } }
        public override float Cooldown { get { return Mathf.Max(base.Cooldown, _platform.Cooldown); } }
        public IBullet ActiveBullet { get { return HasActiveBullet ? _activeBullet : null; } }

        public WeaponInfo Info { get; private set; }
        public IWeaponPlatform Platform { get { return _platform; } }
        public float PowerLevel { get { return 1.0f; } }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (_activeBullet != null)
            {
                if (_activeBullet.State != UnitState.Active)
                {
                    _platform.OnShot();
                    TimeFromLastUse = 0;
                    _activeBullet = null;
                    InvokeTriggers(ConditionType.OnDeactivate);
                }
                else if (!Active)
                {
                    _activeBullet.Detonate();
                }
            }
            else if (Active && CanBeActivated && _platform.EnergyPoints.TryGet(_energyConsumption))
            {
                Shot();
                InvokeTriggers(ConditionType.OnActivate);
            }
        }

        protected override void OnDispose()
        {
            //if (HasActiveBullet)
            //    _activeBullet.Detonate();
        }

        private void Shot()
        {
            if (HasActiveBullet)
                _activeBullet.Detonate();

            _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
            _activeBullet = _bulletFactory.Create(_platform, _spread, _rotation, 0, _initialPosition);
        }

        private bool HasActiveBullet { get { return _activeBullet.IsActive(); } }

        private readonly Vector2 _initialPosition;
        private IBullet _activeBullet;
        private readonly float _rotation;
        private readonly float _spread;
        private readonly float _energyConsumption;
        private readonly IWeaponPlatform _platform;
        private readonly Factory.IBulletFactory _bulletFactory;
    }
}
