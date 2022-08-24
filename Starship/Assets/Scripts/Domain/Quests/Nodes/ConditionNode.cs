using System.Collections.Generic;
using GameDatabase.Enums;
using GameServices.Quests;
using Services.Localization;

namespace Domain.Quests
{
    public class ConditionNode : INode
    {
        public ConditionNode(int id, string description)
        {
            _id = id;
            _description = description;
        }

        public int Id { get { return _id; } }
        public NodeType Type { get { return NodeType.Condition; } }
        public INode TargetNode { get; set; }
        public INodeRequirements Requirements { get; set; }

        public string GetRequirementsText(ILocalization localization)
        {
            if (!string.IsNullOrEmpty(_description))
                return localization.GetString(_description);

            if (Requirements != null)
                return Requirements.GetDescription(localization);

            return string.Empty;
        }

        public bool TryGetBeacons(ICollection<int> beacons)
        {
            if (Requirements == null || Requirements.BeaconPosition < 0)
                return false;

            beacons.Add(Requirements.BeaconPosition);
            return true;
        }

        public void Initialize() { }

        public bool TryProcessEvent(IQuestEventData eventData, out INode target)
        {
            return TryProceed(out target);
        }

        public bool TryProceed(out INode target)
        {
            if (Requirements != null && !Requirements.IsMet)
            {
                target = this;
                return false;
            }

            target = TargetNode;
            return true;
        }

        public bool ActionRequired { get { return false; } }
        public bool TryInvokeAction(IQuestActionProcessor processor) { return false; }

        private readonly int _id;
        private readonly string _description;
    }
}
