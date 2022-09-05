using Combat.Component.Engine;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.DataModel;

namespace Combat.Component.Systems.Devices
{
    public class AcceleratorDevice : ContinuouslyActivatedDevice, IEngineModification
    {
        public AcceleratorDevice(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding,
                deviceSpec.ControlButtonIcon, ship,
                deviceSpec.Lifetime,
                deviceSpec.EnergyConsumption)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _power = deviceSpec.Power / 20;
        }

        public override IEngineModification EngineModification => this;

        public bool TryApplyModification(ref EngineData data)
        {
            if (IsEnabled)
            {
                data.Throttle = 1.0f;
                data.Propulsion *= 4 * _power;
                data.Velocity *= 8 * _power;
            }

            return true;
        }

        protected override bool RemainActive(float elapsedTime)
        {
            if (!base.RemainActive(elapsedTime)) return false;
            InvokeTriggers(ConditionType.OnRemainActive);
            return true;
        }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnDispose() { }

        private readonly float _power;
    }
}
