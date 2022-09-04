using Combat.Collision;
using Combat.Component.Body;
using Combat.Component.Engine;
using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Factory;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class DetonatorDevice : SystemBase, IDevice, IEngineModification, IFeaturesModification, ISystemsModification
    {
        public DetonatorDevice(IShip ship, DeviceStats deviceSpec, int keyBinding, SpaceObjectFactory factory, float damageMultiplier)
            : base(keyBinding, deviceSpec.ControlButtonIcon)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _energyCost = deviceSpec.EnergyConsumption;
            _color = deviceSpec.Color;
            _factory = factory;
            _lifetime = deviceSpec.Lifetime;
            _range = deviceSpec.Range * Mathf.Sqrt(_ship.Body.Scale);
            _damage = deviceSpec.Power*damageMultiplier;
            _offset = deviceSpec.Offset; 
        }

        public override float ActivationCost { get { return _energyCost; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && _ship.Stats.Energy.Value >= _energyCost; } }

        public override IEngineModification EngineModification { get { return this; } }
        public bool TryApplyModification(ref EngineData data)
        {
            if (_active)
            {
                data.TurnRate = 0;
                data.Propulsion = 0;
                data.Throttle = 0;
            }
            return true;
        }

        public override IFeaturesModification FeaturesModification { get { return this; } }
        public bool TryApplyModification(ref FeaturesData data)
        {
            if (_active)
            {
                var life = Mathf.Clamp01(_elapsedTime/_lifetime);
                data.Color = Color.Lerp(data.Color, _color, life);
            }
            return true;
        }

        public override ISystemsModification SystemsModification { get { return this; } }
        public bool IsAlive { get { return true; } }
        public bool CanActivateSystem(ISystem system) { return !_active; }
        public void OnSystemActivated(ISystem system) {}

        public void Deactivate() { }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (_active && _elapsedTime < _lifetime)
            {
                _elapsedTime += elapsedTime;
            }
            else if (_active)
            {
                _active = false;
                _factory.CreateStrongExplosion(
                    _ship.Body.WorldPosition() + RotationHelpers.Transform(_offset, _ship.Body.Rotation), _range,
                    DamageType.Heat,
                    _damage,
                    _ship.Type.Side,
                    _color,
                    1.0f,
                    0.1f * Mathf.Sqrt(_ship.Body.Weight)
                );
                TimeFromLastUse = 0;
                _ship.Affect(new Impact { Effects = CollisionEffect.Destroy });
            }
            else if (Active && CanBeActivated && _ship.Stats.Energy.TryGet(_energyCost))
            {
                _active = true;
                InvokeTriggers(ConditionType.OnActivate);
            }
        }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnDispose() { }

        private bool _active;
        private float _elapsedTime;
        private readonly float _lifetime;
        private readonly Color _color;
        private readonly float _damage;
        private readonly float _range;
        private readonly float _energyCost;
        private readonly IShip _ship;
        private readonly SpaceObjectFactory _factory;
        private readonly Vector2 _offset;
    }
}
