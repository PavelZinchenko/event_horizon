using Combat.Component.Unit;
using Combat.Factory;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class ShipWreckAction : IUnitAction
    {
        public ShipWreckAction(IUnit unit, EffectFactory effectFactory, Sprite objectSprite, Color color, bool staticWreck)
        {
            _factory = effectFactory;
            _objectSprite = objectSprite;
            _color = color;
            _unit = unit;
            _staticWreck = staticWreck;
        }

        public ConditionType TriggerCondition => ConditionType.OnDestroy;

        public bool TryUpdateAction(float elapsedTime) { return false; }

        public bool TryInvokeAction(ConditionType condition)
        {
            var effect = _factory.CreateEffect("Wreck", _objectSprite);
            effect.Position = _unit.Body.Position;
            effect.Rotation = _unit.Body.Rotation;
            effect.Size = _unit.Body.Scale;
            effect.Color = Color.Lerp(Color.black, _color, 0.5f);
            if (_staticWreck)
                effect.Run(60, Vector2.zero, 0);
            else
                effect.Run(60, _unit.Body.Velocity * 0.2f, _unit.Body.AngularVelocity);

            return false;
        }

        public void Dispose() {}

        private readonly bool _staticWreck;
        private readonly EffectFactory _factory;
        private readonly Sprite _objectSprite;
        private readonly Color _color;
        private readonly IUnit _unit;
    }
}
