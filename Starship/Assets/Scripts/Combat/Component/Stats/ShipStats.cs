using Combat.Collision;
using Combat.Component.Mods;
using Combat.Unit.HitPoints;
using Constructor;

namespace Combat.Component.Stats
{
    public class ShipStats : IStats
    {
        public ShipStats(IShipSpecification spec)
        {
            var stats = spec.Stats;

            _resistance = new Resistance
            {
                Energy = stats.EnergyResistancePercentage,
                EnergyDrain = stats.EnergyAbsorptionPercentage,
                Heat = stats.ThermalResistancePercentage,
                Kinetic = stats.KineticResistancePercentage
            };

            WeaponDamageMultiplier = stats.DamageMultiplier.Value;
            RammingDamageMultiplier = stats.RammingDamageMultiplier;
            HitPointsMultiplier = stats.ArmorMultiplier.Value;

            if (stats.ArmorPoints < 0.1f)
                _armorPoints = new EmptyResources();
            else if (stats.ArmorRepairRate > 0)
                _armorPoints = new Energy(stats.ArmorPoints, stats.ArmorRepairRate, stats.ArmorRepairCooldown);
            else
                _armorPoints = new HitPoints(stats.ArmorPoints);

            _shieldPoints = new Energy(stats.ShieldPoints, stats.ShieldRechargeRate, stats.ShieldRechargeCooldown);
            _energyPoints = new Energy(stats.EnergyPoints, stats.EnergyRechargeRate, stats.EnergyRechargeCooldown);
        }

        public bool IsAlive { get { return _armorPoints.Value > 0; } }

        public IResourcePoints Armor { get { return _armorPoints; } }
        public IResourcePoints Shield { get { return _shieldPoints; } }
        public IResourcePoints Energy { get { return _energyPoints; } }

        public float WeaponDamageMultiplier { get; private set; }
        public float RammingDamageMultiplier { get; private set; }
        public float HitPointsMultiplier { get; private set; }

        public Resistance Resistance
        {
            get
            {
                var resistance = _resistance;
                _modifications.Apply(ref resistance);
                return resistance;
            }
        }

        public Modifications<Resistance> Modifications { get { return _modifications; } }

        public IDamageIndicator DamageIndicator { get; set; }

        public float TimeFromLastHit { get; private set; }

        public void ApplyDamage(Impact impact)
        {
            if (Shield.Exists)
                impact.ApplyShield(Shield.Value - impact.ShieldDamage);
            else
                impact.ShieldDamage = 0;

            var resistance = Resistance;

            if (DamageIndicator != null)
                DamageIndicator.ApplyDamage(impact.GetDamage(resistance));

            var damage = impact.GetTotalDamage(resistance);
            if (damage > 0.1f)
                TimeFromLastHit = 0;

            if (resistance.EnergyDrain > 0.01f)
            {
                var energy = resistance.EnergyDrain * impact.EnergyDamage/HitPointsMultiplier;
                Energy.Get(-energy);
            }

            damage -= impact.Repair;
            Armor.Get(damage);
            Energy.Get(impact.EnergyDrain);
            Shield.Get(impact.ShieldDamage);

            if (impact.Effects.Contains(CollisionEffect.Destroy))
                Armor.Get(Armor.MaxValue);
        }

        public void UpdatePhysics(float elapsedTime)
        {
            if (!IsAlive)
                return;

            if (DamageIndicator != null)
                DamageIndicator.Update(elapsedTime);

            _energyPoints.Update(elapsedTime);
            _armorPoints.Update(elapsedTime);
            _shieldPoints.Update(elapsedTime);

            TimeFromLastHit += elapsedTime;
        }

        public void Dispose()
        {
            if (DamageIndicator != null)
                DamageIndicator.Dispose();
        }

        private readonly IResourcePoints _armorPoints;
        private readonly IResourcePoints _shieldPoints;
        private readonly IResourcePoints _energyPoints;
        private readonly Resistance _resistance;
        private readonly Modifications<Resistance> _modifications = new Modifications<Resistance>();
    }
}
