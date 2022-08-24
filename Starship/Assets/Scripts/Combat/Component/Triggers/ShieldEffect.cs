using Combat.Component.Ship;
using Combat.Effects;

namespace Combat.Component.Triggers
{
    public class ShieldEffect : IUnitEffect
    {
        public ShieldEffect(IEffect effect, IShip ship)
        {
            _effect = effect;
            _ship = ship;
        }

        public ConditionType TriggerCondition { get { return ConditionType.None; } }

        public bool TryUpdateEffect(float elapsedTime)
        {
            _effect.Life = _ship.Stats.Shield.Percentage;
            return true;
        }

        public bool TryInvokeEffect(ConditionType condition) { return true; }

        public void Dispose()
        {
            _effect.Dispose();
        }

        private readonly IEffect _effect;
        private readonly IShip _ship;
    }
}
