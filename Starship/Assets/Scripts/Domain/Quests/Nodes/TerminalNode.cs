using System.Collections.Generic;
using GameDatabase.Enums;
using GameServices.Quests;
using Services.Localization;

namespace Domain.Quests
{
    public class TerminalNode : INode
    {
        public TerminalNode(int id, NodeType type)
        {
            _id = id;
            _type = type;
        }

        public int Id { get { return _id; } }
        public NodeType Type { get { return _type; } }

        public string GetRequirementsText(ILocalization localization)
        {
            switch (Type)
            {
                case NodeType.ComingSoon:
                    return localization.GetString("$Quest_ComingSoon");
                default:
                    return "[" + GetType().Name + "] - " + _id;
            }
        }

        public bool TryGetBeacons(ICollection<int> beacons) { return  false; }

        public void Initialize() {}

        public bool TryProceed(out INode target)
        {
            target = this;
            return false;
        }

        public bool TryProcessEvent(IQuestEventData data, out INode target)
        {
            target = this;
            return false;
        }

        public bool ActionRequired { get { return false; } }
        public bool TryInvokeAction(IQuestActionProcessor processor) { return false; }

        private readonly int _id;
        private readonly NodeType _type;

        public static readonly TerminalNode ErrorNode = new TerminalNode(-1, NodeType.Undefined);
    }
}
