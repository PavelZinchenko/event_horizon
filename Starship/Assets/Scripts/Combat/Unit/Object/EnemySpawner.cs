using Combat.Collision;
using Combat.Collision.Behaviour;
using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Factory;
using Combat.Scene;
using Combat.Unit.Ship.Effects.Special;
using Game.Exploration;
using UnityEngine;

namespace Combat.Unit.Object
{
    public class EnemySpawner : UnitBase
    {
        public EnemySpawner(Vector2 position, float activationDistance, float deactivationDistance,
            IScene scene, IEnemyShipBuilder shipBuilder, ShipFactory shipFactory, EffectFactory effectFactory, SpaceObjectFactory objectFactory, IUnitAction action = null)
            : base(new UnitType(UnitClass.AreaOfEffect, UnitSide.Neutral, null), new EmptyBody(null, position, 0, 1, 0, 0), null, null, null)
        {
            _scene = scene;
            _activationDistance = activationDistance;
            _deactivationDistance = deactivationDistance;
            _state = UnitState.Active;
            _shipBuilder = shipBuilder;
            _shipFactory = shipFactory;
            _objectFactory = objectFactory;
            _effectFactory = effectFactory;
            _unitAction = action;
        }

        public float RecoveryTıme { get; set; }
        public IUnit Parent { get; set; }

        public override ICollisionBehaviour CollisionBehaviour => null;
        public override UnitState State => _state;
        public override void OnCollision(Impact impact, IUnit target, CollisionData collisionData) { }
        public override void Vanish() { _state = UnitState.Inactive; }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            _cooldown -= elapsedTime;

            if (_ship != null)
            {
                switch (_ship.State)
                {
                    case UnitState.Active:
                        return;
                    case UnitState.Destroyed:
                        if (RecoveryTıme <= 0) Vanish();
                        _cooldown = RecoveryTıme;
                        _ship = null;
                        return;
                }
            }

            if (Parent != null && Parent.State != UnitState.Active)
            {
                Vanish();
                return;
            }

            if (_cooldown > 0) return;
            
            var player = _scene.PlayerShip;
            if (!player.IsActive())
                return;

            if (Body.Position.SqrDistance(player.Body.Position) > _activationDistance*_activationDistance)
                return;

            SpawnShip();
        }

        protected override void OnDispose() { }

        private void SpawnShip()
        {
            var ship = _shipBuilder.Build(_shipFactory, _objectFactory, Body.Position, RotationHelpers.Angle(Body.Position.Direction(_scene.PlayerShip.Body.Position)));
            if (_ship != null)
                ship.Stats.Armor.Get(_ship.Stats.Armor.MaxValue - _ship.Stats.Armor.Value);

            if (_unitAction != null)
                ship.AddTrigger(_unitAction);

            ship.AddEffect(new ShipVanishEffect(_deactivationDistance, _scene));

            _ship = ship;

            ShowSpawnEffect(ship);
        }

        private void ShowSpawnEffect(IShip ship)
        {
            var effect = _effectFactory.CreateEffect("OrbAdditive");
            effect.Position = ship.Body.Position;
            effect.Rotation = ship.Body.Rotation;
            effect.Size = ship.Body.Scale * 2;
            effect.Color = Color.cyan;
            effect.Run(0.5f, Vector2.zero, 0);
        }

        private float _cooldown;
        private UnitState _state;
        private IShip _ship;
        private readonly IUnitAction _unitAction;
        private readonly float _activationDistance;
        private readonly float _deactivationDistance;
        private readonly IScene _scene;
        private readonly SpaceObjectFactory _objectFactory;
        private readonly ShipFactory _shipFactory;
        private readonly EffectFactory _effectFactory;
        private readonly IEnemyShipBuilder _shipBuilder;
    }
}
