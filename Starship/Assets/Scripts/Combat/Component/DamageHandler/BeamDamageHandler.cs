using Combat.Collision;
using Combat.Component.Unit;
using Combat.Unit;

namespace Combat.Component.DamageHandler
{
    public class BeamDamageHandler : IDamageHandler
    {
        public BeamDamageHandler(IUnit unit)
        {
            _unit = unit;
        }

        public CollisionEffect ApplyDamage(Impact impact)
        {
            var owner = _unit.Type.Owner;

            if (impact.Repair > 0 && owner.IsActive())
            {
                owner.Affect(new Impact { Repair = impact.Repair });
            }

            return CollisionEffect.None;
        }

        public void Dispose() { }

        private readonly IUnit _unit;
    }
}
