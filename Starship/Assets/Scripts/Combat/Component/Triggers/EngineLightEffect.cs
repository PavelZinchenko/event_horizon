using Combat.Component.Ship;
using Combat.Effects;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class EngineLightEffect : IUnitEffect
    {
        public EngineLightEffect(IShip ship, IEffect effect)
        {
            _ship = ship;
            _effect = effect;
            _effect.Life = 0;
            _color = effect.Color;
        }

        public ConditionType TriggerCondition { get { return ConditionType.None; } }

        public bool TryUpdateEffect(float elapsedTime)
        {
            var power = _ship.Engine.Propulsion > 0.001 ? _ship.Engine.Throttle : 0f;
            _power = Mathf.Max(power, _power - 3*elapsedTime);

            _effect.Color = _ship.Features.Color*_color;
            _effect.Life = _power;

            return true;
        }

        public bool TryInvokeEffect(ConditionType condition) { return true; }

        public void Dispose()
        {
            _effect.Dispose();
        }

        private float _power;
        private readonly Color _color;
        private readonly IEffect _effect;
        private readonly IShip _ship;
    }
}
