using Combat.Component.Body;
using Combat.Component.Unit;
using Combat.Factory;
using GameDatabase.Model;
using Services.Audio;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class ShipExplosionAction : IUnitAction
    {
        public ShipExplosionAction(IUnit unit, EffectFactory effectFactory, ISoundPlayer soundPlayer)
        {
            _factory = effectFactory;
            _soundPlayer = soundPlayer;
            _unit = unit;
        }

        public ConditionType TriggerCondition { get { return ConditionType.OnDestroy; } }

        public bool TryUpdateAction(float elapsedTime) { return false; }

        public bool TryInvokeAction(ConditionType condition)
        {
            var effect = _factory.CreateEffect("Wave");
            effect.Color = ColorTable.ShipExplosionColor;
            effect.Position = _unit.Body.Position;
            effect.Size = _unit.Body.Scale*6f;
            effect.Run(1.0f, _unit.Body.Velocity*0.05f, 0f);

            effect = _factory.CreateEffect("FlashAdditive");
            effect.Color = ColorTable.ShipExplosionColor;
            effect.Position = _unit.Body.Position;
            effect.Size = _unit.Body.Scale * 4f;
            effect.Run(1.0f, _unit.Body.Velocity * 0.05f, 0);

            effect = _factory.CreateEffect("SmokeAdditive");
            effect.Color = ColorTable.ShipExplosionColor;
            effect.Position = _unit.Body.Position;
            effect.Size = _unit.Body.Scale * 1.5f;
            effect.Run(1.0f, _unit.Body.Velocity * 0.05f, new System.Random().Next(-20, 20));

            effect = _factory.CreateEffect("Smoke");
            effect.Color = new Color(0.1f,0.1f,0.1f,0.5f);
            effect.Position = _unit.Body.Position;
            effect.Size = _unit.Body.Scale * 2.0f;
            effect.Run(3.0f, _unit.Body.Velocity * 0.05f, new System.Random().Next(-20, 20));

            _factory.CreateDisturbance(_unit.Body.WorldPosition(), _unit.Body.Scale * 10);

            _soundPlayer.Play(new AudioClipId("explosion_01"));

            return false;
        }

        public void Dispose() {}

        private readonly EffectFactory _factory;
        private readonly ISoundPlayer _soundPlayer;
        private readonly IUnit _unit;
    }
}
