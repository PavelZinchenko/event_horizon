using Combat.Component.Body;
using Combat.Factory;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class FlashMultipleEffect : IUnitEffect
    {
        public FlashMultipleEffect(PrefabId effectId, EffectFactory effectFactory, IBody body, float lifetime, float size, float velocityMultiplier, Color color, ConditionType conditionType)
        {
            _body = body;
            _effectId = effectId;
            _lifetime = lifetime;
            _color = color;
            _size = size;
            _effectFactory = effectFactory;
            _velocityMultiplier = velocityMultiplier;
            TriggerCondition = conditionType;
        }

        public ConditionType TriggerCondition { get; private set; }
        public bool TryUpdateEffect(float elapsedTime) { return false; }

        public bool TryInvokeEffect(ConditionType condition)
        {
            var effect = _effectFactory.CreateEffect(_effectId);
            effect.Position = _body.WorldPosition();
            effect.Rotation = _body.WorldRotation();
            effect.Size = _size;
            effect.Color = _color;
            effect.Run(_lifetime, _body.WorldVelocity()*_velocityMultiplier, _body.WorldAngularVelocity());
            return false;
        }

        public void Dispose() {}

        private readonly float _size;
        private readonly float _lifetime;
        private readonly float _velocityMultiplier;
        private readonly Color _color;
        private readonly IBody _body;
        private readonly PrefabId _effectId;
        private readonly EffectFactory _effectFactory;
    }
}
