using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Combat.Collision.Behaviour;
using Combat.Collision.Behaviour.Action;
using Combat.Component.Body;
using Combat.Component.Bullet;
using Combat.Component.Bullet.Action;
using Combat.Component.Bullet.Lifetime;
using Combat.Component.Collider;
using Combat.Component.Controller;
using Combat.Component.DamageHandler;
using Combat.Component.Physics;
using Combat.Component.Platform;
using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Helpers;
using Combat.Scene;
using Constructor;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using Services.Audio;
using Services.ObjectPool;
using UnityEngine;

namespace Combat.Factory
{
    public class BulletFactory : IBulletFactory
    {
        public BulletFactory(Ammunition ammunition, WeaponStatModifier statModifier, IScene scene,
            ISoundPlayer soundPlayer, IObjectPool objectPool, PrefabCache prefabCache,
            SpaceObjectFactory spaceObjectFactory, EffectFactory effectFactory, IShip owner)
        {
            _ammunition = ammunition;
            _statModifier = statModifier;
            _scene = scene;
            _soundPlayer = soundPlayer;
            _objectPool = objectPool;
            _spaceObjectFactory = spaceObjectFactory;
            _effectFactory = effectFactory;
            _prefabCache = prefabCache;
            _owner = owner;

            _prefab = new Lazy<GameObject>(() => _prefabCache.GetBulletPrefab(_ammunition.Body.BulletPrefab));
            _stats = new BulletStats(ammunition, statModifier);
        }

        public IBulletStats Stats
        {
            get { return _stats; }
        }

        public IBullet Create(IWeaponPlatform parent, float spread, float rotation, float offset)
        {
            var bulletGameObject = new GameObjectHolder(_prefab, _objectPool);
            bulletGameObject.IsActive = true;

            var bulletSpeed = _stats.GetBulletSpeed();

            var body = ConfigureBody(bulletGameObject.GetComponent<IBodyComponent>(), parent, bulletSpeed, spread,
                rotation, offset);
            var view = ConfigureView(bulletGameObject.GetComponent<IView>(), _stats.Color);

            var bullet = CreateUnit(body, view, bulletGameObject);
            var collisionBehaviour = CreateCollisionBehaviour(parent);
            bullet.Collider = ConfigureCollider(bulletGameObject.GetComponent<ICollider>(), bullet);
            bullet.CollisionBehaviour = collisionBehaviour;
            bullet.Controller = CreateController(parent, bullet, bulletSpeed, spread, rotation);
            bullet.DamageHandler = CreateDamageHandler(bullet);
            bullet.CanBeDisarmed = _ammunition.Body.CanBeDisarmed;
            BulletTriggerBuilder.Build(this, bullet, collisionBehaviour);

            _scene.AddUnit(bullet);
            bullet.UpdateView(0);

            //if (!_stats.AmmunitionClass.IsBoundToCannon() && !_stats.IgnoresShipSpeed && Recoil > 0)
            //    parent.Body.ApplyForce(bullet.Body.WorldPosition(), -Recoil * (bullet.Body.WorldVelocity() - parent.Body.WorldVelocity()));

            bullet.AddResource(bulletGameObject);
            return bullet;
        }

