using GameDatabase.Model;

namespace GameDatabase.DataModel
{
    public partial class LootModel
    {
        public LootModel(LootContent content)
        {
            Id = ItemId<LootModel>.Empty;
            Loot = content;
        }
    }
}
