using GameDatabase.Model;

namespace Constructor.Model
{
    public struct ShipBonuses
    {
        public StatMultiplier ArmorPointsMultiplier;
        public StatMultiplier ShieldPointsMultiplier;
        public StatMultiplier EnergyMultiplier;
        public StatMultiplier DamageMultiplier;
        public StatMultiplier VelocityMultiplier;
        public StatMultiplier RammingDamageMultiplier;
        public StatMultiplier ShieldRechargeMultiplier;

        public float ExtraHeatResistance;
        public float ExtraEnergyResistance;
        public float ExtraKineticResistance;

        public ShipBonuses CopyWith(
            StatMultiplier? armorPointsMultiplier = null,
            StatMultiplier? shieldPointsMultiplier = null,
            StatMultiplier? energyMultiplier = null,
            StatMultiplier? damageMultiplier = null,
            StatMultiplier? velocityMultiplier = null,
            StatMultiplier? rammingDamageMultiplier = null,
            StatMultiplier? shieldRechargeMultiplier = null,
            float? extraHeatResistance = null,
            float? extraEnergyResistance = null,
            float? extraKineticResistance = null
        )
        {
            return new ShipBonuses
            {
                ArmorPointsMultiplier = armorPointsMultiplier ?? ArmorPointsMultiplier,
                ShieldPointsMultiplier = shieldPointsMultiplier ?? ShieldPointsMultiplier,
                EnergyMultiplier = energyMultiplier ?? EnergyMultiplier,
                DamageMultiplier = damageMultiplier ?? DamageMultiplier,
                VelocityMultiplier = velocityMultiplier ?? VelocityMultiplier,
                RammingDamageMultiplier = rammingDamageMultiplier ?? RammingDamageMultiplier,
                ShieldRechargeMultiplier = shieldRechargeMultiplier ?? ShieldRechargeMultiplier,
                ExtraHeatResistance = extraHeatResistance ?? ExtraHeatResistance,
                ExtraEnergyResistance = extraEnergyResistance ?? ExtraEnergyResistance,
                ExtraKineticResistance = extraKineticResistance ?? ExtraKineticResistance,
            };
        }
    }
}
