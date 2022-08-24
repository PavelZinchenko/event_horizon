using Combat.Collision.Behaviour;
using Combat.Collision.Behaviour.Action;
using Combat.Component.Body;
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
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using Services.Audio;
using Services.ObjectPool;
using UnityEngine;
using Bullet = Combat.Component.Bullet.Bullet;
using IBullet = Combat.Component.Bullet.IBullet;

namespace Combat.Factory
{
    public class BulletFactoryObsolete : IBulletFactory
    {
        public BulletFactoryObsolete(AmmunitionObsoleteStats ammunitionStats, IScene scene, ISoundPlayer soundPlayer, IObjectPool objectPool, PrefabCache prefabCache, SpaceObjectFactory spaceObjectFactory, EffectFactory effectFactory, IShip owner)
        {
            _stats = ammunitionStats;
            _scene = scene;
            _soundPlayer = soundPlayer;
            _objectPool = objectPool;
            _spaceObjectFactory = spaceObjectFactory;
            _effectFactory = effectFactory;
            _prefabCache = prefabCache;
            _owner = owner;

            _prefab = new Lazy<GameObject>(() => _prefabCache.LoadPrefab(_stats.BulletPrefab) ?? (_stats.AmmunitionClass.IsBeam() ?
                _prefabCache.LoadResourcePrefab("Combat/Bullets/Laser") : _prefabCache.LoadResourcePrefab("Combat/Bullets/Plasma2")));

            _bulletStats = new BulletStatsObsolete(ammunitionStats);
        }

        public IBulletStats Stats { get { return _bulletStats; } }

        //public BulletType Type { get; private set; }
        //public BulletEffectType EffectType { get; private set; }

        //public float Range { get { return _stats.Range*RangeMultiplier; } }
        //public float Damage { get { return _stats.Damage*DamageMultiplier; } }
        //public float Size { get { return _stats.Size*SizeMultiplier; } }
        //public Color Color { get { return _stats.Color; } }
        //public float Lifetime { get { return _stats.LifeTime*LifetimeMultiplier; } }
        //public float Impulse { get { return _stats.Impulse*SizeMultiplier; } }
        //public float Recoil { get { return _stats.Recoil*SizeMultiplier; } }
        //public float AreaOfEffect { get { return _stats.AreaOfEffect*SizeMultiplier; } }
        //public float Velocity { get { return _stats.Velocity; } }
        //public float EnergyCost { get { return _stats.EnergyCost; } }
        //public bool IgnoresShipSpeed { get { return _stats.IgnoresShipSpeed; } }

        //public float PowerLevel { get; set; }
        //public float VelocitySpread { get; set; }
        //public float HitPointsMultiplier { get; set; }

        //private float RangeMultiplier { get { return PowerLevel > 0.1f ? PowerLevel : 0f; } }
        //private float LifetimeMultiplier { get { return 0.5f + PowerLevel*0.5f; } }
        //private float DamageMultiplier { get { return PowerLevel; } }
        //private float SizeMultiplier { get { return 0.5f + PowerLevel*0.5f; } }

        public IBullet Create(IWeaponPlatform parent, float spread, float rotation, float offset)
        {
            var color = _stats.AmmunitionClass == AmmunitionClassObsolete.Fireworks ? Color.Lerp(_stats.Color, new Color(Random.value, Random.value, Random.value), 0.75f) : (UnityEngine.Color)_stats.Color;
            var velocity = GetVelocity();

            var bulletGameObject = new GameObjectHolder(_prefab, _objectPool);
            bulletGameObject.IsActive = true;
            var body = ConfigureBody(bulletGameObject.GetComponent<IBodyComponent>(), parent, spread, velocity, rotation, offset);
            var view = ConfigureView(bulletGameObject.GetComponent<IView>(), color);

            var lifetime = _stats.Velocity > 0 && _bulletStats.Range > 0 ? _stats.AmmunitionClass.IsHoming() ? 1.2f*_bulletStats.Range/velocity : _bulletStats.Range/velocity : _bulletStats.Lifetime;
            var unitType = new UnitType(_stats.HitPoints > 0 ? UnitClass.Missile : _stats.AmmunitionClass.IsProjectile() ? UnitClass.EnergyBolt : UnitClass.AreaOfEffect, UnitSide.Undefined, _stats.AmmunitionClass.CanHitAllies() ? null : _owner);

            var bullet = new Bullet(body, view, new Lifetime(lifetime), unitType);
            bullet.AddResource(bulletGameObject);

            bullet.CanBeDisarmed = _stats.AmmunitionClass.CanBeDisarmed() && _bulletStats.Impulse < 0.5f;

            bullet.Collider = ConfigureCollider(bulletGameObject.GetComponent<ICollider>(), bullet);
            bullet.CollisionBehaviour = CreateCollisionBehaviour(_effectFactory, parent, color);

            bullet.Physics = bulletGameObject.GetComponent<PhysicsManager>();

            if (_stats.FireSound)
            {
                if (_stats.FireSound.Loop)
                    bullet.AddAction(new PlaySoundAction(_soundPlayer, _stats.FireSound, ConditionType.None));
                else
                    _soundPlayer.Play(_stats.FireSound);
            }

            bullet.Controller = CreateController(parent, bullet, spread, velocity, rotation);
            bullet.DamageHandler = CreateDamageHandler(bullet);
            CreateTriggers(bullet);

            _scene.AddUnit(bullet);
            bullet.UpdateView(0);

            if (!_stats.AmmunitionClass.IsBoundToCannon() && !_stats.IgnoresShipVelocity && _bulletStats.Recoil > 0)
                parent.Body.ApplyForce(bullet.Body.WorldPosition(), -_bulletStats.Recoil*(bullet.Body.WorldVelocity() - parent.Body.WorldVelocity()));

            return bullet;
        }

