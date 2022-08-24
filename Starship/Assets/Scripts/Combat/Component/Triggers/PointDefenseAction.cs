using Combat.Component.Ship;
using Combat.Component.Systems.Devices;
using Combat.Factory;
using Combat.Unit;
using Combat.Unit.Auxiliary;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class PointDefenseAction : IUnitAction
    {
        public PointDefenseAction(IShip ship, IDevice device, float radius, float damage, float energyConsumption, float cooldown, Color color , SatelliteFactory factory, AudioClipId activationSound)
        {
            _factory = factory;
            _ship = ship;
            _color = color;
            _device = device;
            _damage = damage;
            _radius = radius;
            _cooldown = cooldown;
            _activationSound = activationSound;
            _energyConsumption = energyConsumption;
        }

        public ConditionType TriggerCondition { get { return ConditionType.OnActivate | ConditionType.OnDeactivate; } }

        public bool TryUpdateAction(float elapsedTime)
        {
            if (_pointDefense == null || !_pointDefense.Enabled)
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
                if (_pointDefense.IsActive())
                    _pointDefense.Enabled = false;

                return false;
            }

            if (condition.Contains(ConditionType.OnActivate))
            {
                if (_pointDefense == null)
                    _pointDefense = _factory.CreatePointDefense(_ship, _energyConsumption, _radius, _damage, _cooldown, _activationSound, _color);

                if (_pointDefense.IsActive())
                    _pointDefense.Enabled = true;

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (_pointDefense.IsActive())
                _pointDefense.Destroy();
        }

        private IAuxiliaryUnit _pointDefense;
        private readonly Color _color;
        private readonly AudioClipId _activationSound;
        private readonly float _radius;
        private readonly float _damage;
        private readonly float _energyConsumption;
        private readonly float _cooldown;
        private readonly IShip _ship;
        private readonly IDevice _device;
        private readonly SatelliteFactory _factory;
    }
}
