using Combat.Component.Engine;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.DataModel;

namespace Combat.Component.Systems.Devices
{
    public class AcceleratorDevice : SystemBase, IDevice, IEngineModification
    {
        public AcceleratorDevice(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _power = deviceSpec.Power / 20;

            _lifetime = deviceSpec.Lifetime;
            if (_lifetime == 0) _lifetime = float.MaxValue;
            _ship = ship;
            _energyCost = deviceSpec.EnergyConsumption;
        }

        public override IEngineModification EngineModification { get { return this; } }

        public bool TryApplyModification(ref EngineData data)
        {
            if (_isEnabled)
            {
                data.Throttle = 1.0f;
                data.Propulsion *= 4 * _power;
                data.Velocity *= 8 * _power;
            }

            return true;
        }

        public void Deactivate()
        {
            if (!_isEnabled)
                return;

            _isEnabled = false;
            TimeFromLastUse = 0;
            InvokeTriggers(ConditionType.OnDeactivate);
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && _ship.Stats.Energy.TryGet(_energyCost * elapsedTime))
            {
                if (!_isEnabled)
                {
                    InvokeTriggers(ConditionType.OnActivate);
                    _isEnabled = true;
                    _timeLeft = _lifetime;
                }
                else if (_timeLeft > 0)
                {
                    _timeLeft -= elapsedTime;
                    InvokeTriggers(ConditionType.OnRemainActive);
                }
                else
                {
                    Deactivate();
                }
            }
            else if (_isEnabled)
            {
                Deactivate();
            }
        }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnDispose() { }

        private float _power;
        private float _timeLeft;
        private bool _isEnabled;
        private readonly float _lifetime;
        private readonly float _energyCost;
        private readonly IShip _ship;
    }
}
