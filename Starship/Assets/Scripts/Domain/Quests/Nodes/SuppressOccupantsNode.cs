using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class SuppressOccupantNode : ActionNode
    {
        public SuppressOccupantNode(int id, int starId, bool destroy) 
            : base(id, destroy ? NodeType.DestroyOccupants : NodeType.SuppressOccupants)
        {
            _starId = starId;
            _destroy = destroy;
        }

        protected override void InvokeAction(IQuestActionProcessor processor)
        {
            processor.SuppressOccupant(_starId, _destroy);
        }

        private readonly int _starId;
        private readonly bool _destroy;
    }
}
