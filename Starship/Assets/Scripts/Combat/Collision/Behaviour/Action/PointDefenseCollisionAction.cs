using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Factory;
using UnityEngine;

namespace Combat.Collision.Behaviour.Action
{
    public class PointDefenseCollisionAction : ICollisionAction
    {
        public PointDefenseCollisionAction(float damage, EffectFactory effectFactory, Color color)
        {
            _damage = damage;
            _color = color;
            _effectFactory = effectFactory;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (!collisionData.IsNew)
                return;

            if (target.Type.Side.IsAlly(self.Type.Side))
                return;

            switch (target.Type.Class)
            {
                case UnitClass.EnergyBolt:
                case UnitClass.Missile:
                    targetImpact.Effects |= CollisionEffect.Disarm;
                    break;
                case UnitClass.Drone:
                    targetImpact.EnergyDamage += _damage;
                    break;
                default:
                    return;
            }

            var effect = _effectFactory.CreateEffect("Lightning");
            var direction = self.Body.WorldPosition().Direction(target.Body.WorldPosition());
            effect.Color = _color;
            effect.Position = self.Body.WorldPosition();
            effect.Rotation = RotationHelpers.Angle(direction);
            effect.Size = direction.magnitude;
            effect.Run(0.1f, self.Body.WorldVelocity(), 0);

            selfImpact.Effects |= CollisionEffect.Activate;
        }

        public void Dispose() {}

        private readonly float _damage;
        private readonly Color _color;
        private readonly EffectFactory _effectFactory;
    }
}
