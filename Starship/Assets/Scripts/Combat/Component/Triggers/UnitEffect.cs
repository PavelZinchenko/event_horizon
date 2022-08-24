using Combat.Effects;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class UnitEffect : IUnitEffect
    {
        public UnitEffect(IEffect effect, float lifetime, ConditionType activateCondition, ConditionType deactivateCondition)
        {
            _lifetime = lifetime;
            _effect = effect;
            _effect.Life = 0;
            _activateCondition = activateCondition;
            _deactivateCondition = deactivateCondition;
        }

        public ConditionType TriggerCondition { get { return _activateCondition | _deactivateCondition; } }

        public bool TryUpdateEffect(float elapsedTime)
        {
            if (!_isActive && _effect.Life <= 0)
                return false;

            _effect.Life = Mathf.Clamp01(_isActive ? _effect.Life + elapsedTime/_lifetime : _effect.Life - elapsedTime/_lifetime);
            return true;
        }

        public bool TryInvokeEffect(ConditionType condition)
        {
            if (condition.Contains(_deactivateCondition))
                _isActive = false;
            else if (condition.Contains(_activateCondition))
                _isActive = true;

            return true;
        }

        public void Dispose()
        {
            _effect.Dispose();
        }

        private bool _isActive;
        private readonly float _lifetime;
        private readonly IEffect _effect;
        private readonly ConditionType _activateCondition;
        private readonly ConditionType _deactivateCondition;
    }
}
