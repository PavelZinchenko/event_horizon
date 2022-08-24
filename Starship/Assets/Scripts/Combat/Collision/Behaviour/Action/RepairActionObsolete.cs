using Combat.Collision.Manager;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using GameDatabase.Enums;

namespace Combat.Collision.Behaviour.Action
{
    public class RepairActionObsolete : ICollisionAction
    {
        public RepairActionObsolete(DamageType damageType, float dps, UnitSide unitSide)
        {
            _damageType = damageType;
            _damagePerSecond = dps;
            _unitSide = unitSide;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (target.Type.Side.IsAlly(_unitSide))
                targetImpact.Repair += _damagePerSecond*collisionData.TimeInterval;
            else
                targetImpact.AddDamage(_damageType, _damagePerSecond*collisionData.TimeInterval);
        }

        public void Dispose() { }

        private readonly float _damagePerSecond;
        private readonly DamageType _damageType;
        private readonly UnitSide _unitSide;
    }
}
