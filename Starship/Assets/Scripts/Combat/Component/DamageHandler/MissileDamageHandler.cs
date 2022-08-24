using Combat.Collision;
using Combat.Component.Unit;

namespace Combat.Component.DamageHandler
{
    public class MissileDamageHandler : IDamageHandler
    {
        public MissileDamageHandler(IUnit unit, float hitPoints)
        {
            _unit = unit;
            _hitPoints = hitPoints;
        }

        public CollisionEffect ApplyDamage(Impact impact)
        {
            impact.ApplyImpulse(_unit.Body);
            var damage = impact.GetTotalDamage(Resistance.Empty);
            _hitPoints -= damage;
            
            return _hitPoints > 0 ? CollisionEffect.None : CollisionEffect.Destroy;
        }

        public void Dispose() { }

        private float _hitPoints;
        private readonly IUnit _unit;
    }
}
