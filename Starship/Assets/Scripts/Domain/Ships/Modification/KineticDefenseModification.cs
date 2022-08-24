using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class KineticDefenseModification : IShipModification
    {
        public KineticDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.KineticDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_KineticDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.KineticResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
