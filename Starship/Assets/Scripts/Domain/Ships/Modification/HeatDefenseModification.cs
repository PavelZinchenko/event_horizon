using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class HeatDefenseModification : IShipModification
    {
        public HeatDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.HeatDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_HeatDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.HeatResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
