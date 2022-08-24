using System.Collections.Generic;
using Combat.Collision.Behaviour;
using Combat.Collision.Behaviour.Action;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.Effector;
using Combat.Component.Physics;
using Combat.Component.Ship;
using Combat.Component.Stats;
using Combat.Component.Triggers;
using Combat.Component.View;
using Combat.Helpers;
using Combat.Scene;
using Combat.Unit;
using Combat.Unit.Object;
using Constructor;
using Constructor.Model;
using GameDatabase.Enums;
using GameDatabase.Model;
using Services.Audio;
using Services.ObjectPool;
using UnityEngine;
using Zenject;

namespace Combat.Factory
{
    public class SpaceObjectFactory
    {
        [Inject] private readonly IScene _scene;
        [Inject] private readonly IObjectPool _objectPool;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly EffectFactory _effectFactory;
        [Inject] private readonly PrefabCache _prefabCache;

        public IUnit CreateExplosion(Vector2 position, float radius, DamageType damageType, float damage, UnitSide unitSide, Color color, float impulseMultiplier = 1.0f)
        {
            const float lifetime = 0.5f;
            var unit = CreateArea(position, Vector2.zero, radius, 0.3f, unitSide);

            var effect = _effectFactory.CreateEffect("WaveThin");
            effect.Position = position;
            effect.Size = radius*1.2f;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);

            effect = _effectFactory.CreateEffect("FlashAdditive");
            effect.Position = position;
            effect.Size = radius*2;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);

            effect = _effectFactory.CreateEffect("FlashAdditive");
            effect.Position = position;
            effect.Size = radius;
            effect.Color = Color.white;
            effect.Run(lifetime, Vector2.zero, 0f);

            var impulse = radius * impulseMultiplier;

            var collisionBehaviour = new BulletCollisionBehaviour();
            collisionBehaviour.AddAction(new ApplyExplosionDamageAction(damageType, damage, radius, impulse));

            unit.SetCollisionBehaviour(collisionBehaviour);

            _effectFactory.CreateDisturbance(position, radius);

