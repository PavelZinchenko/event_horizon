using Combat.Component.Body;
using Combat.Component.Unit;
using Combat.Factory;
using GameDatabase.Model;
using Services.Audio;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class AsteroidExplosionAction : IUnitAction
    {
        public AsteroidExplosionAction(IUnit unit, EffectFactory effectFactory, ISoundPlayer soundPlayer)
        {
            _factory = effectFactory;
            _soundPlayer = soundPlayer;
            _unit = unit;
        }

        public ConditionType TriggerCondition { get { return ConditionType.OnDestroy; } }

        public bool TryUpdateAction(float elapsedTime) { return false; }

        public bool TryInvokeAction(ConditionType condition)
        {
            var smokeEffect = _factory.CreateEffect("Smoke");
            smokeEffect.Color = Color.gray;
            smokeEffect.Position = _unit.Body.Position;
            smokeEffect.Rotation = Random.Range(0, 360);
            smokeEffect.Size = _unit.Body.Scale * 2f;
            smokeEffect.Run(2.0f, Vector2.zero, new System.Random().Next(-10, 10));

            smokeEffect = _factory.CreateEffect("SmokeAdditive");
            smokeEffect.Color = Color.gray;
            smokeEffect.Position = _unit.Body.Position;
            smokeEffect.Rotation = Random.Range(0, 360);
            smokeEffect.Size = _unit.Body.Scale * 1.5f;
            smokeEffect.Run(2.0f, Vector2.zero, new System.Random().Next(-10, 10));

            _factory.CreateDisturbance(_unit.Body.WorldPosition(), _unit.Body.Scale * 5);

            _soundPlayer.Play(new AudioClipId("explosion_08"));

            return false;
        }

        public void Dispose() { }

        private readonly EffectFactory _factory;
        private readonly ISoundPlayer _soundPlayer;
        private readonly IUnit _unit;
    }
}
