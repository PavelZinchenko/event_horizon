using Combat.Collision.Manager;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using GameDatabase.Enums;

namespace Combat.Collision.Behaviour.Action
{
    public class CaptureDroneAction : ICollisionAction
    {
        public CaptureDroneAction(BulletImpactType impactType)
        {
            _impactType = impactType;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (!_isAlive || !collisionData.IsNew)
                return;

            if (target.Type.Class != UnitClass.Drone && target.Type.Class != UnitClass.Missile)
                return;

            target.Type.Owner = self.Type.Owner;

            _isAlive = _impactType == BulletImpactType.DamageOverTime || _impactType == BulletImpactType.HitAllTargets;
        }

        public void Dispose() {}

        private bool _isAlive = true;
        private readonly BulletImpactType _impactType;
    }
}
