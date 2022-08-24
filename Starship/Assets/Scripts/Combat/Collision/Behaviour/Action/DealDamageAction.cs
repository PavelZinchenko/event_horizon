using Combat.Collision.Manager;
using Combat.Component.Unit;
using GameDatabase.Enums;

namespace Combat.Collision.Behaviour.Action
{
    public class DealDamageAction : ICollisionAction
    {
        public DealDamageAction(DamageType damageType, float damage, BulletImpactType impactType, bool ignoreDefenseBonus = false)
        {
            _ignoreDefenseBonus = ignoreDefenseBonus;
            _impactType = impactType;
            _damageType = damageType;
            _damage = damage;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            var damage = _ignoreDefenseBonus ? _damage * target.DefenseMultiplier : _damage;

            if (_impactType == BulletImpactType.DamageOverTime)
            {
                targetImpact.AddDamage(_damageType, damage * collisionData.TimeInterval);
            }
            else
            {
                if (!collisionData.IsNew || !_isAlive)
                    return;

                targetImpact.AddDamage(_damageType, damage);
                _isAlive = _impactType == BulletImpactType.HitAllTargets;
            }
        }

        public void Dispose() {}

        private bool _isAlive = true;
        private readonly float _damage;
        private readonly BulletImpactType _impactType;
        private readonly DamageType _damageType;
        private readonly bool _ignoreDefenseBonus;
    }
}
