using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class ShipyardNode : ActionNode
    {
        public ShipyardNode(int id, Faction faction, int level)
            : base(id, NodeType.OpenShipyard)
        {
            _level = level;
            _faction = faction;
        }

        protected override void InvokeAction(IQuestActionProcessor processor)
        {
            processor.OpenShipyard(_faction, _level);
        }

        private readonly int _level;
        private readonly Faction _faction;
    }
}
