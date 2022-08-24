using Combat.Collision.Manager;
using Combat.Component.Body;
using Combat.Component.Unit;
using GameDatabase.Enums;
using UnityEngine;

namespace Combat.Collision.Behaviour.Action
{
    public class ApplyExplosionDamageAction : ICollisionAction
    {
        public ApplyExplosionDamageAction(DamageType damageType, float damage, float radius, float impulse)
        {
            _damageType = damageType;
            _damage = damage;
            _impulse = impulse;
            _maxDistance = radius;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (!collisionData.IsNew)
                return;

            var force = collisionData.Position.Direction(target.Body.WorldPosition());
            var multiplier = Mathf.Clamp01((_maxDistance + target.Body.WorldScale() - force.magnitude) / _maxDistance);
            if (multiplier <= 0)
                return;

            targetImpact.AddDamage(_damageType, _damage * multiplier);
            targetImpact.AddImpulse(target.Body.WorldPosition(), _impulse * multiplier * force.normalized);
        }

        public void Dispose() { }

        private readonly float _impulse;
        private readonly float _damage;
        private readonly float _maxDistance;
        private readonly DamageType _damageType;
    }
}
