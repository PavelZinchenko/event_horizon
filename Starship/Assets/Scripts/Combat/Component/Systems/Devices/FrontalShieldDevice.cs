using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.DataModel;

namespace Combat.Component.Systems.Devices
{
    public class FrontalShieldDevice : ContinuouslyActivatedDevice
    {
        public FrontalShieldDevice(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship, deviceSpec.Lifetime, deviceSpec.EnergyConsumption)
        {
            MaxCooldown = deviceSpec.Cooldown;
        }

        public override bool CanBeActivated =>
            base.CanBeActivated && (IsEnabled || Ship.Stats.Energy.Value >= EnergyCostInitial);

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnDispose() { }
    }
}
