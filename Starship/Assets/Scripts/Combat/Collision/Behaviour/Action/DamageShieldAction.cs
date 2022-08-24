using Combat.Collision.Manager;
using Combat.Component.Unit;

namespace Combat.Collision.Behaviour.Action
{
    public class DamageShieldAction : ICollisionAction
    {
        public DamageShieldAction(float damage, bool multipleTargets)
        {
            _multipleTargets = multipleTargets;
            _damage = damage;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (!collisionData.IsNew || !_isAlive)
                return;

            targetImpact.ShieldDamage += _damage;
            _isAlive = _multipleTargets;
        }

        public void Dispose() { }

        private bool _isAlive = true;
        private readonly float _damage;
        private readonly bool _multipleTargets;
    }
}
