using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class RemoveOccupantNode : ActionNode
    {
        public RemoveOccupantNode(int id, int starId) : base(id, NodeType.DestroyOccupants)
        {
            _starId = starId;
        }

        protected override void InvokeAction(IQuestActionProcessor processor)
        {
            processor.SuppressOccupant(_starId, true);
        }

        private readonly int _starId;
    }
}
