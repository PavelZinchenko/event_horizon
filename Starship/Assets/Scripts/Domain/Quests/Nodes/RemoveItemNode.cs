using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class RemoveItemNode : ActionNode
    {
        public RemoveItemNode(int id, ILoot loot)
            : base(id, NodeType.ReceiveItem)
        {
            _loot = loot;
        }

        protected override void InvokeAction(IQuestActionProcessor processor) { _loot.Remove(); }

        private readonly ILoot _loot;
    }
}
