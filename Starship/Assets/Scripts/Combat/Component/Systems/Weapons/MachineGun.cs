using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Weapons
{
    public class MachineGun : SystemBase, IWeapon
    {
        public MachineGun(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding)
            : base(keyBinding, weaponStats.ControlButtonIcon)
        {
            MaxCooldown = weaponStats.FireRate > 0 ? 1f / weaponStats.FireRate : 0f;

            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost;
            _spread = weaponStats.Spread;
            _magazine = weaponStats.Magazine;

            Info = new WeaponInfo(WeaponType.Common, _spread, bulletFactory, platform);
        }

        public override float ActivationCost { get { return _energyConsumption; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && _platform.EnergyPoints.Value > _energyConsumption; } }
        public override float Cooldown { get { return Mathf.Max(base.Cooldown, _platform.Cooldown); } }

        public WeaponInfo Info { get; private set; }
        public IWeaponPlatform Platform { get { return _platform; } }
        public float PowerLevel { get { return 1.0f; } }
        public IBullet ActiveBullet { get { return null; } }


        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && _shots < _magazine && CanBeActivated)
            {
                if (_platform.IsReady && _platform.EnergyPoints.TryGet(_energyConsumption))
                {
                    Shot();
                    _shots++;
                    InvokeTriggers(ConditionType.OnActivate);
                }
            }
            else if (_shots >= _magazine)
            {
                _shots = 0;
                TimeFromLastUse = 0;
            }
        }

        protected override void OnDispose() {}

        private void Shot()
        {
            _platform.Aim(Info);
            _platform.OnShot();
            _bulletFactory.Create(_platform, _spread, 0, 0);
        }

        private int _shots;
        private readonly int _magazine;
        private readonly float _spread;
        private readonly float _energyConsumption;
        private readonly IWeaponPlatform _platform;
        private readonly Factory.IBulletFactory _bulletFactory;
    }
}
