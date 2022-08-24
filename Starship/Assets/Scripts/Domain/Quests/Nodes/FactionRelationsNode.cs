using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class FactionRelationsNode : ActionNode
    {
        public FactionRelationsNode(int id, int starId, int value, bool additive)
            : base(id, additive ? NodeType.ChangeCharacterRelations : NodeType.SetCharacterRelations)
        {
            _starId = starId;
            _value = value;
            _additive = additive;
        }

        protected override void InvokeAction(IQuestActionProcessor processor)
        {
            processor.SetFactionRelations(_starId, _value, _additive);
        }

        private readonly int _starId;
        private readonly int _value;
        private readonly bool _additive;
    }
}
