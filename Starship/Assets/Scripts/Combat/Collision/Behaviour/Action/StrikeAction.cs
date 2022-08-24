using Combat.Collision.Manager;
using Combat.Component.Unit;
using GameDatabase.Enums;

namespace Combat.Collision.Behaviour.Action
{
    public class StrikeAction : ICollisionAction
    {
        public StrikeAction(float damageMultiplier, bool multipleTargets)
        {
            _multipleTargets = multipleTargets;
            _damageMultiplier = damageMultiplier;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (!collisionData.IsNew || !_isAlive)
                return;

            var impulse = self.Body.Velocity*self.Body.Weight;
            var damage = impulse.magnitude*_damageMultiplier;

            targetImpact.AddImpulse(collisionData.Position, impulse);
            targetImpact.AddDamage(DamageType.Impact, damage);

            _isAlive = _multipleTargets;
        }

        public void Dispose() { }

        private bool _isAlive = true;
        private readonly bool _multipleTargets;
        private readonly float _damageMultiplier;
    }
}
