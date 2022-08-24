using Constructor.Model;
using GameDatabase.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class LightWeightModification : IShipModification
    {
        public LightWeightModification(int seed)
        {
            Seed = seed;
            _value = new StatMultiplier(-0.2f);
        }

        public ModificationType Type => ModificationType.LightWeight;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_Lightweight", 20);
        }

        public void Apply(ref ShipBaseStats ship)
        {
            ship.BaseWeightMultiplier += _value;
        }

        public int Seed { get; }

        private readonly StatMultiplier _value;
    }
}
