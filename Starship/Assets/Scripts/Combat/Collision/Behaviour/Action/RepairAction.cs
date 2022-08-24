using Combat.Collision.Manager;
using Combat.Component.Unit;
using GameDatabase.Enums;

namespace Combat.Collision.Behaviour.Action
{
    public class RepairAction : ICollisionAction
    {
        public RepairAction(float power, BulletImpactType impactType)
        {
            _power = power;
            _impactType = impactType;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (_impactType == BulletImpactType.DamageOverTime)
            {
                targetImpact.Repair += _power * collisionData.TimeInterval;
            }
            else
            {
                if (!collisionData.IsNew || !_isAlive)
                    return;

                targetImpact.Repair += _power;
                _isAlive = _impactType == BulletImpactType.HitAllTargets;
            }
        }

        public void Dispose() { }

        private bool _isAlive = true;
        private readonly float _power;
        private readonly BulletImpactType _impactType;
    }
}
