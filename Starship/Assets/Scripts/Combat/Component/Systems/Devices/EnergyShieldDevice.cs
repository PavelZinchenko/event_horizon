using Combat.Component.Features;
using Combat.Component.Ship;
using GameDatabase.DataModel;

namespace Combat.Component.Systems.Devices
{
    public class EnergyShieldDevice : ContinuouslyActivatedDevice, IFeaturesModification
    {
        public EnergyShieldDevice(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding,
                deviceSpec.ControlButtonIcon,
                ship,
                deviceSpec.Lifetime,
                0,
                deviceSpec.EnergyConsumption)
        {
            MaxCooldown = deviceSpec.Cooldown;
        }

        public override IFeaturesModification FeaturesModification => this;

        public bool TryApplyModification(ref FeaturesData data)
        {
            data.Invulnerable |= IsEnabled;
            return true;
        }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnDispose() { }
    }
}
