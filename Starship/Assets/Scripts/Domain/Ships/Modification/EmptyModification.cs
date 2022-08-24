using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class EmptyModification : IShipModification
    {
        public ModificationType Type => ModificationType.Empty;
        public string GetDescription(ILocalization localization) { return localization.GetString("$Ship_EmptySlot"); }
        public void Apply(ref ShipBaseStats stats) {}
        public int Seed => 0;
    }
}
