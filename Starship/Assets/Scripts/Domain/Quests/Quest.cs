using System;
using System.Collections.Generic;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameServices.Quests;
using Services.Localization;

namespace Domain.Quests
{
    public class Quest : IQuest
    {
        public Quest(QuestModel questModel, int starId, int seed)
        {
            _questModel = questModel;
            _starId = starId;
            _seed = seed;
        }

        public QuestModel Model { get { return _questModel; } }
        public int Id { get { return _questModel.Id.Value; } }
        public int StarId { get { return _starId; } }
        public int Seed { get { return _seed; } }
        public int NodeId { get { return ActiveNode.Id; } }
        public string GetRequirementsText(ILocalization localization) { return ActiveNode.GetRequirementsText(localization); }

        public void Initialize(INode node)
        {
            SetActiveNode(node);
        }

        public bool TryGetBeacons(ICollection<int> beacons)
        {
            return ActiveNode.TryGetBeacons(beacons);
        }

        private INode ActiveNode { get { return _activeNode ?? TerminalNode.ErrorNode; } }

        public QuestStatus Status
        {
            get
            {
                if (_activeNode == null) return QuestStatus.Error;
                switch (_activeNode.Type)
                {
                    case NodeType.CompleteQuest:
                        return QuestStatus.Completed;
                    case NodeType.FailQuest:
                        return QuestStatus.Failed;
                    case NodeType.CancelQuest:
                        return QuestStatus.Cancelled;
                    case NodeType.Undefined:
                        return QuestStatus.Error;
                }

                return _activeNode.ActionRequired ? QuestStatus.ActionRequired : QuestStatus.InProgress;
            }
        }

        public bool TryInvokeAction(IQuestActionProcessor processor)
        {
            if (!ActiveNode.TryInvokeAction(processor)) return false;
            UpdateActiveNode();
            return true;
        }

        public bool TryProcessEvent(IQuestEventData data)
        {
            INode target;
            if (!ActiveNode.TryProcessEvent(data, out target)) return false;
            SetActiveNode(target);
            return true;
        }

        private void SetActiveNode(INode node)
        {
            if (node != null) node.Initialize();
            _activeNode = node;
            UpdateActiveNode();
        }

        private void UpdateActiveNode()
        {
            if (_activeNode == null) return;

            INode node;
            var counter = 100;
            while (_activeNode.TryProceed(out node))
            {
                _activeNode = node;
                if (_activeNode == null) break;
                _activeNode.Initialize();

                if (--counter <= 0)
                {
                    UnityEngine.Debug.LogException(new InvalidOperationException("Quest - infinite loop"));
                    _activeNode = null;
                    break;
                }
            }
        }

        private INode _activeNode;
        private readonly int _seed;
        private readonly int _starId;
        private readonly QuestModel _questModel;
    }
}
