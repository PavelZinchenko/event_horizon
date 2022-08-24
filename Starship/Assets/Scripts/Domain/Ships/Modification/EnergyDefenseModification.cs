using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class EnergyDefenseModification : IShipModification
    {
        public EnergyDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.EnergyDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_EnergyDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.EnergyResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
