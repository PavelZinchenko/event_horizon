using System.Linq;
using Services.Localization;

namespace Domain.Quests
{
    public class ItemsRequirements : IRequirements
    {
        public ItemsRequirements(ILoot loot)
        {
            _loot = loot;
        }

        public bool IsMet { get { return _loot.CanBeRemoved; } }
        public bool CanStart(int starId, int seed) { return IsMet; }

        public string GetDescription(ILocalization localization)
        {
            return string.Join("\n", _loot.Items.Select(GetItemName).ToArray());
        }

        public int BeaconPosition { get { return -1; } }

        private static string GetItemName(LootItem item)
        {
            if (item.Quantity == 1)
                return item.Type.Name;

            return item.Quantity + "x " + item.Type.Name;
        }

        private readonly ILoot _loot;
    }
}
