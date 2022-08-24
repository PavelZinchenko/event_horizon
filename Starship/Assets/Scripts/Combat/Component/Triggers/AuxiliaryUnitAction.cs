using Combat.Component.Systems.Devices;
using Combat.Unit;
using Combat.Unit.Auxiliary;

namespace Combat.Component.Triggers
{
    public class AuxiliaryUnitAction : IUnitAction
    {
        public AuxiliaryUnitAction(IDevice device, IAuxiliaryUnit unit)
        {
            _unit = unit;
            _device = device;
            _unit.Enabled = false;
        }

        public ConditionType TriggerCondition { get { return ConditionType.OnActivate | ConditionType.OnDeactivate; } }

        public bool TryUpdateAction(float elapsedTime)
        {
            if (!_unit.IsActive() || !_unit.Enabled)
            {
                _device.Deactivate();
                return false;
            }

            return true;
        }

        public bool TryInvokeAction(ConditionType condition)
        {
            if (condition.Contains(ConditionType.OnDeactivate))
            {
                _unit.Enabled = false;
                return false;
            }

            if (condition.Contains(ConditionType.OnActivate))
            {
                _unit.Enabled = true;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (_unit.IsActive())
                _unit.Destroy();
        }

        private readonly IDevice _device;
        private readonly IAuxiliaryUnit _unit;
    }
}
