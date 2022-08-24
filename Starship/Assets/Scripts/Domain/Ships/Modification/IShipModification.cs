using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public interface IShipModification
    {
        ModificationType Type { get; }
        string GetDescription(ILocalization localization);

        void Apply(ref ShipBaseStats stats);

        int Seed { get; }
    }
}
