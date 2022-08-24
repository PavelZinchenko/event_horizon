using Combat.Component.Unit;
using Combat.Factory;
using Services.Audio;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class ShipWarpEffect : IUnitEffect
    {
        public ShipWarpEffect(IUnit unit, EffectFactory effectFactory, ISoundPlayer soundPlayer, AudioClip sound, ConditionType condition)
        {
            TriggerCondition = condition;
            _factory = effectFactory;
            _soundPlayer = soundPlayer;
            _sound = sound;
            _unit = unit;
        }

        public ConditionType TriggerCondition { get; private set; }

        public bool TryUpdateEffect(float elapsedTime) { return false; }

        public bool TryInvokeEffect(ConditionType condition)
        {
            var effect = _factory.CreateEffect("Wave");
            effect.Color = ColorTable.ShipWarpColor;
            effect.Position = _unit.Body.Position;
            effect.Size = _unit.Body.Scale * 6f;
            effect.Run(1.0f, _unit.Body.Velocity * 0.05f, 0f);

            effect = _factory.CreateEffect("FlashAdditive");
            effect.Color = ColorTable.ShipWarpColor;
            effect.Position = _unit.Body.Position;
            effect.Size = _unit.Body.Scale * 5f;
            effect.Run(1.0f, _unit.Body.Velocity * 0.05f, 0);

            effect = _factory.CreateEffect("FlashAdditive");
            effect.Color = Color.white;
            effect.Position = _unit.Body.Position;
            effect.Size = _unit.Body.Scale * 4f;
            effect.Run(1.0f, _unit.Body.Velocity * 0.05f, 0);

            _soundPlayer.Play(_sound);

            return false;
        }

        public void Dispose() { }

        private readonly EffectFactory _factory;
        private readonly ISoundPlayer _soundPlayer;
        private readonly AudioClip _sound;
        private readonly IUnit _unit;
    }
}
