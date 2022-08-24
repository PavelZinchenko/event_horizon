using Combat.Collision;
using Combat.Unit.Auxiliary;

namespace Combat.Component.DamageHandler
{
    public class EnergyShieldDamageHandler : IDamageHandler
    {
        public EnergyShieldDamageHandler(IAuxiliaryUnit shield, float energyConsumption)
        {
            _shield = shield;
            _energyConsumption = energyConsumption;
        }

        public CollisionEffect ApplyDamage(Impact impact)
        {
            impact.ApplyImpulse(_shield.Body);
            impact.RemoveImpulse();

            var parent = _shield.Type.Owner;
            if (parent == null)
                return CollisionEffect.None;

            var damage = impact.GetTotalDamage(Resistance.Empty);

            if (parent.Stats.Energy.TryGet(damage*_energyConsumption))
            {
                impact.RemoveDamage();
            }
            else
            {
                var energy = parent.Stats.Energy.Value;
                parent.Stats.Energy.Get(energy);
                impact.RemoveDamage(energy / _energyConsumption, Resistance.Empty);
                _shield.Enabled = false;
            }

            parent.Affect(impact);

            return CollisionEffect.None;
        }

        public void Dispose()
        {
        }

        private readonly IAuxiliaryUnit _shield;
        private readonly float _energyConsumption;
    }
}
