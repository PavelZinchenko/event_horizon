using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class TeleporterDevice : SystemBase, IDevice
    {
        public TeleporterDevice(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _offset = deviceSpec.Offset;
            if (_offset.x == 0 && _offset.y == 0) _offset = Vector2.right;
            _range = deviceSpec.Range;
            _energyCost = deviceSpec.EnergyConsumption;
        }

        public override float ActivationCost { get { return _energyCost; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && _ship.Stats.Energy.Value >= _energyCost; } }

        public void Deactivate() { }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && _ship.Stats.Energy.TryGet(_energyCost))
            {
                InvokeTriggers(ConditionType.OnActivate);

                _ship.Body.ShiftWithDependants(RotationHelpers.Transform(_offset, _ship.Body.Rotation) * _range);
                _ship.Body.ApplyAcceleration(-_ship.Body.Velocity);
                TimeFromLastUse = 0;

                InvokeTriggers(ConditionType.OnDeactivate);
            }
        }

        protected override void OnUpdateView(float elapsedTime) {}

        protected override void OnDispose() {}

        private readonly float _energyCost;
        private readonly float _range;
        private readonly IShip _ship;
        private readonly Vector2 _offset;
    }
}