        private BulletCollisionBehaviour CreateCollisionBehaviour(IWeaponPlatform platform)
        {
            var collisionBehaviour = new BulletCollisionBehaviour();
            var impactType = _ammunition.ImpactType;

            foreach (var effect in _ammunition.Effects)
            {
                if (effect.Type == ImpactEffectType.Damage)
                    collisionBehaviour.AddAction(new DealDamageAction(effect.DamageType, effect.Power * _stats.DamageMultiplier, impactType));
                else if (effect.Type == ImpactEffectType.Push)
                    collisionBehaviour.AddAction(new PushAction(effect.Power, impactType));
                else if (effect.Type == ImpactEffectType.Pull)
                    collisionBehaviour.AddAction(new PushAction(-effect.Power, impactType));
                else if (effect.Type == ImpactEffectType.DrainEnergy)
                    collisionBehaviour.AddAction(new DrainEnergyAction(effect.Power, impactType));
                else if (effect.Type == ImpactEffectType.SiphonHitPoints)
                    collisionBehaviour.AddAction(new SiphonHitPointsAction(effect.DamageType, effect.Power * _stats.DamageMultiplier, effect.Factor, impactType));
                else if (effect.Type == ImpactEffectType.SlowDown)
                    collisionBehaviour.AddAction(new SlowDownAction(effect.Power, impactType));
                else if (effect.Type == ImpactEffectType.CaptureDrones)
                    collisionBehaviour.AddAction(new CaptureDroneAction(impactType));
                else if (effect.Type == ImpactEffectType.Repair)
                    collisionBehaviour.AddAction(new RepairAction(effect.Power * _stats.DamageMultiplier, impactType));
            }

            if (impactType == BulletImpactType.HitFirstTarget)
            {
                if (_ammunition.Body.HitPoints > 0)
                    collisionBehaviour.AddAction(new DetonateAtTargetAction());
                else
                    collisionBehaviour.AddAction(new SelfDestructAction());
            }

            return collisionBehaviour;
        }

        private Bullet CreateUnit(IBody body, IView view, GameObjectHolder gameObject)
        {
            UnitClass unitClass;
            if (_ammunition.Body.HitPoints > 0)
                unitClass = UnitClass.Missile;
            else if (_ammunition.ImpactType == BulletImpactType.HitFirstTarget)
                unitClass = UnitClass.EnergyBolt;
            else
                unitClass = UnitClass.AreaOfEffect;

            var unitType = new UnitType(unitClass, UnitSide.Undefined, _ammunition.Body.FriendlyFire ? null : _owner);
            var bullet = new Bullet(body, view, new Lifetime(_stats.GetBulletLifetime()), unitType);

            bullet.Physics = gameObject.GetComponent<PhysicsManager>();
            return bullet;
        }

        private IBody ConfigureBody(IBodyComponent body, IWeaponPlatform parent, float bulletSpeed, float spread,
            float deltaAngle, float offset)
        {
            IBody parentBody = null;
            var position = Vector2.zero;
            var velocity = Vector2.zero;
            var rotation = deltaAngle;
            var angularVelocity = 0f;
            var weight = _stats.Weight;
            var scale = _stats.BodySize;

            if (_ammunition.Body.Type == GameDatabase.Enums.BulletType.Continuous && !parent.IsTemporary)
            {
                parentBody = parent.Body;
                position = new Vector2(offset, 0);
            }
            else
            {
                rotation = parent.Body.WorldRotation() + (UnityEngine.Random.value - 0.5f) * spread + deltaAngle;
                position = parent.Body.WorldPosition() + RotationHelpers.Direction(rotation) * offset;
            }

            if (_ammunition.Body.Type != GameDatabase.Enums.BulletType.Continuous)
            {
                velocity = RotationHelpers.Direction(rotation) * bulletSpeed;

                if (_ammunition.Body.Type == GameDatabase.Enums.BulletType.Homing)
                    velocity *= 0.1f;

                if (_ammunition.Body.Type != GameDatabase.Enums.BulletType.Static)
                    velocity += parent.Body.WorldVelocity();
            }

            body.Initialize(parentBody, position, rotation, scale, velocity, angularVelocity, weight);
            return body;
        }

        private IView ConfigureView(IView view, Color color)
        {
            view.Life = 0;
            view.Color = color;

            view.UpdateView(0);
            return view;
        }

        private ICollider ConfigureCollider(ICollider collider, IUnit unit)
        {
            collider.Unit = unit;

            if (_ammunition.Body.Type == GameDatabase.Enums.BulletType.Continuous)
                collider.MaxRange = _stats.Range;

            //if (_stats.AmmunitionClass == AmmunitionClassObsolete.IonBeam)
            //    collider.MaxRange = _stats.Velocity / 10;

            //collider.IsTrigger = !_stats.AmmunitionClass.IsBeam();

            return collider;
        }

