using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class TradingNode : ActionNode
    {
        public TradingNode(int id, ILoot loot)
            : base(id, NodeType.ReceiveItem)
        {
            _loot = loot;
        }

        protected override void InvokeAction(IQuestActionProcessor processor)
        {
            processor.StartTrading(_loot);
        }

        private readonly ILoot _loot;
    }
}
