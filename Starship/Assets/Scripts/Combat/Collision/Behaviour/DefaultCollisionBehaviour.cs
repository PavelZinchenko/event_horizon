using Combat.Collision.Manager;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using GameDatabase.Enums;
using UnityEngine;

namespace Combat.Collision.Behaviour
{
    public class DefaultCollisionBehaviour : ICollisionBehaviour
    {
        public DefaultCollisionBehaviour(float rammingDamageMultiplier)
        {
            _rammingDamageMultiplier = rammingDamageMultiplier;
        }

        public void Process(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (collisionData.IsNew && self.Type.Side.IsEnemy(target.Type.Side))
            {
                var impulse = Mathf.Min(self.Body.Weight, target.Body.Weight);
                var damage = 0.1f * collisionData.RelativeVelocity.magnitude * impulse * _rammingDamageMultiplier;

                if (damage > 1)
                    targetImpact.AddDamage(DamageType.Impact, damage);
            }
        }

        public void Dispose()
        {
        }

        private readonly float _rammingDamageMultiplier;
    }
}
