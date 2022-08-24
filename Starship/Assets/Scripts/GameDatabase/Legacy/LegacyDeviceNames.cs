using System.Collections.Generic;

using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Model;

namespace Database.Legacy
{
    public static class LegacyDeviceNames
    {
        public static ItemId<Device> GetId(string value) { int id; return new ItemId<Device>(_items.TryGetValue(value, out id) ? id : -1); }

        public static string GetName(ItemId<Device> id) { return _items.FirstOrDefault(item => item.Value == id.Value).Key; } // TODO: delete

        private static readonly Dictionary<string, int> _items = new Dictionary<string, int>()
        {
            { "Accelerator", 1 },
            { "BrakeDevice", 2 },
            { "Cloaking", 3 },
            { "DecoyDevice", 4 },
            { "Detonator", 5 },
            { "EnergyShield", 6 },
            { "Fortification", 7 },
            { "FrontalShield", 8 },
            { "GravityGenerator", 9 },
            { "PointDefense", 10 },
            { "RepairBotM", 11 },
            { "RepairBotS", 12 },
            { "Stealth", 13 },
            { "SuperStealth", 14 },
            { "Teleporter", 15 },
            { "Teleporter_Boss", 16 },
            { "Teleporter_Boss_1", 17 },
            { "ToxicWasteDevice", 18 },
        };
    }
}
