using System;
using Combat.Component.Platform;
using Combat.Component.Ship;
using Combat.Component.Systems;
using Combat.Component.Systems.Weapons;
using Combat.Component.Triggers;
using Combat.Scene;
using Constructor;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Audio;
using Services.ObjectPool;
using UnityEngine;
using Zenject;

namespace Combat.Factory
{
    public class WeaponFactory
    {
        [Inject] private readonly IScene _scene;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly IObjectPool _objectPool;
        [Inject] private readonly SpaceObjectFactory _spaceObjectFactory;
        [Inject] private readonly EffectFactory _effectFactory;
        [Inject] private readonly PrefabCache _prefabCache;

        public ISystem Create(IWeaponData weaponData, IWeaponPlatform platform, float hitPointsMultiplier, IShip owner)
        {
            var bulletFactory = new BulletFactory(weaponData.Ammunition, weaponData.Stats, _scene, _soundPlayer, _objectPool, _prefabCache, _spaceObjectFactory, _effectFactory, owner);
            bulletFactory.Stats.HitPointsMultiplier = hitPointsMultiplier;
            var stats = weaponData.Weapon.Stats;
            stats.FireRate *= weaponData.Stats.FireRateMultiplier.Value;
            return Create(stats, weaponData.KeyBinding, bulletFactory, platform);
        }

        public ISystem Create(IWeaponDataObsolete weaponData, IWeaponPlatform platform, float hitPointsMultiplier, IShip owner)
        {
            var bulletFactory = new BulletFactoryObsolete(weaponData.Ammunition, _scene, _soundPlayer, _objectPool, _prefabCache, _spaceObjectFactory, _effectFactory, owner);
            bulletFactory.Stats.HitPointsMultiplier = hitPointsMultiplier;
            return Create(weaponData.Weapon, weaponData.KeyBinding, bulletFactory, platform);
        }

        private ISystem Create(WeaponStats weaponStats, int keyBinding, IBulletFactory bulletFactory, IWeaponPlatform platform)
        {
            switch (weaponStats.WeaponClass)
            {
                case WeaponClass.Manageable:
                    {
                        var weapon = new ManageableCannon(platform, weaponStats, bulletFactory, keyBinding);
                        if (weaponStats.ShotSound)
                            weapon.AddTrigger(new SoundEffect(_soundPlayer, weaponStats.ShotSound, ConditionType.OnActivate, ConditionType.OnDeactivate));
                        if (weaponStats.ShotEffectPrefab)
                            weapon.AddTrigger(CreateFlashEffect(weaponStats, bulletFactory, platform));
                        return weapon;
                    }
                case WeaponClass.Continuous:
                    {
                        var weapon = new ContinuousCannon(platform, weaponStats, bulletFactory, keyBinding);
                        if (weaponStats.ShotSound)
                            weapon.AddTrigger(new SoundEffect(_soundPlayer, weaponStats.ShotSound, ConditionType.OnActivate, ConditionType.OnDeactivate));
                        if (weaponStats.ShotEffectPrefab)
                            weapon.AddTrigger(CreateFlashEffect(weaponStats, bulletFactory, platform, ConditionType.OnActivate | ConditionType.OnRemainActive));
                        return weapon;
                    }
                case WeaponClass.MashineGun:
                    {
                        bulletFactory.Stats.RandomFactor = 0.2f;
                        var weapon = new MachineGun(platform, weaponStats, bulletFactory, keyBinding);
                        if (weaponStats.ShotSound)
                            weapon.AddTrigger(new SoundEffect(_soundPlayer, weaponStats.ShotSound, ConditionType.OnActivate));
                        if (weaponStats.ShotEffectPrefab)
                            weapon.AddTrigger(CreateFlashEffect(weaponStats, bulletFactory, platform));
                        return weapon;
                    }
                case WeaponClass.MultiShot:
                    {
                        bulletFactory.Stats.RandomFactor = 0.3f;
                        var weapon = new MultishotCannon(platform, weaponStats, bulletFactory, keyBinding);
                        if (weaponStats.ShotSound)
                            weapon.AddTrigger(new SoundEffect(_soundPlayer, weaponStats.ShotSound, ConditionType.OnActivate));
                        if (weaponStats.ShotEffectPrefab)
                            weapon.AddTrigger(CreateFlashEffect(weaponStats, bulletFactory, platform));
                        return weapon;
                    }
                case WeaponClass.RequiredCharging:
                    {
                        var weapon = new ChargeableCannon(platform, weaponStats, bulletFactory, keyBinding);
                        if (weaponStats.ShotEffectPrefab)
                            weapon.AddTrigger(CreatePowerLevelEffect(weapon, weaponStats, bulletFactory));
                        if (weaponStats.ShotSound)
                            weapon.AddTrigger(new SoundEffect(_soundPlayer, weaponStats.ShotSound, ConditionType.OnDischarge));
                        if (weaponStats.ChargeSound)
                            weapon.AddTrigger(new SoundEffect(_soundPlayer, weaponStats.ChargeSound, ConditionType.OnActivate));
                        return weapon;
                    }
                case WeaponClass.Common:
                    {
                        var weapon = new CommonCannon(platform, weaponStats, bulletFactory, keyBinding);
                        if (weaponStats.ShotSound)
                            weapon.AddTrigger(new SoundEffect(_soundPlayer, weaponStats.ShotSound, ConditionType.OnActivate));
                        if (weaponStats.ShotEffectPrefab)
                            weapon.AddTrigger(CreateFlashEffect(weaponStats, bulletFactory, platform));
                        return weapon;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IUnitEffect CreateFlashEffect(WeaponStats stats, IBulletFactory bulletFactory, IWeaponPlatform platform, ConditionType condition = ConditionType.OnActivate)
        {
            if (!stats.ShotEffectPrefab)
                return null;

            var effect = _effectFactory.CreateEffect(stats.ShotEffectPrefab);
            effect.Color = bulletFactory.Stats.FlashColor;
            effect.Size = bulletFactory.Stats.FlashSize;
            return new FlashEffect(effect, platform.Body, bulletFactory.Stats.FlashTime, Vector2.zero, condition);
        }

        private IUnitEffect CreatePowerLevelEffect(IWeapon weapon, WeaponStats stats, IBulletFactory bulletFactory)
        {
            if (!stats.ShotEffectPrefab)
                return null;

            var effect = _effectFactory.CreateEffect(stats.ShotEffectPrefab);
            effect.Color = bulletFactory.Stats.FlashColor;
            effect.Size = bulletFactory.Stats.FlashSize;
            return new PowerLevelEffect(weapon, effect, Vector2.zero, bulletFactory.Stats.FlashTime, ConditionType.OnActivate);
        }
    }
}
