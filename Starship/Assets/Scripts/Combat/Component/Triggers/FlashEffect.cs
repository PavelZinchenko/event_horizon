using Combat.Component.Body;
using Combat.Effects;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class FlashEffect : IUnitEffect
    {
        public FlashEffect(IEffect effect, IBody body, float lifetime, Vector2 position, ConditionType conditionType)
        {
            _body = body;
            _lifetime = lifetime;
            _effect = effect;
            _effect.Life = 0;
            _position = position;
            TriggerCondition = conditionType;
        }

        public ConditionType TriggerCondition { get; private set; }

        public bool TryUpdateEffect(float elapsedTime)
        {
            if (_effect.Life <= 0)
                return false;

            _effect.Life -= elapsedTime/_lifetime;
            _effect.Rotation = _body.WorldRotation();

            var position = _body.WorldPosition() + _body.WorldVelocity()*elapsedTime;
            if (_position != Vector2.zero)
                position += RotationHelpers.Transform(_position, _body.WorldRotation())*_body.WorldScale();

            _effect.Position = position;

            return true;
        }

        public bool TryInvokeEffect(ConditionType condition)
        {
            _effect.Life = 1.0f;
            return true;
        }

        public void Dispose()
        {
            _effect.Dispose();
        }

        private Vector2 _position;
        private readonly float _lifetime;
        private readonly IBody _body;
        private readonly IEffect _effect;
    }
}
