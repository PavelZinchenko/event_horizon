using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class ChangeFactionNode : ActionNode
    {
        public ChangeFactionNode(int id, int starId, Faction faction)
            : base(id, NodeType.ChangeFaction)
        {
            _starId = starId;
            _faction = faction;
        }

        protected override void InvokeAction(IQuestActionProcessor processor)
        {
            processor.ChangeFaction(_starId, _faction);
        }

        private readonly Faction _faction;
        private readonly int _starId;
    }
}