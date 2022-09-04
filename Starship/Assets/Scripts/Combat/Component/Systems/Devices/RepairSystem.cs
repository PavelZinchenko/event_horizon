using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.DataModel;

namespace Combat.Component.Systems.Devices
{
    public class RepairSystem : ContinuouslyActivatedDevice
    {
        public RepairSystem(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship, deviceSpec.Lifetime, deviceSpec.EnergyConsumption)
        {
            MaxCooldown = deviceSpec.Cooldown;
        }

        protected override bool RemainActive(float elapsedTime)
        {
            if (!base.RemainActive(elapsedTime)) return false;
            InvokeTriggers(ConditionType.OnRemainActive);
            return true;
        }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnDispose() { }
    }
}
