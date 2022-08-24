using Combat.Effects;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class ConstantEffect : IUnitEffect
    {
        public ConstantEffect(IEffect effect)
        {
            _effect = effect;
        }

        public ConditionType TriggerCondition => ConditionType.None;

        public bool TryUpdateEffect(float elapsedTime) { return true; }
        public bool TryInvokeEffect(ConditionType condition) { return true; }

        public void Dispose()
        {
            _effect.Dispose();
        }

        private float _power;
        private readonly Color _color;
        private readonly IEffect _effect;
    }
}