            _scene.AddUnit(unit);
            return unit;
        }

        public IUnit CreateSmokeExplosion(Vector2 position, float radius, DamageType damageType, float damage, UnitSide unitSide, Color color, float impulseMultiplier = 1.0f)
        {
            const float lifetime = 0.5f;
            var unit = CreateArea(position, Vector2.zero, radius, 0.3f, unitSide);

            var effect = _effectFactory.CreateEffect("FlashAdditive");
            effect.Position = position;
            effect.Size = radius * 2;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);

            effect = _effectFactory.CreateEffect("Smoke");
            effect.Position = position;
            effect.Size = radius * 1.5f;
            effect.Color = Color.black;
            effect.Run(lifetime, Vector2.zero, 0f);

            effect = _effectFactory.CreateEffect("SmokeAdditive");
            effect.Position = position;
            effect.Size = radius;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);

            var impulse = radius * impulseMultiplier;

            var collisionBehaviour = new BulletCollisionBehaviour();
            collisionBehaviour.AddAction(new ApplyExplosionDamageAction(damageType, damage, radius, impulse));

            unit.SetCollisionBehaviour(collisionBehaviour);

            _effectFactory.CreateDisturbance(position, radius);

            _scene.AddUnit(unit);
            return unit;
        }

        public IUnit CreateStrongExplosion(Vector2 position, float radius, DamageType damageType, float damage, UnitSide unitSide, Color color, float lifetime = 0.5f, float impulseMultiplier = 1.0f)
        {
            var unit = CreateArea(position, Vector2.zero, radius, 0.3f, unitSide);

            var effect = _effectFactory.CreateEffect("WaveStrong");
            effect.Position = position;
            effect.Size = radius*1.1f;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);

            effect = _effectFactory.CreateEffect("FlashAdditive");
            effect.Position = position;
            effect.Size = radius*2;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);

            effect = _effectFactory.CreateEffect("OrbAdditive");
            effect.Position = position;
            effect.Size = radius*2;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);

            var impulse = radius * impulseMultiplier;

            var collisionBehaviour = new BulletCollisionBehaviour();
            collisionBehaviour.AddAction(new ApplyExplosionDamageAction(damageType, damage, radius, impulse));

            unit.SetCollisionBehaviour(collisionBehaviour);

            _effectFactory.CreateDisturbance(position, 2*radius);

            _scene.AddUnit(unit);
            return unit;
        }

        public IUnit CreateFlash(Vector2 position, float radius, DamageType damageType, float damage, UnitSide unitSide, Color color, float impulseMultiplier = 1.0f)
        {
            const float lifetime = 0.3f;
            var unit = CreateArea(position, Vector2.zero, radius, lifetime, unitSide);

            var effect = _effectFactory.CreateEffect("Flash");
            effect.Position = position;
            effect.Size = 2*radius;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);

            effect = _effectFactory.CreateEffect("FlashAdditive");
            effect.Position = position;
            effect.Size = radius;
            effect.Color = Color.gray;
            effect.Run(lifetime, Vector2.zero, 0f);

            var impulse = radius * impulseMultiplier;

            var collisionBehaviour = new BulletCollisionBehaviour();
            collisionBehaviour.AddAction(new ApplyExplosionDamageAction(damageType, damage, radius, impulse));

            unit.SetCollisionBehaviour(collisionBehaviour);

            _effectFactory.CreateDisturbance(position, radius/2);

            _scene.AddUnit(unit);
            return unit;
        }

        public IUnit CreateWeb(Vector2 position, float radius, float lifetime, DamageType damageType, float damage, float deceleration, UnitSide unitSide, Color color)
        {
            var unit = CreateDeceleratingArea(position, Vector2.zero, radius, deceleration, lifetime, unitSide);

            var effect = _effectFactory.CreateEffect("EnergyField");
            effect.Position = position;
            effect.Size = radius * 2.5f;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);

            var collisionBehaviour = new BulletCollisionBehaviour();
            collisionBehaviour.AddAction(new DealDamageAction(damageType, damage, BulletImpactType.DamageOverTime));

            unit.SetCollisionBehaviour(collisionBehaviour);

            _scene.AddUnit(unit);
            return unit;
        }

        public IUnit CreateDecoy(IShip parent, Vector2 position, float size, Vector2 velocity, float angularVelocity, float hitPoints, float lifetime, Color color)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Objects/Decoy");
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            gameObject.IsActive = true;
            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(null, position, 0f, size, velocity, angularVelocity, 0.1f);
            var collider = gameObject.GetComponent<ICollider>();
            var view = gameObject.GetComponent<IView>();
            view.Color = color;
            var unit = new Decoy(parent, body, view, collider, hitPoints, lifetime);
            unit.AddResource(gameObject);

            var effect = _effectFactory.CreateEffect("FlashAdditive");
            effect.Position = position;
            effect.Size = size*4;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);
            effect.Run(0.3f, parent.Body.Velocity, 0);

            unit.AddTrigger(new DroneExplosionAction(unit, _effectFactory, _soundPlayer));

            _scene.AddUnit(unit);
            return unit;
        }

        public IEnumerable<WormSegment> CreateWormTail(IShip parent, int size, float weightScale, float hitPoints, PrefabId prefabId, float offset1, float offset2, float extraOffset, ColorScheme colorScheme)
        {
            var prefab = _prefabCache.LoadPrefab(prefabId);
            var damageIndicator = new DamageIndicator(parent, _effectFactory, parent.Type.Side == UnitSide.Player ? 0.75f : 0.5f);

            WormSegment segment = null;
            for (var i = 0; i < size; ++i)
            {
                var parentBody = segment != null ? segment.Body : parent.Body;
                var parentWeight = segment != null ? segment.Body.Weight * 0.95f : parent.Body.Weight * weightScale;

                var gameObject = new GameObjectHolder(prefab, _objectPool);
                gameObject.IsActive = true;
                var body = gameObject.GetComponent<IBodyComponent>();
                var position = parentBody.Position - RotationHelpers.Direction(parentBody.Rotation) * parentBody.Scale;
                body.Initialize(null, position, parentBody.Rotation, parentBody.Scale * 0.95f, Vector2.zero, 0.0f, parentWeight * 0.95f);
                var physics = gameObject.GetComponent<PhysicsManager>();
                var collider = gameObject.GetComponent<ICollider>();
                var view = gameObject.GetComponent<IView>();
                view.ApplyHsv(colorScheme.Hue, colorScheme.Saturation);
                var unit = new WormSegment(parent, body, view, collider, physics, hitPoints *= 0.95f);
                unit.AddResource(gameObject);
                unit.AddTrigger(new DroneExplosionAction(unit, _effectFactory, _soundPlayer));

                if (segment != null)
                {
                    unit.Connect(segment, offset1, offset2, 30f);
                    unit.SetDamageIndicator(damageIndicator, false);
                }
                else
                {
                    unit.Connect(parent, offset1 + extraOffset, offset2, 60f);
                    unit.SetDamageIndicator(damageIndicator, true);
                }

                segment = unit;
                _scene.AddUnit(unit);

                yield return unit;
            }
        }

        public ShortLivedObject CreateGravitation(IUnit parent, float radius, float lifetime, float gravitation)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Objects/Gravitation");
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            gameObject.IsActive = true;
            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(parent.Body, Vector2.zero, 0f, 2*radius/parent.Body.Scale, Vector2.zero, 0f, 1.0f);
            var collider = gameObject.GetComponent<ICollider>();
            var unit = new ShortLivedObject(body, new EmptyView(), collider, lifetime, new UnitType(UnitClass.AreaOfEffect, parent.Type.Side, parent.Type.Owner));
            unit.AddResource(gameObject);
            unit.Parent = parent;

            var effector = gameObject.GetComponent<IEffector>();
            effector.Initialize(unit.Type.CollisionMask, gravitation);

            _scene.AddUnit(unit);

            return unit;
        }

        public IUnit CreateEmp(Vector2 position, float radius, DamageType damageType, float damage, float shieldDamage, float energyDrain, UnitSide unitSide, Color color)
        {
            const float lifetime = 0.5f;
            var unit = CreateArea(position, Vector2.zero, radius, lifetime, unitSide);

            var effect = _effectFactory.CreateEffect("FlashAdditive");
            effect.Position = position;
            effect.Size = radius*4;
            effect.Color = color;
            effect.Run(lifetime, Vector2.zero, 0f);

            effect = _effectFactory.CreateEffect("FlashAdditive");
            effect.Position = position;
            effect.Size = radius*2;
            effect.Color = Color.white;
            effect.Run(lifetime, Vector2.zero, 0f);

            var collisionBehaviour = new BulletCollisionBehaviour();
            collisionBehaviour.AddAction(new ApplyExplosionDamageAction(damageType, damage, radius, 0));
            collisionBehaviour.AddAction(new DrainEnergyAction(energyDrain, BulletImpactType.HitAllTargets));
            collisionBehaviour.AddAction(new DamageShieldAction(shieldDamage, true));

            unit.SetCollisionBehaviour(collisionBehaviour);

            _scene.AddUnit(unit);
            return unit;
        }

        public IUnit CreateCloud(Vector2 position, Vector2 velocity, float lifetime, float radius, DamageType damageType, float damage, UnitSide unitSide, Color color)
        {
            var unit = CreateArea(position, velocity, radius, lifetime, unitSide);

            var effect = _effectFactory.CreateEffect("SmokeAdditive");
            color.a *= 0.5f;
            effect.Color = color;
            effect.Position = unit.Body.Position;
            effect.Size = 2.6f*radius;
            effect.Run(lifetime, velocity, new System.Random().Next(-20, 20));

            effect = _effectFactory.CreateEffect("OrbAdditive");
            effect.Position = position;
            effect.Size = 2*radius;
            effect.Color = color;
            effect.Run(0.3f, Vector2.zero, 0f);

            var collisionBehaviour = new BulletCollisionBehaviour();
            collisionBehaviour.AddAction(new DealDamageAction(damageType, damage, BulletImpactType.DamageOverTime));

            unit.SetCollisionBehaviour(collisionBehaviour);

            _scene.AddUnit(unit);
            return unit;
        }

        public Asteroid CreateAsteroid(Vector2 position, Vector2 velocity, float size, float weight, float hitPoints, float damageMultiplier)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Objects/Asteroid");
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(null, position, 0f, size, velocity, 0f, weight);
            var collider = gameObject.GetComponent<ICollider>();
            var view = gameObject.GetComponent<IView>();
            var physics = gameObject.GetComponent<PhysicsManager>();
            var unit = new Asteroid(new UnitType(UnitClass.SpaceObject, UnitSide.Undefined, null), body, view, collider, physics, hitPoints, damageMultiplier);
            unit.AddResource(gameObject);
            
            unit.AddTrigger(new AsteroidExplosionAction(unit, _effectFactory, _soundPlayer));

            gameObject.IsActive = true;
            _scene.AddUnit(unit);

            return unit;
        }

        public IUnit CreateCamera(Vector2 position, float size)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Objects/Camera");
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(null, position, 0f, size, Vector2.zero, 0f, 0f);
            var unit = new CameraUnit(body);
            unit.AddResource(gameObject);

            gameObject.IsActive = true;
            _scene.AddUnit(unit);

            return unit;
        }

        private ShortLivedObject CreateArea(Vector2 position, Vector2 velocity, float radius, float lifetime, UnitSide unitSide)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Objects/Area");
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(null, position, 0f, 2*radius, velocity, 0f, 1.0f);
            var collider = gameObject.GetComponent<ICollider>();
            var unit = new ShortLivedObject(body, new EmptyView(), collider, lifetime, new UnitType(UnitClass.AreaOfEffect, unitSide, null));
            unit.AddResource(gameObject);
            gameObject.IsActive = true;

            return unit;
        }

        private ShortLivedObject CreateDeceleratingArea(Vector2 position, Vector2 velocity, float radius, float deceleration, float lifetime, UnitSide unitSide)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Objects/DeceleratingField");
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            gameObject.IsActive = true;
            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(null, position, 0f, 2*radius, velocity, 0f, 1.0f);
            var collider = gameObject.GetComponent<ICollider>();
            var unit = new ShortLivedObject(body, new EmptyView(), collider, lifetime, new UnitType(UnitClass.AreaOfEffect, unitSide, null));
            unit.AddResource(gameObject);

            var effector = gameObject.GetComponent<IEffector>();
            effector.Initialize(unit.Type.CollisionMask, deceleration);

            return unit;
        }

        public SpaceObject CreatePlanet(Vector2 position, float size, Color color)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Objects/Planet");
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            var view = gameObject.GetComponent<IView>();
            view.Color = color;
            return CreateSpaceObject(gameObject, view, position, 0, size);
        }

        private SpaceObject CreateSpaceObject(GameObjectHolder gameObject, IView view, Vector2 position, float rotation, float size, float angularVelocity = 0, ICollisionBehaviour collisionBehaviour = null)
        {
            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(null, position, rotation, size, Vector2.zero, angularVelocity, 100000.0f);
            var collider = gameObject.GetComponent<ICollider>();
            var unit = new SpaceObject(new UnitType(UnitClass.BackgroundObject, UnitSide.Undefined, null), body, view, collider, collisionBehaviour);
            unit.AddResource(gameObject);
            gameObject.IsActive = true;
            _scene.AddUnit(unit);
            return unit;
        }

        public SpaceObject CreatePlanetaryObject(string prefabId, Vector2 position, float rotation, float angularVelocity, float size, Color color)
        {
            var prefab = _prefabCache.LoadResourcePrefab(prefabId);
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            var view = gameObject.GetComponent<IView>();
            view.Color = color;
            return CreateSpaceObject(gameObject, view, position, rotation, size, angularVelocity, new DockingStationCollisionBehaviour());

        }

        public SpaceObject CreatePlanetaryGasCloud(Vector2 position, float rotation, float angularVelocity, float size, Color color)
        {
            return CreatePlanetaryObject("Combat/Exploration/GasCloud", position, rotation, angularVelocity, size, color);
        }

        public SpaceObject CreatePlanetaryCrater(Vector2 position, float rotation, float size, Color color)
        {
            return CreatePlanetaryObject("Combat/Exploration/Crater", position, rotation, 0, size, color);
        }

        public SpaceObject CreatePlanetaryCrater2(Vector2 position, float rotation, float size, Color color)
        {
            return CreatePlanetaryObject("Combat/Exploration/Crater2", position, rotation, 0, size, color);
        }

        public SpaceObject CreatePlanetaryVolcano(Vector2 position, float rotation, float size, Color color)
        {
            return CreatePlanetaryObject("Combat/Exploration/Volcano", position, rotation, 0, size, color);
        }

        public SpaceObject CreatePlanetaryVolcano2(Vector2 position, float rotation, float size, Color color)
        {
            return CreatePlanetaryObject("Combat/Exploration/Volcano2", position, rotation, 0, size, color);
        }

        public SpaceObject CreatePlanetaryVortex(Vector2 position, float size, Color color)
        {
            return CreatePlanetaryObject("Combat/Exploration/Vortex", position, 0, 0, size, color);
        }

        public SpaceObject CreatePlanetaryContainer(Vector2 position, float rotation, float size, Color color)
        {
            return CreatePlanetaryObject("Combat/Exploration/Container", position, rotation, 0, size, color);
        }

        public SpaceObject CreatePlanetaryFloatingContainer(Vector2 position, float size, Color color)
        {
            return CreatePlanetaryObject("Combat/Exploration/Box", position, 0, 0, size, color);
        }

        public SpaceObject CreatePlanetaryRock(Vector2 position, float size, Color color)
        {
            return CreatePlanetaryObject("Combat/Exploration/Rock", position, 0, 0, size, color);
        }

        public SpaceObject CreatePlanetaryFloatingRock(Vector2 position, float size, Color color)
        {
            return CreatePlanetaryObject("Combat/Exploration/Asteroid", position, 0, 0, size, color);
        }

        public SpaceObject CreatePlanetaryShipWreck(Sprite sprite, Vector2 position, float rotation, float angularVelocity, float size, Color color)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Exploration/ShipWreck");
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            gameObject.GetComponent<SpriteRenderer>(true).sprite = sprite;
            var view = gameObject.GetComponent<IView>();
            view.Color = color;

            return CreateSpaceObject(gameObject, view, position, rotation, size, angularVelocity, new DockingStationCollisionBehaviour());
        }

        public SpaceObject CreatePlanetaryGasCloud(Vector2 position, float rotation, float angularVelocity, float size, Color color, float dps)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Exploration/GasCloud");
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(null, position, rotation, size, Vector2.zero, angularVelocity, 0);
            var collider = gameObject.GetComponent<ICollider>();
            var view = gameObject.GetComponent<IView>();
            view.Color = color;

            var collisionBehaviour = new BulletCollisionBehaviour();
            collisionBehaviour.AddAction(new DealDamageAction(DamageType.Direct, dps, BulletImpactType.DamageOverTime, true));

            var unit = new SpaceObject(new UnitType(UnitClass.AreaOfEffect, UnitSide.Enemy, null), body, view, collider, collisionBehaviour);
            unit.AddResource(gameObject);

            gameObject.IsActive = true;
            _scene.AddUnit(unit);

            return unit;
        }
    }
}
