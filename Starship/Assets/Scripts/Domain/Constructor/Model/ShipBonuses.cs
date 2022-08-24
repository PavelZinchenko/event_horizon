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
    }
}
