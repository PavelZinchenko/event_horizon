using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class LootNode : ActionNode
    {
        public LootNode(int id, ILoot loot)
            : base(id, NodeType.ReceiveItem)
        {
            _loot = loot;
        }

        protected override void InvokeAction(IQuestActionProcessor processor) { _loot.Consume(); }

        private readonly ILoot _loot;
    }
}
