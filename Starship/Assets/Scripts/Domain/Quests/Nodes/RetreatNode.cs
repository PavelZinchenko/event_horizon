using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class RetreatNode : ActionNode
    {
        public RetreatNode(int id) : base(id, NodeType.Retreat) {}

        protected override void InvokeAction(IQuestActionProcessor processor)
        {
            processor.Retreat();
        }
    }
}
