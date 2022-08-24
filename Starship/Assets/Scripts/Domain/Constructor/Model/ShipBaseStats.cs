using GameDatabase.DataModel;
using GameDatabase.Model;

namespace Constructor.Model
{
    public struct ShipBaseStats
    {
        public StatMultiplier BaseArmorMultiplier;
        public StatMultiplier BaseWeightMultiplier;
        public StatMultiplier EnergyResistanceMultiplier;
        public StatMultiplier HeatResistanceMultiplier;
        public StatMultiplier KineticResistanceMultiplier;
        public float RegenerationRate;
        public bool AutoTargeting;
        public Layout Layout;
        public ImmutableCollection<Device> BuiltinDevices;
    }
}
