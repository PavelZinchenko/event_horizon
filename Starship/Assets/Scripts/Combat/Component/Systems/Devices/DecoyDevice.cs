using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Factory;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class DecoyDevice : SystemBase, IDevice
    {
        public DecoyDevice(IShip ship, DeviceStats deviceSpec, float decoyHitPoints, int keyBinding, SpaceObjectFactory factory)
            : base(keyBinding, deviceSpec.ControlButtonIcon)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _energyCost = deviceSpec.EnergyConsumption;
            _lifetime = deviceSpec.Lifetime;
            _hitPoints = decoyHitPoints * deviceSpec.Size;
            _color = deviceSpec.Color;
            _factory = factory;
            _count = Mathf.RoundToInt(deviceSpec.Power);
            _range = deviceSpec.Range;
        }

        public override float ActivationCost { get { return _energyCost; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && _ship.Stats.Energy.Value >= _energyCost; } }

        public void Deactivate() {}

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && _ship.Stats.Energy.TryGet(_energyCost))
            {
                InvokeTriggers(ConditionType.OnActivate);
                TimeFromLastUse = 0;

                CreateDecoys();
            }
        }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnDispose() { }

        private void CreateDecoys()
        {
            for (var i = 0; i < _count; ++i)
            {
                var direction = RotationHelpers.Direction((90 + 180 * i) / _count + _ship.Body.Rotation + 90);
                _factory.CreateDecoy(_ship, _ship.Body.Position + direction, 0.5f, _ship.Body.Velocity + direction*_range, Random.Range(-90, 90), _hitPoints, _lifetime*(Random.value*0.5f + 0.75f) , _color);
            }
        }

        private readonly int _count;
        private readonly Color _color;
        private readonly float _range;
        private readonly float _lifetime;
        private readonly float _hitPoints;
        private readonly float _energyCost;
        private readonly IShip _ship;
        private readonly SpaceObjectFactory _factory;
    }
}