        private ICollider ConfigureCollider(ICollider collider, IUnit unit)
        {
            collider.Unit = unit;

            if (_stats.AmmunitionClass.IsBeam())
                collider.MaxRange = _bulletStats.Range;

            if (_stats.AmmunitionClass == AmmunitionClassObsolete.IonBeam)
                collider.MaxRange = _stats.Velocity / 10;

            //collider.IsTrigger = !_stats.AmmunitionClass.IsBeam();

            return collider;
        }

        private IView ConfigureView(IView view, Color color)
        {
            view.Life = 0;
            view.Color = color;

            view.UpdateView(0);
            return view;
        }

        private IBody ConfigureBody(IBodyComponent body, IWeaponPlatform parent, float spread, float bulletVelocity, float deltaAngle, float offset)
        {
            IBody parentBody = null;
            var position = Vector2.zero;
            var velocity = Vector2.zero;
            var rotation = deltaAngle;
            var angularVelocity = 0f;
            var weight = _bulletStats.Impulse;
            var scale = _bulletStats.Size;

            if (_stats.AmmunitionClass.IsBoundToCannon() && !parent.IsTemporary)
            {
                parentBody = parent.Body;
            }
            else
            {
                position = parent.Body.WorldPosition() + RotationHelpers.Direction(parent.Body.WorldRotation())*offset;
                rotation = parent.Body.WorldRotation() + deltaAngle + (Random.value - 0.5f)*spread;
            }

            if (!_stats.AmmunitionClass.IsBeam())
            {
                velocity = RotationHelpers.Direction(rotation) * bulletVelocity;
                rotation = parent.Body.WorldRotation() + deltaAngle + (Random.value - 0.5f) * spread;

                if (_stats.AmmunitionClass.HasEngine())
                    velocity *= 0.1f;

                if (!_stats.IgnoresShipVelocity)
                    velocity += parent.Body.WorldVelocity();
            }

            body.Initialize(parentBody, position, rotation, scale, velocity, angularVelocity, weight);
            return body;
        }

        private IDamageHandler CreateDamageHandler(Bullet bullet)
        {
            if (_stats.HitPoints > 0)
                return new MissileDamageHandler(bullet, _stats.HitPoints * _bulletStats.HitPointsMultiplier);
            else if (_stats.AmmunitionClass.IsBoundToCannon())
                return new BeamDamageHandler(bullet);
            else
                return new DefaultDamageHandler(bullet);
        }

        private IController CreateController(IWeaponPlatform parent, Bullet bullet, float spread, float velocity, float rotationOffset)
        {
            if (_stats.AmmunitionClass == AmmunitionClassObsolete.TractorBeam)
                return new TractorBeamController(bullet, _bulletStats.Range);
            if (_stats.AmmunitionClass.StickToTarget())
                return new LookAtTargetController(bullet, 60, spread);
            if (_stats.AmmunitionClass.IsHoming())
                return new HomingController(bullet, velocity, 120f / (0.2f + _bulletStats.Impulse *2), 0.5f * velocity / (0.2f + _bulletStats.Impulse *2), _bulletStats.Range, _scene);
            if (_stats.AmmunitionClass == AmmunitionClassObsolete.UnguidedRocket)
                return new RocketController(bullet, velocity, 1.0f * velocity / (0.1f + _bulletStats.Impulse));
            if (_stats.AmmunitionClass == AmmunitionClassObsolete.Aura)
                return new AuraController(bullet, _bulletStats.AreaOfEffect, _stats.LifeTime);
            if (_stats.AmmunitionClass.IsBoundToCannon() && !parent.IsTemporary)
                return new BeamController(bullet, spread, rotationOffset);

            return null;
        }

