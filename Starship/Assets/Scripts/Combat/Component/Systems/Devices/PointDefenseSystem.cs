using Combat.Component.Ship;
using GameDatabase.DataModel;

namespace Combat.Component.Systems.Devices
{
    public class PointDefenseSystem : ContinuouslyActivatedDevice
    {
        // TODO: This should use lifetime too, but that requires breaking change to the vanilla DB
        public PointDefenseSystem(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship, 0, 0, deviceSpec.EnergyConsumption)
        {
            MaxCooldown = deviceSpec.Cooldown;
        }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnDispose() { }
    }
}
