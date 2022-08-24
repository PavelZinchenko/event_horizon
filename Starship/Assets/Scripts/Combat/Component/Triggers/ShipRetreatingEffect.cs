using Combat.Component.Body;
using Combat.Component.Unit;
using Combat.Effects;
using Combat.Factory;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class ShipRetreatingEffect : IUnitEffect
    {
        public ShipRetreatingEffect(IUnit unit, EffectFactory effectFactory, ConditionType startCondition, ConditionType stopCondition)
        {
            _startCondition = startCondition;
            _stopCondition = stopCondition;
            _factory = effectFactory;
            _unit = unit;
        }

        public ConditionType TriggerCondition { get { return _startCondition | _stopCondition; } }

        public bool TryUpdateEffect(float elapsedTime)
        {
            _elapsedTime += elapsedTime;
            _effect.Life = 0.5f + 0.25f*Mathf.Sin(_elapsedTime);
            _effect.Position = _unit.Body.WorldPosition();
            return true;
        }

        public bool TryInvokeEffect(ConditionType condition)
        {
            if (condition.Contains(_stopCondition))
            {
                if (_effect != null)
                    _effect.Dispose();
                _effect = null;
                return false;
            }

            if (condition.Contains(_startCondition))
            {
                if (_effect == null)
                {
                    _effect = _factory.CreateEffect("Wave");
                    _effect.Color = ColorTable.ShipWarpColor;
                    _effect.Position = _unit.Body.WorldPosition();
                    _effect.Size = _unit.Body.Scale*1.5f;
                }

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (_effect != null)
                _effect.Dispose();
            _effect = null;
        }

        private float _elapsedTime;
        private IEffect _effect;
        private readonly ConditionType _startCondition;
        private readonly ConditionType _stopCondition;
        private readonly EffectFactory _factory;
        private readonly IUnit _unit;
    }
}