        private float GetVelocity()
        {
            return _bulletStats.RandomFactor > 0 ? _stats.Velocity*(1f + (Random.value - 0.5f)* _bulletStats.RandomFactor) : _stats.Velocity;
        }

        private BulletCollisionBehaviour CreateCollisionBehaviour(EffectFactory effectFactory, IWeaponPlatform platform, Color color)
        {
            var collisionBehaviour = new BulletCollisionBehaviour();

            if (_stats.HitEffectPrefab)
                collisionBehaviour.AddAction(new ShowHitEffectAction(effectFactory, _stats.HitEffectPrefab, color, _bulletStats.Size * 1.5f));

            if (_stats.AmmunitionClass.IsRepair())
                collisionBehaviour.AddAction(new RepairActionObsolete(_stats.DamageType, _bulletStats.Damage, platform.Type.Side));
            else if (_stats.AmmunitionClass == AmmunitionClassObsolete.BlackHole)
                collisionBehaviour.AddAction(new BlackHoleAction());
            else if (_stats.AmmunitionClass == AmmunitionClassObsolete.VampiricRay)
                collisionBehaviour.AddAction(new SiphonHitPointsAction(_stats.DamageType, _bulletStats.Damage, 1.0f, BulletImpactType.DamageOverTime));
            else if (_stats.AmmunitionClass == AmmunitionClassObsolete.SmallVampiricRay)
                collisionBehaviour.AddAction(new SiphonHitPointsAction(_stats.DamageType, _bulletStats.Damage, 0.1f, BulletImpactType.DamageOverTime));
            else if (_stats.AmmunitionClass.IsDot())
                collisionBehaviour.AddAction(new DealDamageAction(_stats.DamageType, _bulletStats.Damage, BulletImpactType.DamageOverTime));
            else if (_stats.AmmunitionClass.HasDirectDamage(_stats))
                collisionBehaviour.AddAction(new DealDamageAction(_stats.DamageType, _bulletStats.Damage, BulletImpactType.HitFirstTarget));

            if (_stats.AmmunitionClass == AmmunitionClassObsolete.EnergySiphon)
                collisionBehaviour.AddAction(new DrainEnergyAction(4*_stats.EnergyCost, BulletImpactType.DamageOverTime));

            if (_stats.AmmunitionClass.HasDirectImpulse() && _bulletStats.Impulse > 0)
                collisionBehaviour.AddAction(new StrikeAction(_owner != null ? _owner.Stats.WeaponDamageMultiplier : 1.0f, false));

            if (_stats.AmmunitionClass == AmmunitionClassObsolete.DroneControl)
                collisionBehaviour.AddAction(new CaptureDroneAction(BulletImpactType.HitFirstTarget));

            if (_stats.AmmunitionClass == AmmunitionClassObsolete.Immobilizer || _stats.AmmunitionClass == AmmunitionClassObsolete.HomingImmobilizer)
                collisionBehaviour.AddAction(new SlowDownAction(_bulletStats.Lifetime, BulletImpactType.HitFirstTarget));

            if (_stats.AmmunitionClass == AmmunitionClassObsolete.Singularity)
                collisionBehaviour.AddAction(new RollAction(_bulletStats.Lifetime));

            if (_stats.HitPoints > 0)
                collisionBehaviour.AddAction(new DetonateAtTargetAction());
            else if (!_stats.AmmunitionClass.IsDot())
                collisionBehaviour.AddAction(new SelfDestructAction());

            return collisionBehaviour;
        }

