using Combat.Component.Body;
using Combat.Component.Unit;
using Combat.Factory;
using GameDatabase.Model;
using Services.Audio;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class DroneExplosionAction : IUnitAction
    {
        public DroneExplosionAction(IUnit unit, EffectFactory effectFactory, ISoundPlayer soundPlayer)
        {
            _factory = effectFactory;
            _soundPlayer = soundPlayer;
            _unit = unit;
        }

        public ConditionType TriggerCondition { get { return ConditionType.OnDestroy; } }

        public bool TryUpdateAction(float elapsedTime) { return false; }

        public bool TryInvokeAction(ConditionType condition)
        {
            var flash = _factory.CreateEffect("Flash");
            flash.Color = ColorTable.ShipExplosionColor;
            flash.Position = _unit.Body.Position;
            flash.Size = _unit.Body.Scale * 4f;
            flash.Run(0.3f, _unit.Body.Velocity * 0.05f, 0);

            var smokeEffect = _factory.CreateEffect("Smoke");
            smokeEffect.Color = Color.black;
            smokeEffect.Position = _unit.Body.Position;
            smokeEffect.Size = _unit.Body.Scale * 2f;
            smokeEffect.Run(1.0f, _unit.Body.Velocity * 0.05f, new System.Random().Next(-30, 30));

            _factory.CreateDisturbance(_unit.Body.WorldPosition(), _unit.Body.Scale*5);

            _soundPlayer.Play(new AudioClipId("explosion_10"));

            return false;
        }

        public void Dispose() { }

        private readonly EffectFactory _factory;
        private readonly ISoundPlayer _soundPlayer;
        private readonly IUnit _unit;
    }
}
