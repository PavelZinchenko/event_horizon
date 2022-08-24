using Combat.Collision;
using Combat.Component.Unit;

namespace Combat.Component.DamageHandler
{
    public class DefaultDamageHandler : IDamageHandler
    {
        public DefaultDamageHandler(IUnit unit)
        {
            _unit = unit;
        }

        public CollisionEffect ApplyDamage(Impact impact)
        {
            impact.ApplyImpulse(_unit.Body);
            return CollisionEffect.None;
        }

        public void Dispose() {}

        private readonly IUnit _unit;
    }
}