        private void CreateTriggers(Bullet bullet)
        {
            var explodeCondition = _stats.AmmunitionClass.DetonateIfExpired() ? ConditionType.OnExpire | ConditionType.OnDetonate : ConditionType.OnDetonate;
            if (_stats.HitSound)
                bullet.AddAction(new PlaySoundAction(_soundPlayer, _stats.HitSound, explodeCondition));

            if (_stats.AmmunitionClass.ExplodeIfDetonated(_stats))
                if (_stats.DamageType == DamageType.Impact)
                    bullet.AddAction(new CreateSmokeExplosionAction(bullet, _spaceObjectFactory, _stats.DamageType, _bulletStats.Damage, _bulletStats.AreaOfEffect, explodeCondition));
                else if (_stats.AreaOfEffect > 2f)
                    bullet.AddAction(new CreateExplosionAction(bullet, _spaceObjectFactory, _stats.DamageType, _bulletStats.Damage, _bulletStats.AreaOfEffect, explodeCondition));
                else
                    bullet.AddAction(new CreateFlashAction(bullet, _spaceObjectFactory, _stats.DamageType, _bulletStats.Damage, _bulletStats.AreaOfEffect, explodeCondition));

            if (_stats.AmmunitionClass == AmmunitionClassObsolete.Fireworks)
                bullet.AddAction(new CreateFlashAction(bullet, _spaceObjectFactory, _stats.DamageType, _bulletStats.Damage, _bulletStats.AreaOfEffect, explodeCondition));
            if (_stats.AmmunitionClass.AoeIfDetonated())
                bullet.AddAction(new CreateCloudAction(bullet, _spaceObjectFactory, _stats.DamageType, _bulletStats.Damage, _bulletStats.AreaOfEffect, _bulletStats.Lifetime, explodeCondition));
            if (_stats.AmmunitionClass.WebIfDetonated())
                bullet.AddAction(new CreatePlasmaWebAction(bullet, _spaceObjectFactory, _stats.DamageType, _bulletStats.Damage, _bulletStats.AreaOfEffect, _bulletStats.AreaOfEffect * 0.5f, _bulletStats.Lifetime, explodeCondition));
            if (_stats.AmmunitionClass == AmmunitionClassObsolete.BlackHole)
            {
                bullet.AddAction(new CreateGravitationAction(bullet, _spaceObjectFactory, _bulletStats.Range, 100, _bulletStats.Lifetime, ConditionType.None));
                bullet.AddAction(new CreateStrongExplosionAction(bullet, _spaceObjectFactory, _stats.DamageType, _bulletStats.Damage, _bulletStats.AreaOfEffect, explodeCondition));
            }
            if (_stats.AmmunitionClass == AmmunitionClassObsolete.Explosion)
                bullet.AddAction(new CreateStrongExplosionAction(bullet, _spaceObjectFactory, _stats.DamageType, _bulletStats.Damage, _bulletStats.AreaOfEffect, ConditionType.None));
            if (_stats.AmmunitionClass.IsCarrier() && _stats.CoupledAmmunition != null)
            {
                var stats = _stats.CoupledAmmunition.Stats;
                stats.Damage = _bulletStats.Damage;
                stats.DamageType = _stats.DamageType;
                var factory = new BulletFactoryObsolete(stats, _scene, _soundPlayer, _objectPool, _prefabCache, _spaceObjectFactory, _effectFactory, _owner);
                bullet.AddAction(new SpawnBulletsAction(factory, 1, 0, 0, bullet, _soundPlayer, AudioClipId.None, explodeCondition));
            }
            if (_stats.AmmunitionClass.IsClusterBomb() && _stats.CoupledAmmunition != null)
            {
                var stats = _stats.CoupledAmmunition.Stats;
                stats.DamageType = _stats.DamageType;
                stats.Damage = _bulletStats.Damage;
                stats.Range = _bulletStats.AreaOfEffect;
                var factory = new BulletFactoryObsolete(stats, _scene, _soundPlayer, _objectPool, _prefabCache, _spaceObjectFactory, _effectFactory, _owner);
                factory.Stats.RandomFactor = 0.75f;
                bullet.AddAction(new SpawnBulletsAction(factory, 20, 0.5f, 0, bullet, _soundPlayer, AudioClipId.None, explodeCondition));
            }
            if (_stats.AmmunitionClass.EmpIfDetonated())
            {
                var shieldDamage = _bulletStats.Damage * 10f;
                var energyDrain = _stats.EnergyCost * 10;
                bullet.AddAction(new CreateEmpAction(bullet, _spaceObjectFactory, _stats.DamageType, _bulletStats.Damage, shieldDamage, energyDrain, _bulletStats.AreaOfEffect, explodeCondition));
            }
        }

        private readonly Lazy<GameObject> _prefab;

        private readonly BulletStatsObsolete _bulletStats;
        private readonly AmmunitionObsoleteStats _stats;
        private readonly IScene _scene;
        private readonly ISoundPlayer _soundPlayer;
        private readonly IObjectPool _objectPool;
        private readonly SpaceObjectFactory _spaceObjectFactory;
        private readonly EffectFactory _effectFactory;
        private readonly PrefabCache _prefabCache;
        private readonly IShip _owner;
    }
}
