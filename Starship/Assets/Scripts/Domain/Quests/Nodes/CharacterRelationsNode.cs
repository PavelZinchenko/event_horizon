using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class CharacterRelationsNode : ActionNode
    {
        public CharacterRelationsNode(int id, int characterId, int value, bool additive)
            : base(id, additive ? NodeType.ChangeCharacterRelations : NodeType.SetCharacterRelations)
        {
            _characterId = characterId;
            _value = value;
            _additive = additive;
        }

        protected override void InvokeAction(IQuestActionProcessor processor)
        {
            processor.SetCharacterRelations(_characterId, _value, _additive);
        }

        private readonly int _characterId;
        private readonly int _value;
        private readonly bool _additive;
    }
}
