using Combat.Component.Ship;
using Combat.Component.Systems;
using Combat.Factory;
using Combat.Unit;
using Combat.Unit.Auxiliary;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class RepairBotAction : IUnitAction
    {
        public RepairBotAction(IShip ship, ISystem device, SatelliteFactory factory, float repairRate, float size, Color color, AudioClipId activationSound)
        {
            _factory = factory;
            _ship = ship;
            _size = size;
            _color = color;
            _repairRate = repairRate;
            _repairBotDevice = device;
            _activationSound = activationSound;
        }

        public ConditionType TriggerCondition { get { return ConditionType.OnActivate | ConditionType.OnDeactivate; } }

        public bool TryUpdateAction(float elapsedTime)
        {
            if (_repairBot.State == UnitState.Destroyed)
            {
                _repairBotDevice.Enabled = false;
                return false;
            }

            return _repairBot.State == UnitState.Active;
        }

        public bool TryInvokeAction(ConditionType condition)
        {
            if (condition.Contains(ConditionType.OnDeactivate))
            {
                if (_repairBot.IsActive())
                    _repairBot.Enabled = false;
            }
            else if (condition.Contains(ConditionType.OnActivate))
            {
                if (_repairBot == null || _repairBot.State == UnitState.Inactive)
                    _repairBot = _factory.CreateRepairBot(_ship, _repairRate, _size, _size, 1f, _color, _activationSound);

                if (_repairBot.IsActive())
                    _repairBot.Enabled = true;

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (_repairBot.IsActive())
                _repairBot.Destroy();
        }

        private IAuxiliaryUnit _repairBot;
        private readonly ISystem _repairBotDevice;
        private readonly float _repairRate;
        private readonly Color _color;
        private readonly AudioClipId _activationSound;
        private readonly float _size;
        private readonly IShip _ship;
        private readonly SatelliteFactory _factory;
    }
}