        private IDamageHandler CreateDamageHandler(Bullet bullet)
        {
            var hitPoints = _stats.HitPoints;
            if (hitPoints > 0)
                return new MissileDamageHandler(bullet, hitPoints);
            else if (_ammunition.Body.Type == GameDatabase.Enums.BulletType.Continuous)
                return new BeamDamageHandler(bullet);
            else
                return new DefaultDamageHandler(bullet);
        }

        private IController CreateController(IWeaponPlatform parent, Bullet bullet, float bulletSpeed, float spread,
            float rotationOffset)
        {
            var range = _stats.Range;
            var weight = _stats.Weight;

            if (_ammunition.Body.Type == GameDatabase.Enums.BulletType.Homing)
                return new HomingController(bullet, bulletSpeed, 120f / (0.2f + weight * 2),
                    0.5f * bulletSpeed / (0.2f + weight * 2), range, _scene);
            else if (_ammunition.Body.Type == GameDatabase.Enums.BulletType.Continuous && !parent.IsTemporary)
                return new BeamController(bullet, spread, rotationOffset);

            return null;
        }

        private BulletFactory CreateFactory(Ammunition ammunition, WeaponStatModifier stats)
        {
            var factory = new BulletFactory(ammunition, stats, _scene, _soundPlayer, _objectPool, _prefabCache,
                _spaceObjectFactory, _effectFactory, _owner);

            factory.Stats.PowerLevel = _stats.PowerLevel;
            factory.Stats.HitPointsMultiplier = _stats.HitPointsMultiplier;
            factory.Stats.RandomFactor = _stats.RandomFactor;
            factory._nestingLevel = _nestingLevel + 1;

            return factory;
        }

        private int _nestingLevel;
        private readonly Lazy<GameObject> _prefab;
        private readonly BulletStats _stats;
        private readonly Ammunition _ammunition;
        private readonly WeaponStatModifier _statModifier;
        private readonly IScene _scene;
        private readonly ISoundPlayer _soundPlayer;
        private readonly IObjectPool _objectPool;
        private readonly SpaceObjectFactory _spaceObjectFactory;
        private readonly EffectFactory _effectFactory;
        private readonly PrefabCache _prefabCache;
        private readonly IShip _owner;

        private const int MaxNestingLevel = 100;

        private class BulletTriggerBuilder : IBulletTriggerFactory<BulletTriggerBuilder.Result>
        {
            public enum Result
            {
                Ok = 0,
                Error = 1,
                OutOfAmmo = 2,
            }

            public static void Build(BulletFactory factory, Bullet bullet, BulletCollisionBehaviour collisionBehaviour)
            {
                new BulletTriggerBuilder(factory, bullet, collisionBehaviour).Build();
            }

            private BulletTriggerBuilder(BulletFactory factory, Bullet bullet, BulletCollisionBehaviour collisionBehaviour)
            {
                _factory = factory;
                _bullet = bullet;
                _collisionBehaviour = collisionBehaviour;
            }

            private void Build()
            {
                var triggers = _factory._ammunition.Triggers;
                var count = triggers.Count;

                var hasOutOfAmmoTriggers = false;
                var outOfAmmoCondition = ConditionType.None;

                for (var i = 0; i < count; ++i)
                {
                    var trigger = triggers[i];
                    if (trigger.Condition == BulletTriggerCondition.Undefined) continue;
                    if (trigger.Condition == BulletTriggerCondition.OutOfAmmo)
                    {
                        hasOutOfAmmoTriggers = true;
                        continue;
                    }

                    _condition = FromTriggerCondition(trigger.Condition);
                    var result = trigger.Create(this);

                    if (result == Result.OutOfAmmo) 
                        outOfAmmoCondition |= _condition;
                }

                if (hasOutOfAmmoTriggers && outOfAmmoCondition != ConditionType.None)
                {
                    _condition = outOfAmmoCondition;
                    for (var i = 0; i < count; ++i)
                    {
                        var trigger = triggers[i];
                        if (trigger.Condition == BulletTriggerCondition.OutOfAmmo) 
                            trigger.Create(this);
                    }
                }
            }

