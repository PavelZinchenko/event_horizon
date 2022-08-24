using Combat.Component.Body;
using Combat.Component.Systems.Weapons;
using Combat.Effects;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class PowerLevelEffect : IUnitEffect
    {
        public PowerLevelEffect(IWeapon weapon, IEffect effect, Vector2 position, float lifetime, ConditionType conditionType)
        {
            _effect = effect;
            _effect.Life = 0;
            _position = position;
            _weapon = weapon;
            _lifetime = lifetime;
            TriggerCondition = conditionType;
        }

        public ConditionType TriggerCondition { get; private set; }

        public bool TryUpdateEffect(float elapsedTime)
        {
            if (_weapon.PowerLevel <= 0 && _effect.Life <= 0)
                return false;

            var life = _weapon.PowerLevel;
            _effect.Life = _effect.Life < life ? life : Mathf.MoveTowards(_effect.Life, life, elapsedTime / _lifetime);

            var body = _weapon.Platform.Body;
            _effect.Rotation = body.WorldRotation();
            var position = body.WorldPosition() + RotationHelpers.Transform(_position, body.WorldRotation()) * body.WorldScale();

            _effect.Position = position;

            return true;
        }

        public bool TryInvokeEffect(ConditionType condition)
        {
            _effect.Life = _weapon.PowerLevel;
            return true;
        }

        public void Dispose()
        {
            _effect.Dispose();
        }

        private readonly Vector2 _position;
        private readonly float _lifetime;
        private readonly IEffect _effect;
        private readonly IWeapon _weapon;
    }
}
