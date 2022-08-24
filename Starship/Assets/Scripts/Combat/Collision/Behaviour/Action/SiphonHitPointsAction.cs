using Combat.Collision.Manager;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using GameDatabase.Enums;

namespace Combat.Collision.Behaviour.Action
{
    public class SiphonHitPointsAction : ICollisionAction
    {
        public SiphonHitPointsAction(DamageType damageType, float damage, float conversionFactor, BulletImpactType impactType)
        {
            _impactType = impactType;
            _damageType = damageType;
            _damage = damage;
            _conversionFactor = conversionFactor;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact,
            ref Impact targetImpact)
        {
            float damage;
            if (_impactType == BulletImpactType.DamageOverTime)
            {
                damage = _damage * collisionData.TimeInterval;
                targetImpact.AddDamage(_damageType, damage);
            }
            else
            {
                if (!collisionData.IsNew || !_isAlive)
                    return;

                damage = _damage;
                targetImpact.AddDamage(_damageType, damage);

                _isAlive = _impactType == BulletImpactType.HitAllTargets;
            }

            if (target.Type.Class == UnitClass.Ship || target.Type.Class == UnitClass.Drone)
                selfImpact.Repair += damage * _conversionFactor;
        }

        public void Dispose() { }

        private bool _isAlive = true;
        private readonly float _damage;
        private readonly float _conversionFactor;
        private readonly DamageType _damageType;
        private readonly BulletImpactType _impactType;
    }
}
