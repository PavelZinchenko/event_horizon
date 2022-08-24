using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class StartQuestNode : ActionNode
    {
        public StartQuestNode(int id, QuestModel quest)
            : base(id, NodeType.StartQuest)
        {
            _quest = quest;
        }

        protected override void InvokeAction(IQuestActionProcessor processor) { processor.StartQuest(_quest); }

        private readonly QuestModel _quest;
    }
}
