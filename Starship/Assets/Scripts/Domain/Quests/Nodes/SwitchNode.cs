using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDatabase.Enums;
using GameServices.Quests;
using Services.Localization;

namespace Domain.Quests
{
    public class SwitchNode : INode
    {
        public SwitchNode(int id, string description)
        {
            _id = id;
            _description = description;
        }

        public int Id { get { return _id; } }
        public NodeType Type { get { return NodeType.Switch; } }
        public INode DefaultNode { get; set; }

        public string GetRequirementsText(ILocalization localization)
        {
            if (!string.IsNullOrEmpty(_description))
                return localization.GetString(_description);

            //#if UNITY_EDITOR
            //            text.AppendLine(GetType().Name + " - " + _id);
            //#endif

            return string.Join("\n", _transitions.Select(item =>
                item.Requirements.GetDescription(localization)).Where(item => !string.IsNullOrEmpty(item)).ToArray());
        }

        public bool TryGetBeacons(ICollection<int> beacons)
        {
            var result = false;

            foreach (var transition in _transitions)
            {
                if (transition.Requirements.BeaconPosition < 0) continue;
                beacons.Add(transition.Requirements.BeaconPosition);
                result = true;
            }

            return result;
        }

        public void Initialize() {}

        public void AddTransition(INodeRequirements requirements, INode node)
        {
            _transitions.Add(new Transition { Requirements = requirements, TargetNode = node });
        }

        public bool TryProcessEvent(IQuestEventData eventData, out INode target)
        {
            return TryProceed(out target);
        }

        public bool TryProceed(out INode target)
        {
            foreach (var transition in _transitions)
            {
                if (!transition.Requirements.IsMet) continue;

                target = transition.TargetNode;
                return true;
            }

            if (DefaultNode != null)
            {
                target = DefaultNode;
                return true;
            }

            target = this;
            return false;
        }

        public bool ActionRequired { get { return false; } }
        public bool TryInvokeAction(IQuestActionProcessor processor) { return false; }

        private readonly int _id;
        private readonly string _description;
        private readonly List<Transition> _transitions = new List<Transition>();

        private struct Transition
        {
            public INodeRequirements Requirements;
            public INode TargetNode;
        }
    }
}
