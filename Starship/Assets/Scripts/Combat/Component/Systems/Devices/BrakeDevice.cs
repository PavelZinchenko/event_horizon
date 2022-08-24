using Combat.Component.Engine;
using Combat.Component.Ship;
using GameDatabase.DataModel;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class BrakeDevice : SystemBase, IDevice, IEngineModification
    {
        public BrakeDevice(IShip ship, DeviceStats deviceSpec, float shipWeight)
            : base(-1, SpriteId.Empty)
        {
            _power = deviceSpec.Power / Mathf.Sqrt(shipWeight);
            _ship = ship;
        }

        public override bool CanBeActivated { get { return false; } }

        public override IEngineModification EngineModification { get { return this; } }

        public bool TryApplyModification(ref EngineData data)
        {
            if (_active)
                data.Deceleration += _power;

            return true;
        }

        public void Deactivate() {}

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            _active = _ship.Engine.Throttle <= 0.001f;
        }

        protected override void OnUpdateView(float elapsedTime) { }
        protected override void OnDispose() { }

        private bool _active;
        private readonly float _power;
        private readonly IShip _ship;
    }
}
