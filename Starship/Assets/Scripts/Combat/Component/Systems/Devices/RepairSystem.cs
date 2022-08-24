using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.DataModel;

namespace Combat.Component.Systems.Devices
{
    public class RepairSystem : SystemBase, IDevice
    {
        public RepairSystem(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _energyCost = deviceSpec.EnergyConsumption;
        }

        public void Deactivate()
        {
            if (!_isActive)
                return;

            _isActive = false;
            TimeFromLastUse = 0;
            InvokeTriggers(ConditionType.OnDeactivate);
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && _ship.Stats.Energy.TryGet(_energyCost * elapsedTime))
            {
                if (!_isActive)
                {
                    InvokeTriggers(ConditionType.OnActivate);
                    _isActive = true;
                }
                else
                {
                    InvokeTriggers(ConditionType.OnRemainActive);
                }
            }
            else if (_isActive)
            {
                Deactivate();
            }
        }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnDispose() { }

        private bool _isActive;
        private readonly float _energyCost;
        private readonly IShip _ship;
    }
}