            private static ConditionType FromTriggerCondition(BulletTriggerCondition condition)
            {
                switch (condition)
                {
                    case BulletTriggerCondition.Created:
                        return ConditionType.None;
                    case BulletTriggerCondition.Destroyed:
                        return ConditionType.OnDestroy;
                    case BulletTriggerCondition.Hit:
                        return ConditionType.OnCollide;
                    case BulletTriggerCondition.Disarmed:
                        return ConditionType.OnDisarm;
                    case BulletTriggerCondition.Expired:
                        return ConditionType.OnExpire;
                    case BulletTriggerCondition.Detonated:
                        return ConditionType.OnDetonate;
                    default:
                        return ConditionType.None;
                }
            }

            public Result Create(BulletTrigger_None trigger) { return Result.Ok; }

            public Result Create(BulletTrigger_PlaySfx trigger)
            {
                var condition = FromTriggerCondition(trigger.Condition);
                CreateSoundEffect(_bullet, trigger.AudioClip, condition);
                CreateVisualEffect(_bullet, _collisionBehaviour, condition, trigger);
                return Result.Ok;
            }

            public Result Create(BulletTrigger_SpawnBullet trigger)
            {
                if (trigger.Ammunition == null) return Result.Error;

                var maxNestingLevel = trigger.MaxNestingLevel > 0 ? trigger.MaxNestingLevel : MaxNestingLevel;
                if (_factory._nestingLevel >= maxNestingLevel) return Result.OutOfAmmo;

                var factory = CreateFactory(trigger.Ammunition, trigger);
                var magazine = Math.Max(trigger.Quantity, 1);
                _bullet.AddAction(new SpawnBulletsAction(factory, magazine, factory._stats.BodySize / 2, trigger.Cooldown,
                    _bullet, factory._soundPlayer, trigger.AudioClip, _condition));

                return Result.Ok;
            }

            public Result Create(BulletTrigger_Detonate content)
            {
                var condition = FromTriggerCondition(content.Condition);
                _bullet.AddAction(new DetonateAction(condition));
                return Result.Ok;
            }

            private void CreateSoundEffect(Bullet bullet, AudioClipId audioClip, ConditionType condition)
            {
                if (!audioClip) return;

                if (condition == ConditionType.None && !audioClip.Loop)
                    _factory._soundPlayer.Play(audioClip);
                else
                    bullet.AddAction(new PlaySoundAction(_factory._soundPlayer, audioClip, condition));
            }

            private void CreateVisualEffect(Bullet bullet, BulletCollisionBehaviour collisionBehaviour,
                ConditionType condition, BulletTrigger_PlaySfx trigger)
            {
                if (trigger.VisualEffect == null) return;

                var color = trigger.ColorMode.Apply(trigger.Color, _factory._stats.Color);
                var size = trigger.Size > 0 ? trigger.Size : 1.0f;

                if (condition == ConditionType.OnCollide)
                    collisionBehaviour.AddAction(new ShowHitEffectAction(_factory._effectFactory, trigger.VisualEffect, color,
                        size * _factory._stats.BodySize, trigger.Lifetime));
                else
                    bullet.AddAction(new PlayEffectAction(bullet, _factory._effectFactory, trigger.VisualEffect, color, size,
                        trigger.Lifetime, condition));
            }

            private BulletFactory CreateFactory(Ammunition ammunition, BulletTrigger_SpawnBullet trigger)
            {
                var stats = _factory._statModifier;
                stats.Color = trigger.ColorMode.Apply(trigger.Color, _factory._stats.Color);
                if (trigger.PowerMultiplier > 0) stats.DamageMultiplier *= trigger.PowerMultiplier;
                if (trigger.Size > 0)
                {
                    stats.AoeRadiusMultiplier *= trigger.Size;
                    stats.WeightMultiplier *= trigger.Size;
                    stats.RangeMultiplier *= trigger.Size;
                }

                var factory = _factory.CreateFactory(trigger.Ammunition, stats);
                factory.Stats.RandomFactor = trigger.RandomFactor;
                return factory;
            }

            private ConditionType _condition;

            private readonly BulletFactory _factory;
            private readonly Bullet _bullet;
            private readonly BulletCollisionBehaviour _collisionBehaviour;
        }
    }
}