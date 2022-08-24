using Constructor.Model;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class InfectedModification : IShipModification
    {
        public InfectedModification(int seed, IDatabase database)
        {
            Seed = seed;
            _device = database.GetDevice(new ItemId<Device>(18)); // Toxic waste
        }

        public ModificationType Type => ModificationType.Infected;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_Infected", "30", "1");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.RegenerationRate += 0.01f;
            stats.BaseArmorMultiplier *= 0.7f;
            stats.BuiltinDevices += _device;
        }

        public int Seed { get; }

        private readonly Device _device;
    }
}
